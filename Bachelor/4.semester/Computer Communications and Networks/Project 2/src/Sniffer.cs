// File: Sniffer.cs
// Solution: IPK - Project 2 - ZETA - Sniffer
// Date: 12.4.2020
// Author: Jan Lorenc (xloren15)
// Faculty: Faculty of Information Technology VUT
// Description: This file contains a class representing the packet sniffer

using System;
using System.Net;
using System.Collections.Generic;
using PacketDotNet;
using SharpPcap;

namespace ipk_sniffer
{
    /// <summary>This class responsibility is capturing TCP and UDP packets and reporting it</summary>
    public class Sniffer
    {
        /// <summary>Program command line arguments</summary>
        private readonly Arguments _args;
        /// <summary>List of captured packets in the output format</summary>
        private List<SniffedPacket> _outs;

        /// <summary>Simple constructor assigning program arguments and initializing empty output list</summary>
        /// <param name="args">Program command line arguments</param>
        public Sniffer(Arguments args)
        {
            _args = args;
            _outs = new List<SniffedPacket>();
        }

        /// <summary>Sniffes TCP/UDP packets until it reaches specified number and stores them to the output list</summary>
        public void Sniff()
        {
            int capturedPackets = 0;
            _args.Interface.Open(DeviceMode.Promiscuous, 1000);
            
            Console.WriteLine("Sniffing...");
            while(capturedPackets < _args.PacketCount)
            {
                //skip if no packet was captured or it isn't IP packet (then cannot be tcp/udp)
                RawCapture r = _args.Interface.GetNextPacket();
                if(r == null)
                    continue;
                var packet = Packet.ParsePacket(r.LinkLayerType, r.Data);
                if(!(packet is PacketDotNet.EthernetPacket))
                    continue;
                var ipPacket = packet.Extract<PacketDotNet.IPPacket>();
                if(ipPacket == null)
                    continue;

                // PacketDotNet packet classes don't contain source/dest ip addresses -> need to get it from ipPacket beforehand
                string sourceHostName = ipPacket.SourceAddress.ToString();
                try
                {
                    sourceHostName = Dns.GetHostEntry(sourceHostName).HostName;
                }
                catch{}
                string destHostName = ipPacket.DestinationAddress.ToString();
                try
                {
                    destHostName = Dns.GetHostEntry(destHostName).HostName;
                }
                catch{}

                //is it TCP and do we want TCP?
                var tcpPacket = packet.Extract<PacketDotNet.TcpPacket>();
                if(tcpPacket != null && _args.Tcp)
                {
                    //check if port matches
                    if(_args.Port != 0 && tcpPacket.DestinationPort != _args.Port)
                        continue;
                    //all is well, we can add
                    _outs.Add(new SniffedPacket()
                    {
                        Time = DateTime.Now.ToString("hh:mm:ss.fff"),
                        SourcePort = tcpPacket.SourcePort.ToString(),
                        DestPort = tcpPacket.DestinationPort.ToString(),
                        SourceNameOrIp = sourceHostName,
                        DestNameOrIp = destHostName,
                        Data = tcpPacket.Bytes
                    });
                    capturedPackets++;
                    Console.WriteLine("Captured TCP packet.");
                }
                //is it UDP and do we want UDP?
                var udpPacket = packet.Extract<PacketDotNet.UdpPacket>();
                if(udpPacket != null && _args.Udp)
                {
                    //check if port matches
                    if(_args.Port != 0 && udpPacket.DestinationPort != _args.Port)
                        continue;
                    //all is well, we can add
                    _outs.Add(new SniffedPacket()
                    {
                        Time = DateTime.Now.ToString("hh:mm:ss.fff"),
                        SourcePort = udpPacket.SourcePort.ToString(),
                        DestPort = udpPacket.DestinationPort.ToString(),
                        SourceNameOrIp = sourceHostName,
                        DestNameOrIp = destHostName,
                        Data = udpPacket.Bytes
                    });
                    capturedPackets++;
                    Console.WriteLine("Captured UDP packet.");
                }  
            }
            _args.Interface.Close();
            Console.WriteLine("Sniffing finished.\nCaptured packets:\n");
        }

        /// <summary>Writes out all the captured packets</summary>
        public void Out()
        {
            foreach(var packet in _outs)
            {
                Console.WriteLine($"{packet.Head}\n");
                foreach(var line in packet.GetLines())
                {
                    Console.WriteLine(line);
                }
                Console.WriteLine();
            }
        }
    }
}
