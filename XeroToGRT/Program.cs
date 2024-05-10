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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public string OutFile { get; set; }

            [Option('i', "inputfile", Required = true, HelpText = "The Garmin Xero C1 fit file.")]
            public string InputFile { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
            if (!File.Exists(input)) 
            {
                Console.Error.WriteLine($"Input file {input} does not exist.");
                return;
            }

            var parser = new XeroParser(verbose);
            var session = parser.Process(input);
            if (session == null) { return; }

            var exporter = new MagnetoSpeedCsvExporter();
            exporter.Init(session);
            exporter.Export(output);
        }
    }
}
