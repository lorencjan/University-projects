/* File: Server.cs
 * Solution: IPK - Project 1
 * Date: 22.2.2020
 * Author: Jan Lorenc (xloren15)
 * Faculty: Faculty of Information Technology VUT
 * Description: In this file is the class for the server, the main bulk of the project
 */

using System;
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Collections.Generic;
  
namespace Server
{ 
    public class Server
    {         
        private int _port;
        private IPHostEntry _host;
        private IPAddress _ipAddress;
        private IPEndPoint _localEndPoint;
        private Socket _listenSocket;
        private readonly int _listenSocketBacklog = 10;
        
        /// <summary>Sets the server up to be ready to run</summary>
        /// <param name="serverName"></param>
        /// <param name="port"></param>
        public Server(string serverName, int port)
        {
            _port = port;
            _host = Dns.GetHostEntry(serverName);                //get the host
            _ipAddress = _host.AddressList[0];                   //acquire the ip address
            _localEndPoint = new IPEndPoint(_ipAddress, _port);  //establish a local endpoint
            //create a listener
            _listenSocket = new Socket(_ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Main server method. Implements the whole server loop with binding and listening beforehand. 
        /// </summary>
        public void Run() 
        {
            try
            { 
                Console.WriteLine("Server is running ... "); 
                // bind the socket to the local endpoint
                _listenSocket.Bind(_localEndPoint); 
                // listen for a client
                _listenSocket.Listen(_listenSocketBacklog); 
        
                while (true)
                { 
                    Console.WriteLine("Server is listening ... "); 
        
                    // wait for a connection
                    Socket clientSocket = _listenSocket.Accept(); 
        
                    // get the data
                    byte[] bytes = new byte[clientSocket.ReceiveBufferSize];
                    int bytesRead = clientSocket.Receive(bytes); 
                    string data = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                    bytes = null;
                    
                    /* process the data */
                    string statusLine = data.Split("\r\n")[0];
                    var info = statusLine.Split(' ');
                    string response = null;
                    if(info.Length >= 3) //Method Url Version ... required request line
                    {
                        string method = info[0];
                        string msg = info[1].Substring(1);
                        string version = info[2];
                        var content = data.Split("\r\n\r\n", 2)[1];
                        //this app requires http protocol
                        if(version.Substring(0, 4) != "HTTP")
                            response = "HTTP/1.1 500 Internal Server Error\r\n";
                        //let's get the response
                        else
                            response = ResolveRequest(method, msg, content);
                    }
                    else
                        response = "HTTP/1.1 400 Bad Request\r\n";

                    // send response to the client
                    clientSocket.Send(Encoding.ASCII.GetBytes(response)); 
                    Console.WriteLine("Server responded."); 

                    // close the client socket
                    clientSocket.Shutdown(SocketShutdown.Both); 
                    clientSocket.Close(); 
                } 
            } 
            catch (Exception e)
            { 
                Console.Error.WriteLine($"Internal error occured!\n{e.Message}");
            } 
        }

        /// <summary>Processes the request according to the method given</summary>
        /// <param name="method">HTTP request method ... only GET and POST are allowed</param>
        /// <param name="msg">Url message ... == content for GET, info for POST</param>
        /// <param name="content">
        /// Received message content ... null for GET (not needed), contains addresses to resolve for POST
        /// </param>
        /// <returns>Full response message to the client in string format</returns>
        private string ResolveRequest(string method, string msg, string content)
        {
            try
            {
                const string badRequest = "HTTP/1.1 400 Bad Request\r\n";
                const string notFound = "HTTP/1.1 404 Not Found\r\n";

                //resolving e.g. this: GET /resolve?name=apple.com&type=A
                if(method == "GET")
                {
                    //is it "resolve" request?
                    var urlParts = msg.Split('?');
                    if(urlParts.Length != 2 || urlParts[0] != "resolve")
                        return badRequest;
                    //are the params ok?
                    var args = urlParts[1].Split('&');
                    if(args.Length != 2)
                        return badRequest;
                    var arg1 = args[0].Split('=');
                    var arg2 = args[1].Split('=');
                    if(arg1.Length != 2 || arg2.Length != 2)
                        return badRequest;
                    var parameters = new Dictionary<string, string>()
                    {
                        {arg1[0], arg1[1]},
                        {arg2[0], arg2[1]},
                    };
                    if(!parameters.ContainsKey("name") || parameters["name"] == String.Empty ||
                       !parameters.ContainsKey("type") || (parameters["type"] != "A" && parameters["type"] != "PTR"))
                        return badRequest;
                    //check format ... type A requires IPv4 address, PTR name of the server
                    if(!CheckInputData(parameters["name"], parameters["type"]))
                        return badRequest;
                    //parameters are ok, let's resolve the request
                    string resolved = Translate(parameters["name"], parameters["type"]);
                    if(resolved == null)
                        return notFound;

                    return BuildSuccessResponse($"{parameters["name"]}:{parameters["type"]}={resolved}");
                }
                //POST /dns-query
                //resolving e.g. curl --data-binary @queries.txt -X POST http://localhost:5353/dns-query
                else if(method == "POST")
                {
                    //check dns-query
                    if(msg != "dns-query")
                        return badRequest;
                    //process urls
                    var contentArr = content.TrimEnd().Split("\n");
                    var responseContent = new StringBuilder();
                    foreach(var input in contentArr)
                    {
                        //get the name and type
                        var parts = input.Split(':');
                        if(parts.Length != 2)
                            return badRequest;
                        string name = parts[0].Trim();
                        string type = parts[1].Trim();
                        if(name == String.Empty || (type != "A" && type != "PTR"))
                            return badRequest;
                        //check format ... type A requires IPv4 address, PTR name of the server
                        if(!CheckInputData(name, type))
                            return badRequest;
                        //get the result
                        string resolved = Translate(name, type);
                        if(resolved != null)
                            responseContent.Append($"{name}:{type}={resolved}\r\n");
                    }
                    string response = responseContent.ToString().Trim();
                    if(response == String.Empty)
                        return notFound;
                    return BuildSuccessResponse(response);
                }
                else //error
                    return "405 Method Not Allowed\r\n";
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
                return "500 Internal Server Error\r\n";
            }
        }

        /// <summary>Translates url to IP address and vice versa depending on the type given</summary>
        /// <param name="name">Server name / IP address</param>
        /// <param name="type">Defines whether to resolve A or PTR ... no others are allowed</param>
        /// <returns>DNS translated server name</returns>
        private string Translate(string name, string type)
        {
            //get the host
            IPHostEntry host;
            try
            {
                host = Dns.GetHostEntry(name);
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
                return null;
            }
            //choose ip address or a name depending on the type ... only A or PTR are allowed
            string resolved = null;
            if(type == "A")
            {
                if(host.AddressList.Length == 0)
                    return null;
                //we want only IPv4 addresses, in the list are both -> return first IPv4 address
                foreach(var ipAddress in host.AddressList)
                {
                    if(ipAddress.AddressFamily == AddressFamily.InterNetwork) //is IPv4?
                    {
                        resolved = ipAddress.ToString();
                        break;
                    }
                }
            }
            else
                resolved = host.HostName;
            return resolved;
        }

        /// <summary>Creates complete response message with a predefined header</summary>
        /// <param name="content">Content of the message</param>
        /// <returns>Response to be sent to the client</returns>
        private string BuildSuccessResponse(string content)
        {
            var response = new StringBuilder();
            response.Append("HTTP/1.1 200 OK\r\n");
            response.Append("Content-Type: text/plain; charset=UTF-8\r\n");
            response.Append($"Content-Length: {content.Length}\r\n\r\n");
            response.Append(content);
            return response.ToString();
        }

        /// <summary>Checks input data format ... type A requires IPv4 address, PTR name of the server</summary>
        /// <param name="name">Server name / IP address</param>
        /// <param name="type">Defines whether to resolve A or PTR ... no others are allowed</param>
        /// <returns>False is A receives IP address or PTR doesn't, otherwise true</returns>
        private bool CheckInputData(string name, string type)
        {
            IPAddress ip = null;
            if(type == "A" && IPAddress.TryParse(name, out ip))
                return false;
            if(type == "PTR" && (!IPAddress.TryParse(name, out ip) || ip.AddressFamily != AddressFamily.InterNetwork))
                return false;
            return true;
        }
    } 
} 