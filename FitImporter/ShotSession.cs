namespace XeroChronoImporter
{
    public class Shot
    {
        public int ShotNumber { get; set; }

        public float Speed { get; set; }

        public DateTime Timestamp { get; set; }

        public string? Unit {  get; set; }
    }

    public class ShotSession
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        public int ShotCount => Shots.Count;

        public float AvgSpeed { get; set; }

        public float MinSpeed { get; set; }

        public float MaxSpeed { get; set; }

        public List<Shot>? Shots { get; set; }
    }
}