using StaticUtil;
using System;
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
        }

        protected BaseMaps _mapper { get; set; }
        public GenericEncoder() {
            _mapper = new BaseMaps();
        }

        protected (byte[], int) Execute(byte[] src, int from, int to, int destinationPaddingCount = 0) {
            var ratio = MathUtil.MinRatio(from, to);
            var paddingCount = (ratio.Item2 - (src.Length % ratio.Item2)) % ratio.Item2;
            var padding = new List<byte>();
            for (var i = 0; i < paddingCount; i++) padding.Add(0);
            var srcBytes = src.Concat(padding).ToArray();

            var destBytes = new List<byte>();

            var buffer = 0;
            var bitCount = 0;
            var srcCounter = 0;

            do {
                if (bitCount <= to && srcCounter < srcBytes.Length) {
                    buffer = buffer << from;
                    buffer = buffer | srcBytes[srcCounter];
                    bitCount += from;
                    srcCounter++;
                }

                if (bitCount >= to) {
                    var move = bitCount - to;
                    destBytes.Add((byte)(buffer >> move));
                    bitCount -= to;
                    var mask = (1 << bitCount) - 1;
                    buffer = buffer & mask;
                }

            } while (bitCount > 0 || srcCounter < srcBytes.Length);

            destBytes = destBytes.Take(destBytes.Count() - destinationPaddingCount).ToList();
            return (destBytes.ToArray(), paddingCount);
        }

        public (byte[], int) ConvertBytes(byte[] src, EncodingType from, EncodingType to, int srcPaddingCount = 0) {
            var baseFrom = (int)Math.Log((double)from, 2);
            var baseTo = (int)Math.Log((double)to, 2);
            var result = Execute(src, baseFrom, baseTo, srcPaddingCount);
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
            var (bytes, padding) = _mapper.Decode(text, from);
            var (resBytes, resPadding) = ConvertBytes(bytes, from, to, padding);
            var resString = GetString(resBytes, to, resPadding);
            return resString;
        }
    }
}