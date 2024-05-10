using XeroChronoImporter;

namespace XeroToGRT.Exporters
{
    public interface ISessionCsvExporter
    {
        void Init(ShotSession session);

        void Export(string outPath);
    }
}
