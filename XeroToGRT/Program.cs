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

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    var session = XeroParser.Process(o.InputFile, o.Verbose);
                    if (session == null) { return; }
                    var exporter = new MagnetoSpeedCsvExporter(session);
                    exporter.Export(o.OutFile);
                });
        }
    }
}
