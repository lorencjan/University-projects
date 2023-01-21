// File: SniffedPacket.cs
// Solution: IPK - Project 2 - ZETA - Sniffer
// Date: 12.4.2020
// Author: Jan Lorenc (xloren15)
// Faculty: Faculty of Information Technology VUT
// Description: This file contains a class representing packet to be outputted

using System.Collections.Generic;

namespace ipk_sniffer
{
    /// <summary>Contains output information about single TCP/UDP packet that was sniffed</summary>
    public class SniffedPacket
    {
        /// <summary>Data lines to be outputted</summary>
        private List<string> _lines = new List<string>();
        
        /// <summary>Time of the capture in hh:mm:ss.fff format</summary>
        public string Time { get; set; }
        /// <summary>Host name of the source device or it's IP address if the name couldn't be retrieved</summary>
        public string SourceNameOrIp { get; set; }
        /// <summary>Port number of the source device</summary>
        public string SourcePort { get; set; }
        /// <summary>Host name of the destination device or it's IP address if the name couldn't be retrieved</summary>
        public string DestNameOrIp { get; set; }
        /// <summary>Port number of the destination device</summary>
        public string DestPort { get; set; }
        /// <summary>Getter for output header information</summary>
        public string Head { get { return $"{Time} {SourceNameOrIp} : {SourcePort} > {DestNameOrIp} : {DestPort}";}}
        /// <summary>Packet raw byte data</summary>
        public byte[] Data;

        /// <summary>Parses the byte data into ascii and creates lines for 16 byte of data</summary>
        /// <returns>Returns list of formatted output lines</returns>
        public List<string> GetLines()
        {
            int lineNumber = 0;
            string bytes = string.Empty;
            string ascii = string.Empty;
            for(int i = 0; i < Data.Length; i++)
            {
                //start new line every 16 bytes
                if(i % 16 == 0 && i != 0)
                {
                    _lines.Add($"0x{lineNumber.ToString("X4")}:{bytes} {ascii}");
                    bytes = string.Empty;
                    ascii = string.Empty;
                    lineNumber += 16;
                }
                //on one line there should be an extra space between first and second 8th of bytes
                else if(i % 8 == 0 && i != 0)
                {
                    bytes += " ";
                    ascii += " ";
                }
                //convert data to: 2 space hexa / ascii (only chars 32-126 are printable)
                bytes += $" {Data[i].ToString("X2")}";
                ascii += $"{(Data[i] >=32 && Data[i] <= 126 ? (char)Data[i] : '.')}";
            }
            //add final line as it wasn't added in the loop unless it's 16 dividable
            if(Data.Length % 16 != 0)
                _lines.Add($"0x{lineNumber.ToString("X4")}:{bytes} {ascii}");
            return _lines;
        }
    }
}
