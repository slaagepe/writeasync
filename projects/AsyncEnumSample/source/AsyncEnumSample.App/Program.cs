﻿//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace AsyncEnumSample
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            string path = "ExampleFile.txt";
            Random random = new Random();
            byte[] writtenBytes = new byte[1000000];
            for (int i = 0; i < writtenBytes.Length; ++i)
            {
                writtenBytes[i] = (byte)('A' + (i % 7));
            }

            AsyncFileWriter writer = new AsyncFileWriter(99999);
            writer.WriteAllBytesAsync(path, writtenBytes).Wait();
            Console.WriteLine("Wrote {0} bytes.", writtenBytes.Length);

            AsyncFileReader reader = new AsyncFileReader(100001);
            Task<byte[]> task = reader.ReadAllBytesAsync(path);
            byte[] readBytes = task.Result;
            Console.WriteLine("Read {0} bytes.", readBytes.Length);
            Console.WriteLine("First 30 bytes: " + Encoding.ASCII.GetString(readBytes, 0, 30));
            Console.WriteLine("Last 30 bytes: " + Encoding.ASCII.GetString(readBytes, readBytes.Length - 30, 30));

            if (writtenBytes.Length != readBytes.Length)
            {
                throw new InvalidOperationException("Lengths do not match.");
            }

            for (int i = 0; i < readBytes.Length; ++i)
            {
                if (writtenBytes[i] != readBytes[i])
                {
                    throw new InvalidOperationException("Mismatching byte values at index " + i);
                }
            }

            Console.WriteLine("All byte values match!");
        }
    }
}
