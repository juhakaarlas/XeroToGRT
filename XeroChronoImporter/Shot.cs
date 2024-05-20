using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XeroChronoImporter
{
    public class Shot
    {
        public int ShotNumber { get; set; }

        public double Speed { get; set; }

        public DateTime Timestamp { get; set; }

        public TimeOnly Time { get; set; }

        public SpeedUnit Unit { get; set; }

        public bool ColdBore { get; set; }

        public bool CleanBore { get; set; }

        public string? Notes { get; set; }
    }
}
