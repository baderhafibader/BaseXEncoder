using Printer;
using StaticUtil;
using System;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace xUtil.Encoding {
    public class GenericEncoder {
        public enum EncodingType {
            Base2 = 2,
            Base4 = 4,
            Base8 = 8,
            Base16 = 16,
            Base32 = 32,
            Base64 = 64,
            Base128 = 128,
            Base256 = 256,

            BIN = 2,
            BINARY = 2,
            OCT = 8,
            OCTAL = 8,
            HEX = 16,
            ASCII = 256,
        }

        protected BaseMaps _mapper { get; set; }
        public GenericEncoder() {
            _mapper = new BaseMaps();
        }

        protected (byte[], int) Execute(byte[] src, EncodingType baseFrom, EncodingType baseto, int destinationPaddingCount = 0) {
            

            var from = (int)Math.Log((double)baseFrom, 2);
            var to = (int)Math.Log((double)baseto, 2);

            var ratio = MathUtil.MinRatio(from, to);
            var paddingCount = (ratio.Item2 - (src.Length % ratio.Item2)) % ratio.Item2;
            var padding = new List<byte>();
            for (var i = 0; i < paddingCount; i++) padding.Add(0);
            var srcBytes = src.Concat(padding).ToArray();

            var destBytes = new List<byte>();
            Trace.WriteLine($"\n----------- Execute | from({from}), to({to}), destPad({destinationPaddingCount}), ratio({ratio.Item1}:{ratio.Item2}), srcPad({paddingCount}) -----------");
            Trace.WriteLine($"src byte[]:");
            Printer.BitPrinter.PrintBits(srcBytes, from);

            var buffer = 0;
            var bitCount = 0;
            var srcCounter = 0;
            var bufferChange = 0;
            var colors = new TerminalColor[] { TerminalColor.RED, TerminalColor.BLUE, TerminalColor.YELLOW, TerminalColor.GREEN};

            do {
                Trace.WriteLine($"\n----------- cycle -----------");
                Trace.WriteLine($"buffer at cycle start:");
                Printer.BitPrinter.PrintBits(
                    new int[] { buffer },
                    byteSize: from,
                    (colors[(bufferChange + 0) % (colors.Length)], bitCount - to * 0),
                    (colors[(bufferChange + 1) % (colors.Length)], bitCount - to * 1),
                    (colors[(bufferChange + 2) % (colors.Length)], bitCount - to * 2),
                    (colors[(bufferChange + 3) % (colors.Length)], bitCount - to * 3)
                );
                if (bitCount < to && srcCounter < srcBytes.Length) {
                    buffer = buffer << from;
                    buffer = buffer | srcBytes[srcCounter];
                    bitCount += from;
                    srcCounter++;
                    Trace.WriteLine($"buffer AFTER taking new bits:");
                    Printer.BitPrinter.PrintBits(
                        new int[] { buffer },
                        byteSize: from,
                        (colors[(bufferChange + 0) % (colors.Length)], bitCount - to * 0),
                        (colors[(bufferChange + 1) % (colors.Length)], bitCount - to * 1),
                        (colors[(bufferChange + 2) % (colors.Length)], bitCount - to * 2),
                        (colors[(bufferChange + 3) % (colors.Length)], bitCount - to * 3)
                    );
                }

                if (bitCount >= to) {
                    var move = bitCount - to;
                    var byteToCopy = (byte)(buffer >> move);
                    destBytes.Add(byteToCopy);
                    bitCount -= to;
                    Trace.WriteLine($"dest bytes:");
                    Printer.BitPrinter.PrintBits(
                        new int[] { MathUtil.GetIntOfLast4Bytes(destBytes.ToArray())},
                        byteSize: to,
                        (colors[ (((destBytes.Count -1 + 4 ) % colors.Length) + colors.Length) % colors.Length], to *1 *((Math.Sign(destBytes.Count - 0) + 1) / 2)),
                        (colors[ (((destBytes.Count -1 + 3 ) % colors.Length) + colors.Length) % colors.Length], to *2 *((Math.Sign(destBytes.Count - 1) + 1) / 2)),
                        (colors[ (((destBytes.Count -1 + 2 ) % colors.Length) + colors.Length) % colors.Length], to *3 *((Math.Sign(destBytes.Count - 2) + 1) / 2)),
                        (colors[ (((destBytes.Count -1 + 1 ) % colors.Length) + colors.Length) % colors.Length], to *4 *((Math.Sign(destBytes.Count - 3) + 1) / 2))

                    );
                    var mask = (1 << bitCount) - 1;
                    buffer = buffer & mask;
                    bufferChange++;
                    Trace.WriteLine($"buffer AFTER taking new bits:");
                    Printer.BitPrinter.PrintBits(
                        new int[] { buffer },
                        byteSize: from,
                        (colors[(bufferChange + 0) % (colors.Length)], bitCount - to *0),
                        (colors[(bufferChange + 1) % (colors.Length)], bitCount - to *1),
                        (colors[(bufferChange + 2) % (colors.Length)], bitCount - to *2),
                        (colors[(bufferChange + 3) % (colors.Length)], bitCount - to *3)
                    );
                }
            } while (bitCount > 0 || srcCounter < srcBytes.Length);

            destBytes = destBytes.Take(destBytes.Count() - destinationPaddingCount).ToList();
            return (destBytes.ToArray(), paddingCount);
        }

        public (byte[], int) ConvertBytes(byte[] src, EncodingType from, EncodingType to, int srcPaddingCount = 0) {

            var result = Execute(src, from, to, srcPaddingCount);
            return (result.Item1, result.Item2);
        }

        public byte[] GetBytes(string text, EncodingType encodingType) {
            var (bytes, padding) = _mapper.Decode(text, encodingType);
            return bytes;
        }

        public string GetString(byte[] bytes, EncodingType encodingType, int padding) {
            var text = _mapper.Encode(bytes, encodingType, padding);
            return text;
        }
        public string Convert(string text, EncodingType from, EncodingType to) {
            Trace.WriteLine(
                $"\n==================================================================================================================\n" +
                $"\t\t\tConvert '{text}', {from} to {to}" +
                $"\n==================================================================================================================");

            var (bytes, padding) = _mapper.Decode(text, from);
            Trace.WriteLine($"\n===========\ninput as byte[]: {bytes}, padding: {padding}");
            
            var (resBytes, resPadding) = ConvertBytes(bytes, from, to, padding);
            Trace.WriteLine($"\n===========\ninput as byte[]: {bytes}, padding: {padding}");
            
            var resString = GetString(resBytes, to, resPadding);
            Trace.WriteLine(
                $"\n==================================================================================================================\n" +
                $"\t\t\tConvert '\u001b[32m{text}\u001b[0m', {from} to {to}, result:\u001b[32m{resString}\u001b[0m" +
                $"\n==================================================================================================================\n");
            return resString;
        }
    }
}