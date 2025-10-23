using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using Printer;
using xUtil.Encoding;

class Program
{
    static void Main(string[] args) {
        var encoder = new GenericEncoder(GenericEncoder.EncodingType.Base256, GenericEncoder.EncodingType.Base128);
        var temp = new byte[] { 11, 15, 255 };
        Console.WriteLine("src bytes (8bit):");
        BitPrinter.PrintBitsArray(temp);
        var res = encoder.EncodeBytes(temp);
        Console.WriteLine("dest bytes (6bit):");
        BitPrinter.PrintBitsArray(res.Item1);
        var res2 = encoder.DecodeBytes(res.Item1, res.Item2);
        Console.WriteLine("src bytes decrypted (8bit):");
        BitPrinter.PrintBitsArray(res2);
    }
}