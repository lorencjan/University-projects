// File: Program.cs
// Solution: IPK - Project 2 - ZETA - Sniffer
// Date: 12.4.2020
// Author: Jan Lorenc (xloren15)
// Faculty: Faculty of Information Technology VUT
// Description: This file contains the program's main() starting function

using System;

namespace ipk_sniffer
{
    /// <summary>Contains only the program's main function for starting the program</summary>
    class Program
    {
        static void Main(string[] args)
        {
            //get arguments
            var arguments = new Arguments(args);
            if(!arguments.Parse())
            {
                Console.Error.WriteLine(arguments.ErrorMsg);
                return;
            }

            //sniff packets
            var sniffer = new Sniffer(arguments);
            sniffer.Sniff();
            sniffer.Out();
        }
    }
}
