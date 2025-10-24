using StaticUtil;
using System;
using static xUtil.Encoding.GenericEncoder;

namespace xUtil.Encoding {
    public class BaseMaps {
        public string Map2 => "01";
        public string Binary => Map2;
        public string Map4 => "0123";
        public string Map8 => "01234567";
        public string Octal => Map8;
        public string Map16 => "0123456789ABCDEF";
        public string Hex => Map16;
        public string Map32 => "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        public string Map64 => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        private string _map128 = "";
        private string _map256 = "";

        public BaseMaps() {
            InitMap128();
            InitMap256();
            InitAllDictionaries();
        }

        private void InitMap128() {
            var sb = new System.Text.StringBuilder(128);
            for (int i = 0; i < 128; i++)
                sb.Append((char)i);
            _map128 = sb.ToString();
        }

        public string Map128 => _map128;

        private void InitMap256() {
            var sb = new System.Text.StringBuilder(256);
            for (int i = 0; i < 256; i++)
                sb.Append((char)i);
            _map256 = sb.ToString();
        }

        public string Map256 => _map256;
        public string Ascii => _map256;


        public string GetCharset(EncodingType charsetBase) {
            return charsetBase switch {
                EncodingType.Base2 => Map2,
                EncodingType.Base4 => Map4,
                EncodingType.Base8 => Map8,
                EncodingType.Base16 => Map16,
                EncodingType.Base32 => Map32,
                EncodingType.Base64 => Map64,
                EncodingType.Base128 => Map128,
                EncodingType.Base256 => Map256,
                _ => ""
            };
        }

        private readonly Dictionary<int, Dictionary<byte, char>> encodeMaps = new();
        private readonly Dictionary<int, Dictionary<char, byte>> decodeMaps = new();

        private void InitAllDictionaries() {
            var bases = new[] { 2, 4, 8, 16, 32, 64, 128, 256 };
            foreach (var b in bases) {
                var charset = GetCharset((EncodingType)b);
                var encode = new Dictionary<byte, char>();
                var decode = new Dictionary<char, byte>();
                for (int i = 0; i < charset.Length; i++) {
                    encode[(byte)i] = charset[i];
                    decode[charset[i]] = (byte)i;
                }
                encodeMaps[b] = encode;
                decodeMaps[b] = decode;
            }
        }

        public char Encode(byte value, EncodingType encodingType) => encodeMaps[(int)encodingType][value];
        public byte Decode(char c, EncodingType encodingType) => c == '=' ? (byte)0 : decodeMaps[(int)encodingType][c];

        public string Encode(byte[] values, EncodingType encodingType, int padding) {
            var sb = new System.Text.StringBuilder(values.Length);
            for(var i = 0; i < values.Length; i++) {
                var v = values[i];
                if (i >= values.Length - padding) {
                    sb.Append('=');
                } else {
                    sb.Append(Encode(v, encodingType));
                }
            }
            return sb.ToString();
        }
        public (byte[], int) Decode(string str, EncodingType encodingType) {
            var result = new List<byte>();
            var paddingCount = 0;
            for (int i = 0; i < str.Length; i++) {
                result.Add(Decode(str[i], encodingType));
                if (str[i] == '=' && paddingCount > 0) {
                    throw new NotSupportedException("Concatenated string not supported");
                }
                if (str[i] == '=') {
                    paddingCount++;
                }
            }
            return (result.ToArray(), paddingCount);
        }
    }
}