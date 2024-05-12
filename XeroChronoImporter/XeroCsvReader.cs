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

        public ShotSession? ReadXeroCsvFile(StreamReader reader)
        {
            var session = ReadSessionHeader(reader);

            if (session == null) return null;

            if (session == null) return null;

            string speedUnit = GetSpeedUnitFromShotsHeader(reader) ?? "m/s";
            List<Shot> shots = ReadShots(reader, speedUnit);
            session.Shots = shots;
            session = ReadSessionData(reader, session);

            return session;
        }

        public ShotSession? ReadXeroCsvFile(string filename)
        {
            using (var reader = File.OpenText(filename)) 
            {
                return ReadXeroCsvFile(reader);
            }
        }

        public static (DateOnly date, TimeOnly time) ParseDateTimeString(string xeroSessionDate)
        {
            string[] date = xeroSessionDate.Split("at");
            string datePart = date[0].Trim().Replace("\"", string.Empty);
            string timePart = date[1].Trim().Replace("\"", string.Empty);

            return (DateOnly.Parse(datePart), TimeOnly.ParseExact(timePart, "HH.mm"));
        }

        public ShotSession ReadSessionData(StreamReader reader, ShotSession session)
        {
            int pos = 0;

            Dictionary<string, string> sessionData = new Dictionary<string, string>();

            while (reader.Peek() != '-')
            {
                var line = reader.ReadLine();

                if (string.IsNullOrEmpty(line)) continue;
                
                var values = line.Split(',');

                var expected = _sessionDataExpectedLineOrder[pos];

                // Session note may contain a quoted value with a comma as part of the note
                if (expected == SessionNote)
                {
                    values = line.Split(',', 2);

                    if (values[1].StartsWith("\""))
                    {
                        int endQuote = values[1].LastIndexOf('"');
                        values[1] = values[1].Substring(0, endQuote);
                    }
                }

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
                string[] date = sessionDate.Split(',');

                (var datePart, var timePart) = ParseDateTimeString(date[1]);

                session.StartTime = new DateTime(datePart, timePart);
            }

            session.Weight = double.Parse(sessionData[SessionProjectileWeight]);
            session.WeightUnit = WeightUnit.Grains;
            session.Note = sessionData[SessionNote].Replace("\"", string.Empty);
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


                var shotTime = shotData[5].Replace('.', ':');
                string cleanBore = string.IsNullOrEmpty(shotData[6].Trim()) ? "false": shotData[6];
                string coldBore = string.IsNullOrEmpty(shotData[7].Trim()) ? "false" : shotData[6];

                var shot = new Shot()
                {
                    Unit = speedUnit,
                    ShotNumber = int.Parse(shotData[0]),
                    Speed = double.Parse(shotData[1]),
                    Time = TimeOnly.Parse(shotTime),
                    CleanBore = bool.Parse(cleanBore),
                    ColdBore = bool.Parse(coldBore),
                    Notes = shotData[8]
                };

                shots.Add(shot);
            }

            //read past the separator line
            if (reader.Peek() == '-') reader.ReadLine();

            return shots;
        }

        public string? GetSpeedUnitFromShotsHeader(StreamReader reader)
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

            return ConvertSpeedUnit(speedUnit);
        }

        public static string ConvertSpeedUnit(string unit)
        {
            switch (unit)
            {
                case "MPS": return "m/s";
                default:
                    return unit;
            }
        }

        public ShotSession? ReadSessionHeader(StreamReader reader)
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
