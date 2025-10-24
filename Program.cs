using Printer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using xUtil.Encoding;

class Program {
    static void Main(string[] args) {
        #if DEBUG
        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        Trace.AutoFlush = true;
        Trace.WriteLine("MODE: DEBUG");
        #endif

        var encoder = new GenericEncoder();
        var x = encoder.Convert("Hello", GenericEncoder.EncodingType.Base256, GenericEncoder.EncodingType.Base64);

        Console.WriteLine(x);
    }
}