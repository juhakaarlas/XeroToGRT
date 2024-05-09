using XeroChronoImporter;

namespace XeroToGRT.Exporters
{
    internal class MagnetoSpeedCsvExporter : ISessionCsvExporter
    {
        bool ISessionCsvExporter.Export(ShotSession session, string path)
        {
            throw new NotImplementedException();
        }
    }
}
