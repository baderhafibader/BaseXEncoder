using System;
using System.Diagnostics;

namespace Printer {
    public static class BitPrinter {
        public static void PrintBits<T>(IEnumerable<T> collection, int byteSize = 8, params (TerminalColor color, int index)[] colors)
        where T : struct, IConvertible {
            colors ??= Array.Empty<(TerminalColor, int)>();
            colors = colors.OrderBy(ci => ci.index).ToArray();
            foreach (var value in collection) {
                ulong v = Convert.ToUInt64(value);
                int sizeInBytes = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
                for (int i = sizeInBytes - 1; i >= 0; i--) {
                    byte b = (byte)((v >> (i * 8)) & 0xFF);
                    var str = Convert.ToString(b, 2).PadLeft(byteSize, '0');
                    for(var j = 0; j < str.Length; j++) {
                        foreach(var ci in colors) {
                            if ((j + (sizeInBytes - i - 1) * byteSize) >= ((byteSize * sizeInBytes) - ci.index) ) {
                                Trace.Write(ci.color);
                                break;
                            }
                        }
                        Trace.Write(str[j]);
                        Trace.Write(TerminalColor.RESET);
                    }
                    if (i > 0) Trace.Write(" ");
                }
                Trace.WriteLine("");
            }
        }
    }

    public class TerminalColor {
        private int _code { get; set; }
        private TerminalColor(int colorInt) { _code = colorInt; }
        public override string ToString() => $"\u001b[{_code}m";
        public static TerminalColor BLACK { get; set; } = new TerminalColor(30);
        public static TerminalColor RED { get; set; } = new TerminalColor(31);
        public static TerminalColor GREEN { get; set; } = new TerminalColor(32);
        public static TerminalColor YELLOW { get; set; } = new TerminalColor(33);
        public static TerminalColor BLUE { get; set; } = new TerminalColor(34);
        public static TerminalColor MAGENTA { get; set; } = new TerminalColor(35);
        public static TerminalColor CYAN { get; set; } = new TerminalColor(36);
        public static TerminalColor WHITE { get; set; } = new TerminalColor(37);
        public static TerminalColor RESET { get; set; } = new TerminalColor(0);
    }
}