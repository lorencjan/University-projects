/*
 *  File: pro.cpp
 *  Solution: PRL - Proj 2 - Assign preorder order to vertices
 *  Date: 22.04.2022
 *  Author: Jan Lorenc
 *  Description: MPI implementation of ordering input string to order in which preorder would go.
 */

#include <iostream>
#include <mpi.h>
#include <math.h>
#include <vector>

using namespace std;

/**
 * Helper function for MPI sending to avoid boilerplate.
 * 
 * @param data Data to send.
 * @param length Length of the data.
 * @param dest Rank of the destination process to send the value to.
 */
void Send(const void* data, int count, int dest) {
    if(MPI_Send(data, count, MPI_INT, dest, 0, MPI_COMM_WORLD)) {
        cerr << "Error: Failed to send MPI message." << endl;
        MPI_Abort(MPI_COMM_WORLD, EXIT_FAILURE);
    }
}

/**
 * Helper function for MPI receiving to avoid boilerplate.
 * 
 * @param data Data to receive.
 * @param length Length of the data.
 * @param src Rank of the source process to receive the value from.
 * @returns Received value.
 */
void Receive(int* data, int length, int src) {
    if (MPI_Recv(data, length, MPI_INT, src, 0, MPI_COMM_WORLD, nullptr)) {
        cerr << "Error: Failed to receive MPI message." << endl;
        MPI_Abort(MPI_COMM_WORLD, EXIT_FAILURE);
    }
}

/**
 * Helper function for MPI gathering all to avoid boilerplate.
 * 
 * @param data Data to send.
 * @param dest Gathered data.
 * @returns Received value.
 */
void AllGather(int* data, int* dest) {
    if (MPI_Allgather(data, 1, MPI_INT, dest, 1, MPI_INT, MPI_COMM_WORLD)) {
        cerr << "Error: Failed to gather MPI data." << endl;
        MPI_Abort(MPI_COMM_WORLD, EXIT_FAILURE);
    }
}

/**
 * Helper function for MPI barrier to avoid boilerplate.
 */
void Barrier(){
    MPI_Barrier(MPI_COMM_WORLD);
}

/**
 * Gets list of neighbors for Etour.
 * 
 * @param pointTo Where current edge points to.
 * @param inputSize Size of the input string.
 * @returns List of neighbors as vector.
 */
vector<int> getNeighbors(int pointTo, int inputSize){
    vector<int> neighbors;
    int leftChild = 2 * (pointTo + 1);
    if (leftChild <= inputSize) {
        neighbors.push_back(pointTo * 4);
        neighbors.push_back(pointTo * 4 + 1);
    }
    if (leftChild + 1 <= inputSize) {
        neighbors.push_back(pointTo * 4 + 2);
        neighbors.push_back(pointTo * 4 + 3);
    }
    if (pointTo != 0) {
        neighbors.push_back((pointTo - 1) * 2 + 1);
        neighbors.push_back((pointTo - 1) * 2);
    }

    return neighbors;
}

/**
 * Computes Etour.
 * 
 * @param neighbors List of neigbors.
 * @param rank Rank of the current process.
 * @param numOfProcs Total number of processes.
 * @returns Vector representing Etour.
 */
vector<int> getEtour(vector<int> &neighbors, int rank, int numOfProcs) {
    int etour[numOfProcs];
    int reverse = (rank % 2) ? rank - 1 : rank + 1;
    const int numOfNeighbors = neighbors.size();
    const int increment = 2;
    for(int i = 0; i < numOfNeighbors; i += increment) {
        if (neighbors[i] != reverse)
            continue;
        
        etour[rank] = (i + increment < numOfNeighbors) ? neighbors[i + increment] : neighbors[0];
        break;
    }

    AllGather(&etour[rank], etour);
    return vector<int>(etour, etour + sizeof etour / sizeof etour[0]);
}

/**
 * Computes reversed Etour (predecessors).
 * 
 * @param etour Etour which reverse we want.
 * @param rank Rank of the current process.
 * @param numOfProcs Total number of processes.
 * @returns Vector representing reversed Etour.
 */
vector<int> getReversedEtour(vector<int> &etour, int rank, int numOfProcs) {
    int etourReversed[numOfProcs];
    Send(&rank, 1, etour[rank]);
    Receive(&etourReversed[rank], 1, MPI_ANY_SOURCE);
    AllGather(&etourReversed[rank], etourReversed);
    return vector<int>(etourReversed, etourReversed + sizeof etourReversed / sizeof etourReversed[0]);
}

/**
 * Sends itself to its successor.
 * 
 * @param next Successor
 * @param prev Predecessor
 * @param rank Rank of the current process
 */
void sendRequest(int &next, int &prev, int rank){
    if (next != -1)  { // send only if not last
        int data[] = {rank, prev};
        Send(&data, 2, next);
    }
    Barrier(); // wait for all to send
}

/**
 * Anwers on request by sending its weight so that the caller can add up and also whom to ask next.
 * 
 * @param next Successor
 * @param prev Predecessor
 * @param weight Weight of the current process
 */
void receiveRequestAndRespond(int &next, int &prev, int weight){
    // do nothing if I don't expect any message, just wait for others for sync
    if (prev == -1) {
        Barrier();
        return;
    }

    // receive request
    int data[2];
    Receive(data, 2, prev);
    int src = data[0];
    prev = data[1];

    // send answer
    data[0] = weight;
    data[1] = next;
    Send(&data, 2, src);
    Barrier();
}

/**
 * Receives response on its request.
 * 
 * @param next Successor
 * @returns Received weight
 */
int receiveRequestResponse(int &next) {
    // do nothing if I don't expect any message, just wait for others for sync
    if (next == -1) {
        Barrier();
        return 0;
    }

    // receive response and return weight
    int data[2];
    Receive(data, 2, MPI_ANY_SOURCE);
    next = data[1];
    Barrier();
    return data[0];
}

/**
 * Runs the preorder algorithm. Firstly, it initializes weights according to edge type.
 * Secondly, it runs suffix sum and lastly, it corrects the weight.
 * 
 * @param etour
 * @param etourReversed
 * @param rank Rank of the current process.
 * @param numOfProcs Total number of processes.
 * @param inputSize Size of the input string.
 * @returns Final weight
 */
int preorder(vector<int> &etour, vector<int> &etourReversed, int rank, int numOfProcs, int inputSize) {
    // init weights ... 1 for forward edges, 0 for reverse
    int weight = (rank % 2) ? 0 : 1;

    // suffix sum
    Barrier();
    int next = (etour[rank]) ? etour[rank] : -1;
    int prev = (rank) ? etourReversed[rank] : -1;
    for (int i = 0; i < log2(numOfProcs); i++) {
        sendRequest(next, prev, rank);
        receiveRequestAndRespond(next, prev, weight);
        weight += receiveRequestResponse(next);
    }

    // weight correction for forward edges
    if (rank % 2 == 0)
        weight = inputSize - weight;

    return weight;
}

/**
 * Processes receive their order and using MPI_Gather the
 * data are collected and orderd to a sequence for the master.
 * 
 * @param pointTo Where the current proccess == edge points to.
 * @param input Program input string.
 * @param weight Weight of the current process.
 * @param rank Rank of the current process.
 * @param numOfProcs Total number of processes.
 * @returns Result string
 */
string gatherResults(int pointTo, string input, int weight, int rank, int numOfProcs){
    char result[numOfProcs] = {};
    if (rank % 2 == 0) // forward edges send their weight == order
        Send(&pointTo, 1, weight);
        
    if (rank && rank < input.size()) { // all but master (root) get their order
        int order;
        Receive(&order, 1, MPI_ANY_SOURCE);
        result[rank] = input[order];
    }

    if (MPI_Gather(&result[rank], 1, MPI_CHAR, result, 1, MPI_CHAR, 0, MPI_COMM_WORLD)) {
        cerr << "Error: Failed to gather MPI data." << endl;
        MPI_Abort(MPI_COMM_WORLD, EXIT_FAILURE);
    }
    Barrier();

    result[0] = input[0]; // add root
    return string(result);
}

int main(int argc, char * argv[])
{
    string input = argv[1];
    int inputSize = input.size();

    // handle input of single vertex (just root)
    if (inputSize == 1) {
        cout << input;
        return 0;
    }

    int numOfProcs, rank;

    MPI_Init(&argc,&argv);
    MPI_Comm_size(MPI_COMM_WORLD, &numOfProcs);
    MPI_Comm_rank(MPI_COMM_WORLD, &rank);

    // process == route ... find out where it points (even = forward edge, odd = reverse -> from/to)
    int pointTo = (rank % 2) ? rank / 4 : rank / 2 + 1;

    // compute Etour
    auto neighbors = getNeighbors(pointTo, inputSize);
    auto etour = getEtour(neighbors, rank, numOfProcs);
    auto etourReversed = getReversedEtour(etour, rank, numOfProcs);

    auto weight = preorder(etour, etourReversed, rank, numOfProcs, inputSize);

    auto result = gatherResults(pointTo, input, weight, rank, numOfProcs);

    // print result if master
    if (rank == 0)
        cout << result.substr(0, numOfProcs);

    MPI_Finalize();
    return 0;
}
