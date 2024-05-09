using XeroChronoImporter;

namespace XeroToGRT.Exporters
{
    public interface ISessionCsvExporter
    {
        void Export(string path);
    }
}
