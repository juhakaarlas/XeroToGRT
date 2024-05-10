using System.Diagnostics.CodeAnalysis;
using XeroChronoImporter;
using XeroToGRT.Exporters;
using CommandLine;

namespace XeroToGRT
{
    internal class Program
    {
        public class Options
        {
            [Option('v', "verbose", Required = false, Default = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('o', "outfile", Required = true, HelpText = "The output CSV file path.")]
            public string OutFile { get; set; }

            [Option('i', "inputfile", Required = true, HelpText = "The Garmin Xero C1 fit file.")]
            public string InputFile { get; set; }
        }

        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Options))]
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    Process(o.InputFile, o.OutFile, o.Verbose);
                });
        }

        static void Process(string input, string output, bool verbose)
        {
            var session = XeroParser.Process(input, verbose);
            if (session == null) { return; }

            var exporter = new MagnetoSpeedCsvExporter();
            exporter.Init(session);
            exporter.Export(output);
        }
    }
}
