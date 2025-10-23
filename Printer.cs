using System;

namespace Printer {
    public static class BitPrinter {
        private static void PrintBits<T>(T value) where T : struct, IConvertible {
            ulong v = Convert.ToUInt64(value);
            int sizeInBytes = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));

            for (int i = sizeInBytes - 1; i >= 0; i--) {
                byte b = (byte)((v >> (i * 8)) & 0xFF);
                Console.Write(Convert.ToString(b, 2).PadLeft(8, '0'));
                if (i > 0) Console.Write(" ");
            }
            Console.WriteLine();
        }
        public static void PrintBitsArray<T>(IEnumerable<T> collection) where T : struct, IConvertible {
            foreach (var item in collection) {
                PrintBits(item);
            }
        }
    }
}