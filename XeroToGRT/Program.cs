using XeroChronoImporter;
using XeroToGRT.Exporters;

namespace XeroToGRT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var session = Parser.Process(args[0]);

            if (session == null) { return; }

            var exporter = new MagnetoSpeedCsvExporter(session);
            exporter.Export(args[1]);
        }
    }
}
