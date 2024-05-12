namespace XeroChronoImporter
{
    public enum SpeedUnit
    {
        Mps,
        Fps,
        Kph,
        Mph
    }

    public enum WeightUnit
    {
        Grains,
        Grams
    }

    public class Shot
    {
        public int ShotNumber { get; set; }

        public double Speed { get; set; }

        public DateTime Timestamp { get; set; }

        public TimeOnly Time {  get; set; }

        public string? Unit {  get; set; }

        public bool ColdBore { get; set; }

        public bool CleanBore { get; set; }

        public string Notes {  get; set; }
    }

    public class ShotSession
    {
        private const double InvalidSpeed = -1;

        public const int RoundingPrecision = 1;

        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        public int? ShotCount => Shots?.Count;

        private double _avgSpeed = InvalidSpeed;

        public string? SpeedUnit => Shots?.FirstOrDefault()?.Unit;

        public string CartridgeType { get; set; }

        public double Weight { get; set; }

        public WeightUnit WeightUnit { get; set; }

        public double AvgSpeed {
            get
            {
                if (_avgSpeed == InvalidSpeed)
                {
                    _avgSpeed = CalculateAvgSpeed();
                }

                return _avgSpeed;
            }
        }

        private double _stdDev = InvalidSpeed;

        public double StdDev
        {
            get
            {
                if (_stdDev == InvalidSpeed)
                {
                    _stdDev = CalculateStdDev();
                }
                return _stdDev;
            }
        }

        private double _minSpeed = InvalidSpeed;

        public double MinSpeed
        {
            get
            {
                if (_minSpeed == InvalidSpeed)
                {
                    if (Shots == null) { return InvalidSpeed; }

                    _minSpeed = Shots.Min(s =>  s.Speed);
                }

                return _minSpeed;
            }
        }

        private double _maxSpeed = InvalidSpeed;

        public double MaxSpeed
        {
            get
            {
                if (_maxSpeed == InvalidSpeed)
                {
                    if (Shots == null) { return InvalidSpeed; }
                    _maxSpeed = Shots.Max(s => s.Speed);
                }

                return _maxSpeed;
            }
        }

        public double ExtremeSpread => Math.Round(MaxSpeed - MinSpeed, RoundingPrecision);

        public List<Shot>? Shots { get; set; }

        private double CalculateAvgSpeed()
        {
            if (Shots == null) { return InvalidSpeed; }

            return  Math.Round(Shots.Average(s => s.Speed), RoundingPrecision);
        }

        private double CalculateStdDev()
        {
            if (Shots == null) { return InvalidSpeed; }

            return Math.Round(Math.Sqrt(Shots.Average(s => Math.Pow(s.Speed - AvgSpeed, 2))), RoundingPrecision);
        }
    }
}
