using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace XeroChronoImporter
{

    public class XeroCsvReader
    {
        private const string SessionAvgSpeed = "AVERAGE SPEED";
        private const string SessionStdDev = "STD DEV";
        private const string SessionSpread = "SPREAD";
        private const string SessionAvgPowerFactor = "AVERAGE POWER FACTOR";
        private const string SessionProjectileWeight = "PROJECTILE WEIGHT(GRAINS)";
        private const string SessionNote = "SESSION NOTE";
        private const string SessionDate = "DATE";

        private readonly string[] _sessionDataExpectedLineOrder =
            [
                SessionAvgSpeed, SessionStdDev, SessionSpread, SessionAvgPowerFactor, SessionProjectileWeight, SessionNote
            ];

        public const int CsvShotFieldCount = 9;

        public bool Verbose { get; set; }

        public ShotSession? ReadXeroCsvFile(string filename)
        {
            using (var reader = File.OpenText(filename)) 
            {
                var session = ReadSessionHeader(reader);

                if (session == null) return null;

                if (session == null) return null;

                string speedUnit = ReadShotsHeader(reader) ?? "m/s";

                List<Shot> shots = ReadShots(reader, speedUnit);

                session.Shots = shots;

                session = ReadSessionData(reader, session);

                return session;

            }
        }

        private ShotSession ReadSessionData(StreamReader reader, ShotSession session)
        {
            int pos = 0;

            Dictionary<string, string> sessionData = new Dictionary<string, string>();

            while (reader.Peek() != '-')
            {
                var line = reader.ReadLine();

                if (string.IsNullOrEmpty(line)) continue;
                var values = line.Split(',');

                var expected = _sessionDataExpectedLineOrder[pos];

                if (!values[0].Equals(expected))
                {
                    throw new FormatException($"Session data fields are not in expected order. Expected {expected} but read {values[0]}");
                }

                sessionData[values[0]] = values[1];
                pos++;
            }

            // skip the separator and read the session date
            reader.ReadLine();
            string? sessionDate = reader.ReadLine();

            if (!string.IsNullOrEmpty(sessionDate)) 
            {
                session.StartTime = DateTime.Parse(sessionDate);
            }
            
            return session;

        }

        private List<Shot> ReadShots(StreamReader reader, string speedUnit)
        {
            var shots = new List<Shot>();

            while (reader.Peek() != '-')
            {
                var shotCsv = reader.ReadLine();

                if (string.IsNullOrEmpty(shotCsv)) continue;

                string[]? shotData = shotCsv.Split(',');

                if (shotData == null || shotData.Length == 0) continue;

                var shot = new Shot()
                {
                    Unit = speedUnit,
                    ShotNumber = int.Parse(shotData[0]),
                    Speed = double.Parse(shotData[1]),
                    Time = TimeOnly.Parse(shotData[5]),
                    CleanBore = bool.Parse(shotData[6]),
                    ColdBore = bool.Parse(shotData[7]),
                    Notes = shotData[8]
                };

                shots.Add(shot);
            }

            //read past the separator line
            if (reader.Peek() == '-') reader.ReadLine();

            return shots;
        }

        private string? ReadShotsHeader(StreamReader reader)
        {
            if (reader.Peek() != '#') return null;

            string header = reader.ReadLine() ?? string.Empty; 

            if (string.IsNullOrEmpty(header)) { return null; }

            string[] fields = header.Split(',');

            //speed should be at position 2
            if (fields.Length != CsvShotFieldCount || !fields[1].StartsWith("SPEED")) 
            {
                throw new FormatException("The CSV file has an unexpected shot list header.");
            }

            string speedUnit = fields[1].Substring(fields[1].IndexOf("(") + 1, 3);

            return speedUnit;
        }

        private ShotSession? ReadSessionHeader(TextReader reader)
        {
            string header = reader.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(header)) return null;

            //trim optional double quotes from header
            if (header.StartsWith('"'))
            {
                header = header.Substring(1, header.Length-2);
            }

            //extract the cartridge type which is the firt value before a comma
            string cart = header.Split(',')[0];

            var session = new ShotSession()
            {
                CartridgeType = cart
            };

            return session;
        }
    }
}
