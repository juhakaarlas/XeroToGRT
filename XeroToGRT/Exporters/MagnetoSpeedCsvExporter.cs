using System.Text;
using XeroChronoImporter;

namespace XeroToGRT.Exporters
{
    public class MagnetoSpeedCsvExporter : ISessionCsvExporter
    {
        private const string SeriesFormat = "Series,1,Shots,{0}";
        private const string MinMaxFormat = "Min,{0},Max,{1}";
        private const string StatsFormat = "Avg,{0},S-D,{1}";
        private const string SpreadFormat = "ES,{0},,";
        private const string SeriesEnd = ",,,";
        private const string EOF = "----,----,----,----";
        private const string SeriesHeader = "Series,Shot,Speed,";
        private const string ShotFormat = "1,{0},{1},{2}";

        private ShotSession? _session;

        public void Init(ShotSession session)
        {
            _session = session;
        }

        public void Export(string outPath)
        {
            var sb = CreateSessionCsv();
            sb = CreateSeriesCsv(sb);
            sb.AppendLine(EOF);

            File.WriteAllText(outPath, sb.ToString());
        }

        private StringBuilder CreateSessionCsv() 
        {
            StringBuilder sb = new StringBuilder();

            if (_session == null) { return sb; }

            sb.AppendFormat(SeriesFormat, _session.ShotCount);
            sb.AppendLine();
            sb.AppendFormat(MinMaxFormat, _session.MinSpeed, _session.MaxSpeed);
            sb.AppendLine();
            sb.AppendFormat(StatsFormat, _session.AvgSpeed, _session.StdDev);
            sb.AppendLine();
            sb.AppendFormat(SpreadFormat, _session.ExtremeSpread);
            sb.AppendLine();
            sb.AppendLine(SeriesEnd);

            return sb;
        }

        private StringBuilder CreateSeriesCsv(StringBuilder sb)
        {
            sb.AppendLine(SeriesHeader);

            if (_session == null || _session.Shots == null) { return  sb; }

            foreach (var shot in _session.Shots)
            {
                sb.AppendFormat(ShotFormat, shot.ShotNumber, shot.Speed, shot.Unit);
                sb.AppendLine();
            }

            return sb;
        }
    }
}
