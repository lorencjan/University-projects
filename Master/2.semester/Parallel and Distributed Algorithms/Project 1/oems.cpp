/*
 *  File: oems.cpp
 *  Solution: PRL - Proj 1 - Odd-even merge sort
 *  Date: 19.03.2022
 *  Author: Jan Lorenc
 *  Description: MPI implementation of odd-even merge sort algorithm for sequence of 8 numbers.
 *               Implementations follows the HW net of 19 processors unit as demonstrated in the lecture.
 *               Each process simulates one of the processors, no processors optimalizations are made.
 */

#include <iostream>
#include <fstream>
#include <mpi.h>

using namespace std;

/**
 * Holds ranks of processes from which to receive inputs and to which to send the outputs (lower/higher of the inputs).
 * Basically to which processors is this one connected in the net.
 */
typedef struct Comparer {
    // ranks of the processes from which to receive input values
    int src1;
    int src2;
    // ranks of the processes to which to send the lower/higher of the 2 compared values
    int dest_low;
    int dest_high;
} Comparer_t;

/**
 * Hardcoded connections of net for 8 inputs in 3 layers - 4x 1x1, 2x 2x2, 1x 4x4
 * Corresponding image with ids is provided in the documentation.
 */
const Comparer_t Net[] = {
    {0, 0, 5, 4}, {0, 0, 5, 4}, {0, 0, 7, 6}, {0, 0, 7, 6},         // first layer == four 1x1 processors ... input is from master that reads the file
    {0, 1, 8, 10}, {0, 1, 13, 8}, {2, 3, 9, 10}, {2, 3, 13, 9},   // second layer == two 2x2 blocks        - input
    {4, 5, 11, 12}, {6, 7, 11, 12},                                 // trailing 1x1 processors in 2x2 blocks - tail
    {4, 6, 14, 0}, {8, 9, 18, 14}, {8, 9, 15, 16}, {5, 7, 0, 15},   // third layer == 4x4 block                            - input
    {10, 11, 17, 16}, {12, 13, 18, 17},                             // trailing 1x1 procs of 2x2 blocks in 4x4             - middle
    {12, 14, 0, 0}, {14, 15, 0, 0}, {11, 15, 0, 0}                  // trailing 1x1 procs of 4x4 block - sending to master - tail
};

/**
 * Helper function for MPI sending to avoid boilerplate.
 * 
 * @param value Value to send.
 * @param dest Rank of the destination process to send the value to.
 */
void Send(int value, int dest) {
    if(MPI_Send(&value, 1, MPI_INT, dest, 0, MPI_COMM_WORLD)) {
        cerr << "Error: Failed to send MPI message." << endl;
        MPI_Abort(MPI_COMM_WORLD, EXIT_FAILURE);
    }
}

/**
 * Helper function for MPI receiving to avoid boilerplate.
 * 
 * @param src Rank of the source process to receive the value from.
 * @returns Received value.
 */
int Receive(int src) {
    int value;
    if (MPI_Recv(&value, 1, MPI_INT, src, 0, MPI_COMM_WORLD, nullptr)) {
        cerr << "Error: Failed to receive MPI message." << endl;
        MPI_Abort(MPI_COMM_WORLD, EXIT_FAILURE);
    }

    return value;
}

/**
 * There should be exactly 8 1B numbers in the file. Exits with error if not.
 */
void input_error() {
    cerr << "Error: Invalid input. Eight 1B numbers expected." << endl;
    MPI_Abort(MPI_COMM_WORLD, EXIT_FAILURE);
}

/**
 * Reads 8 numbers from the file (fails if file cannot be opened or doesn't contain 8 numbers).
 * It also prints the read numbers to standard output and sends them to corresponding 1x1 comparers.
 */
void read_input() {
    const string filename = "numbers";
    const int input_size = 8;
    
    // load file stream
	ifstream stream(filename);
	if (!stream.is_open() || stream.bad())	{
		cerr << "Error: Failed to read input file." << endl;
		MPI_Abort(MPI_COMM_WORLD, EXIT_FAILURE);
	}
    
    // read the file till the end and print the numbers
    int i = 0;
	while (!stream.eof()) {
        int number = stream.get();
        if (stream.eof())
            break;

        if (i > input_size - 1)
            input_error();

		cout << (i != 0 ? " " : "") << number;

        //send to first 0-3 rank processes symbolizing first 4 1x1 blocks/processors
        Send(number, i / 2);
        i++;
    }
	cout << endl;

    if (i != input_size)
        input_error();
}

/**
 * Receives numbers from the output processors in the net and prints them in sorted order.
 */
void print_output() {
    int output_ranks[] = {13, 18, 18, 17, 17, 16, 16, 10};
    for (int rank : output_ranks)
        cout << Receive(rank) << endl;
}

/**
 * Simulates the hw processor. Receives 2 values, compares them and sends higher/lower to appropriate processor.
 *
 * @param rank Rank of the current process.
 */
void simulate_comparer(const int rank) {
    auto conn = Net[rank];

    int x = Receive(conn.src1);
    int y = Receive(conn.src2);

    if (x > y) {
        Send(y, conn.dest_low);
        Send(x, conn.dest_high);
    }
    else {
        Send(x, conn.dest_low);
        Send(y, conn.dest_high);
    }
}

int main(int argc, char *argv[]) {
    int rank;
    MPI_Init(&argc, &argv);
    MPI_Comm_rank(MPI_COMM_WORLD, &rank);
    
    if (rank == 0) // master
	    read_input();

	simulate_comparer(rank);

    if (rank == 0) // master
	    print_output();

	MPI_Finalize();
	return EXIT_SUCCESS;
}