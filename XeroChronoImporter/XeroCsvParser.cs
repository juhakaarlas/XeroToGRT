namespace XeroChronoImporter
{
    public class XeroCsvParser : IXeroParser
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
                SessionAvgSpeed, 
                SessionStdDev, 
                SessionSpread, 
                SessionAvgPowerFactor, 
                SessionProjectileWeight, 
                SessionNote
            ];

        public const int CsvShotFieldCount = 9;

        public bool Verbose { get; set; }

        private ILogger _logger;

        public XeroCsvParser(ILogger logger)
        {
            _logger = logger;
        }

        public ShotSession? Process(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            LogMessage("Processing CSV file...");

            using (var file = File.OpenRead(path))
            {
                using (var reader = new StreamReader(file))
                {
                    return ReadXeroCsvFile(reader);
                }
            }
        }

        public ShotSession? ReadXeroCsvFile(StreamReader reader)
        {
            LogMessage("Processing CSV header...");
            var session = ReadSessionHeader(reader);

            if (session == null) return null;
            
            SpeedUnit speedUnit = GetSpeedUnitFromShotsHeader(reader) ?? SpeedUnit.Mps;
            LogMessage("Processing shot data...");
            List<Shot> shots = ReadShots(reader, speedUnit);
            session.Shots = shots;
            session = ReadSessionData(reader, session);

            return session;
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

                if (date[0] != SessionDate) 
                {
                    throw new FormatException($"Session data fields are not in expected order. Expected {sessionDate} but read {date[0]}");
                }

                (var datePart, var timePart) = ParseDateTimeString(date[1]);

                session.StartTime = new DateTime(datePart, timePart);
            }

            session.Weight = double.Parse(sessionData[SessionProjectileWeight]);
            session.WeightUnit = WeightUnit.Grains;
            session.Note = sessionData[SessionNote].Replace("\"", string.Empty);
            return session;
        }

        public List<Shot> ReadShots(StreamReader reader, SpeedUnit speedUnit)
        {
            var shots = new List<Shot>();

            while (reader.Peek() != '-')
            {
                var shotCsv = reader.ReadLine();

                if (string.IsNullOrEmpty(shotCsv)) continue;

                LogMessage(shotCsv);

                string[]? shotData = shotCsv.Split(',');

                if (shotData == null || shotData.Length == 0) continue;


                var shotTime = shotData[5].Replace('.', ':');
                string cleanBoreRawValue = shotData[6].Trim();
                string coldBoreRawValue = shotData[7].Trim();
                bool cleanBore = string.IsNullOrEmpty(cleanBoreRawValue) ? false : true;
                bool coldBore = string.IsNullOrEmpty(coldBoreRawValue) ? false : true;

                var shot = new Shot()
                {
                    Unit = speedUnit,
                    ShotNumber = int.Parse(shotData[0]),
                    Speed = double.Parse(shotData[1]),
                    Time = TimeOnly.Parse(shotTime),
                    CleanBore = cleanBore,
                    ColdBore = coldBore,
                    Notes = shotData[8]
                };

                shots.Add(shot);
            }

            //read past the separator line
            if (reader.Peek() == '-') reader.ReadLine();

            return shots;
        }

        public SpeedUnit? GetSpeedUnitFromShotsHeader(StreamReader reader)
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

        public static SpeedUnit ConvertSpeedUnit(string unit)
        {
            return EnumExtensions.GetValueFromDescription<SpeedUnit>(unit);
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

        private void LogMessage(string message)
        {
            if (!Verbose || _logger == null) return;

            _logger.Log(message);
        }
    }
}
