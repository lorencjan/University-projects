/* File: Program.cs
 * Solution: IPK - Project 1
 * Date: 22.2.2020
 * Author: Jan Lorenc (xloren15)
 * Faculty: Faculty of Information Technology VUT
 * Description: This file contains the main running method for this project
 */

using System;
using System.Net;
  
namespace Server
{ 
    class Program
    { 
        static int Main(string[] args) 
        { 
            int port;
            //check program arguments
            switch(args.Length)
            {
                case 0:
                    Console.Error.WriteLine("Error: Port not specified!");
                    return 1;
                case 1:
                    if(!int.TryParse(args[0], out port) || port < 0 || port > UInt16.MaxValue)
                    {
                        Console.Error.WriteLine($"Error: Port must be a number between 0 and {UInt16.MaxValue}!");
                        return 1;
                    }
                    break;
                default:
                    Console.Error.WriteLine($"Error: Too many arguments, specify just one argument, a port!");
                    return 1;
            }

            //gets the pc name which doesn't has the ip of localhost, on my pc it's completely different, on the ipk referential
            //it's 127.0.1.1 but localhost is 127.0.0.1 -> no connection here to localhost
            //string serverName = Dns.GetHostName(); // ...therefore using localhost
            
            //run the server
            Server server = new Server("localhost", port);
            server.Run();

            return 0;
        } 
    } 
} 