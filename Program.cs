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

class Program {
    static void Main(string[] args) {
        var encoder = new GenericEncoder();
        var temp = new byte[] { 11, 15, 255 };
        var decoded64 = encoder.Convert("Hello", GenericEncoder.EncodingType.Base256, GenericEncoder.EncodingType.Base64);
        Console.WriteLine(decoded64);
    }
}