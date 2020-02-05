using System;
using System.Collections.Generic;
using System.IO;

namespace GSTExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            // if there were no args present, display the syntax
            if (args.Length == 0)
            {
                Console.WriteLine("GSTExtract v0.1 by shibboleet");
                Console.WriteLine("Syntax: GSTExtract in.gst <out.txt>");
                return;
            }

            string fileName = args[0];
            string outFile = "";

            // the user gave an input file but not an output file name
            if (args.Length == 1)
                outFile = fileName + ".txt";
            else
                outFile = args[1];

            // ensure our file exists
            if (!File.Exists(fileName))
            {
                Console.WriteLine("Error: input file not found.");
                return;
            }

            // create our reader
            EndianBinaryReader reader = new EndianBinaryReader(fileName);

            // create our output list
            List<string> output = new List<string>()
            {
                $"File generated from {Path.GetFileName(fileName)}\n"
            };

            // there is no clear indication of the length inside of the file, so we just makes sure that we don't
            // go out of bounds by checking if we are at the end of the file
            while (!reader.IsEOF())
            {
                // we don't need to save this data since everything we have is already being put in the string
                new GhostPacket(ref reader, ref output);
            }

            File.WriteAllLines(outFile, output);
        }
    }
}
