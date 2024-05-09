using XeroChronoImporter;

namespace XeroToGRT.Exporters
{
    internal interface ISessionCsvExporter
    {
        bool Export(ShotSession session, string path);
    }
}
