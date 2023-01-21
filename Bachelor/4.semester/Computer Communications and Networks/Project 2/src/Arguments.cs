// File: Arguments.cs
// Solution: IPK - Project 2 - ZETA - Sniffer
// Date: 12.4.2020
// Author: Jan Lorenc (xloren15)
// Faculty: Faculty of Information Technology VUT
// Description: This file contains Arguments class handling program's command line arguments

using System.Linq;
using System.Collections.Generic;
using SharpPcap;

namespace ipk_sniffer
{
    /// <summary>Handles command line argument parsing and stores the argument values</summary>
    public class Arguments
    {
        /// <summary>Raw command line arguments</summary>
        private readonly List<string> _args;
        /// <summary>Interface on which the sniffer listens</summary>
        public ICaptureDevice Interface { get; set; }

        /// <summary>Port on which the sniffer listens. Default values is 0 signifying all.</summary>
        public uint Port { get; set; }
        /// <summary>Defines whether to listen for TCP packets. Default is true.</summary>
        public bool Tcp { get; set; }
        /// <summary>Defines whether to listen for UDP packets. Default is true.</summary>
        public bool Udp { get; set; }
        /// <summary>Defines how many packets we should capture until the program terminates. Default is 1.</summary>
        public uint PacketCount { get; set; }
        /// <summary>Error message in case of wrong argument set</summary>
        public string ErrorMsg { get; set; }
        
        /// <summary>Simple constructor assigning program arguments</summary>
        /// <param name="args">Program arguments</param>
        public Arguments(string[] args)
        {
            _args = new List<string>(args);
        }
        
        /// <summary>Gets all needed arguments from command line</summary>
        /// <returns>True if the arguments were passed correctly, otherwise false</returns>
        public bool Parse()
        {
            if(GetHelp())
                return false;
            GetInterface();
            GetPort();
            GetPacketTypes();
            GetPacketCount();
            return WereArgumentsValid();
        }

        /// <summary>Finds out whether to display help message and if so, builds it</summary>
        /// <returns>True if help should be shown, false otherwise.</returns>
        private bool GetHelp()
        {
            if(GetIndexFromRawArgs(new List<string>(){"-h", "--help"}) == -1)
                return false;
            
            ErrorMsg = "This is a tcp/udp packet sniffer program. Specify a network interface on which you want to listen.\n";
            ErrorMsg += "Program options:\n";
            ErrorMsg += "-h / --help  Displays this help information\n";
            ErrorMsg += "-i [name]    Network interface where the program sniffs\n";
            ErrorMsg += "-p [number]  Port number on which the program listens (listens on all by default)\n";
            ErrorMsg += "-t / --tcp   Program captures only TCP packets if this option is set\n";
            ErrorMsg += "-u / --udp   Program captures only UDP packets if this option is set\n";
            ErrorMsg += "-n [number]  Number of packets to capture (default is 1)";
            return true;
        }

        /// <summary>Extracts interface identifier from raw command line arguments and stores it into Interface property</summary>
        private void GetInterface()
        {
            //get all available interfaces
            var interfaces = CaptureDeviceList.Instance;
            string listOfInterfaces = string.Empty;
            int count = 1;
            foreach (ICaptureDevice i in interfaces)
            {
                listOfInterfaces += $"{count++}) Name: {i.Name}, Description: {i.Description}\n";
            }
            
            //check if an interface was specified
            int index = _args.IndexOf("-i");
            if(index == -1)
            {
                ErrorMsg = "Argument error: No interface specified. Available network interfaces:\n";
                ErrorMsg += listOfInterfaces;
                return;
            }
            if(index+1 >= _args.Count) // -i option needs to have a parameter
            {
                ErrorMsg = "Argument error: Missing interface identifier!";
                return;
            }

            //set the interfaces whose name was specified, if it exist
            Interface = interfaces.FirstOrDefault(x => x.Name == _args[index + 1]);
            if(Interface == null)
            {
                ErrorMsg = "Argument error: Specified interface not found. Available interfaces:\n";
                ErrorMsg += listOfInterfaces;
                return;
            }

            _args.RemoveAt(index);
            _args.RemoveAt(index); // after removing -i, it's value shifted to -i's index
        }

        /// <summary>Gets the port from the raw arguments</summary>
        private void GetPort()
        {
            //if not specified, default is 0
            int index = _args.IndexOf("-p");
            if(index == -1)
            {
                Port = 0;
                return;
            }
            if(index+1 >= _args.Count) // -p option needs to have a parameter
            {
                ErrorMsg = "Argument error: Missing port value!";
                return;
            }

            //check if the port has correct value
            uint portInt;
            if(!uint.TryParse(_args[index + 1], out portInt) || portInt == 0 || portInt > 65535)
            {
                ErrorMsg = "Argument error: Invalid port value, it must be an integer in range 1-65535!";
                return;
            }
            Port = portInt;

            _args.RemoveAt(index);
            _args.RemoveAt(index); // after removing -p, it's value shifted to -p's index
        }

        /// <summary>Gets from the raw arguments the information, which packets to capture</summary>
        private void GetPacketTypes()
        {
            //check, if we should listen only for one type of packets,
            //if both/none is specified, we capture both, otherwise only the one selected
            int tcpIndex = GetIndexFromRawArgs(new List<string>(){"-t", "--tcp"});
            int udpIndex = GetIndexFromRawArgs(new List<string>(){"-u", "--udp"});
            if(tcpIndex == -1 && udpIndex == -1 || tcpIndex != -1 && udpIndex != -1)
            {
                Tcp = true;
                Udp = true;
            }
            else if(tcpIndex != -1)
            {
                Tcp = true;
                Udp = false;
            }
            else
            {
                Tcp = false;
                Udp = true;
            }

            if(tcpIndex != -1)
                _args.RemoveAt(tcpIndex);
            if(udpIndex != -1)
                _args.RemoveAt(tcpIndex != -1 && tcpIndex < udpIndex ? udpIndex-1 : udpIndex);
        }

        /// <summary>Gets the packet count from the raw arguments.</summary>
        private void GetPacketCount()
        {
            //if not given, default is 1
            int index = _args.IndexOf("-n");
            if(index == -1)
            {
                PacketCount = 1;
                return;
            }
            if(index+1 >= _args.Count) // -p option needs to have a parameter
            {
                ErrorMsg = "Argument error: Missing packet count value!";
                return;
            }

            //check if the count has correct value
            uint packetCountInt;
            if(!uint.TryParse(_args[index + 1], out packetCountInt) || packetCountInt == 0)
            {
                ErrorMsg = "Argument error: Packet count must be an integer greater than 0!";
                return;
            }
            PacketCount = packetCountInt;

            _args.RemoveAt(index);
            _args.RemoveAt(index); // after removing -n, it's value shifted to -n's index
        }

        /// <summary>After the extraction of all arguments, none should remain.</summary>
        /// <returns>True on successful arguments parsing, false otherwise.</returns>
        private bool WereArgumentsValid()
        {
            if(_args.Count == 0 && ErrorMsg == null)
                return true;
            if(ErrorMsg == null)
                ErrorMsg = "Argument error: Invalid format of program arguments!";
            return false;
        }

        /// <summary>Finds the first index of given values from raw arguments.</summary>
        /// <param name="args">Program arguments to look for</param>
        /// <returns>Index of first occurence from given list or -1 of not found.</returns>
        private int GetIndexFromRawArgs(List<string> args)
        {
            foreach(var arg in args)
            {
                int index = _args.IndexOf(arg);
                if(index != -1)
                    return index;
            }
            return -1;
        }
    }
}
