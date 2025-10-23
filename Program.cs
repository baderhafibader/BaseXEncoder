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

class Program
{
    static Dictionary<byte, char> Map64 { get; set; } = new Dictionary<byte, char> { };
    private static byte FillDict(byte key, int start, int end = 255) {
        if (end == 255) end = start;
        int offset = start - key;
        while (key <= end - offset)
        {
            Map64.Add(key, (char)(key + offset));
            key++;
        }
        return key;
    }
    private static void InitDict()
    {
        byte key = 0;
        key = FillDict(key, (byte)'A', (byte)'Z');
        key = FillDict(key, (byte)'a', (byte)'z');
        key = FillDict(key, (byte)'0', (byte)'9');
        key = FillDict(key, (byte)'+');
        key = FillDict(key, (byte)'/');
    }

    static int decodedBitGroupSize = 8; //(from base 256)
    static int encodedBitGroupSize = 6; //(to base 64)

    static byte[] Decode(byte[] src, int paddingCount)
    {
        var destBytes = new List<byte>();
        var buffer = 0;
        var bitCount = 0;
        var srcCounter = 0;

        do
        {
            // Console.WriteLine("\n==========================\nDecode(): src: " + src.Length + "  pc: " + paddingCount);
            // Console.WriteLine("buffer start cycle: "); PrintBitsArray(new int[] { buffer });
            if (bitCount <= decodedBitGroupSize && srcCounter < src.Length)
            {
                buffer = buffer << encodedBitGroupSize;
                buffer = buffer | src[srcCounter];
                bitCount += encodedBitGroupSize;
                srcCounter++;
            }
            // Console.WriteLine("buffer after read src: "); PrintBitsArray(new int[] { buffer });

            if( bitCount >=8) {
                var copySize = decodedBitGroupSize;
                var move = bitCount - copySize;
                // Console.WriteLine("buffer wite dest data: "); PrintBitsArray(new int[] { (byte)(buffer >> move) });
                destBytes.Add((byte)(buffer >> move));
                bitCount -= copySize;
                var mask = (1 << bitCount) - 1;
                buffer = buffer & mask;
                // Console.WriteLine("buffer after write dest: "); PrintBitsArray(new int[] { buffer });
            }

        } while (bitCount > 0 || srcCounter < src.Length);

        destBytes = destBytes.Take(destBytes.Count() - paddingCount).ToList();
        // Console.WriteLine("\nDecode exit\n==========================\n");
        return destBytes.ToArray();
    }

    static (byte[], int) Encode(byte[] src)
    {
        var paddingCount = (3 - (src.Length % 3) ) % 3;
        var padding = new List<byte>();
        for (var i = 0; i < paddingCount; i++) padding.Add(0);
        var srcBytes = src.Concat(padding).ToArray();

        var destBytes = new List<byte>();
        var buffer = 0;
        var bitCount = 0;
        var srcCounter = 0;
        do {

            if(bitCount <= encodedBitGroupSize && srcCounter < srcBytes.Length) {
                buffer = buffer << decodedBitGroupSize;
                buffer = buffer | srcBytes[srcCounter];
                bitCount += decodedBitGroupSize;
                srcCounter++;
            }

            var copySize = Math.Min(bitCount, encodedBitGroupSize); //size of next copy
            var move = bitCount - copySize; // size of next skip/shift
            destBytes.Add((byte)(buffer >> move)); 
            bitCount -= copySize;
            var mask = (1 << bitCount) - 1;
            buffer = buffer & mask;
        } while (bitCount > 0);

        return (destBytes.ToArray(), paddingCount);
    }

    static void PrintBits<T>(T value) where T : struct, IConvertible
    {
        ulong v = Convert.ToUInt64(value);
        int sizeInBytes = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));

        for (int i = sizeInBytes - 1; i >= 0; i--)
        {
            byte b = (byte)((v >> (i * 8)) & 0xFF);
            Console.Write(Convert.ToString(b, 2).PadLeft(8, '0'));
            if (i > 0) Console.Write(" ");
        }
        Console.WriteLine();
    }
    static void PrintBitsArray<T>(IEnumerable<T> collection) where T : struct, IConvertible
    {
        foreach (var item in collection)
        {
            PrintBits(item);
        }
    }

    static void Main(string[] args)
    {
        InitDict();
        var temp = new byte[] { 11, 15 };
        Console.WriteLine("src bytes (8bit):");
        PrintBitsArray(temp);
        var res = Encode(temp);
        Console.WriteLine("dest bytes (6bit):");
        PrintBitsArray(res.Item1);
        var res2 = Decode(res.Item1, res.Item2);
        Console.WriteLine("src bytes decrypted (8bit):");
        PrintBitsArray(res2);
    }
}