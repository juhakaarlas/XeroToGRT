namespace XeroChronoImporter.Tests
{
    public class ShotSessionTests
    {
        private List<Shot> _testShots = new List<Shot> ()
        { 
            new Shot ()
            {
                Speed = 1,
                Unit = SpeedUnit.Mps
            },
            new Shot ()
            {
                Speed = 2
            },
            new Shot ()
            {
                Speed = 3
            }
        };


        [Fact]
        public void AvgSpeed_Returns_Correct_Value()
        {
            var target = new ShotSession() {  Shots = _testShots };
            var expectedAvg = _testShots.Average<Shot>(s => s.Speed);
            expectedAvg = Math.Round(expectedAvg, ShotSession.RoundingPrecision);

            Assert.Equal(expectedAvg, target.AvgSpeed);
        }

        [Fact]
        public void ShotCount_Returns_Correct_Value()
        {
            var target = new ShotSession()
            {
                Shots = _testShots
            };

            Assert.Equal(3, target.ShotCount);
        }

        [Fact]
        public void SpeedUnit_Returns_Correct_Value()
        {
            var target = new ShotSession()
            {
                Shots = _testShots
            };

            Assert.Equal(SpeedUnit.Mps, target.SpeedUnit);
        }

        [Fact]
        public void MaxSpeed_Returns_Correct_Value()
        {
            var target = new ShotSession()
            {
                Shots = _testShots
            };

            Assert.Equal(3, target.MaxSpeed);
        }

        [Fact]
        public void MinSpeed_Returns_Correct_Value()
        {
            var target = new ShotSession()
            {
                Shots = _testShots
            };

            Assert.Equal(1, target.MinSpeed);
        }

        [Fact]
        public void ExtremeSpread_Returns_Correct_Value()
        {
            var target = new ShotSession()
            {
                Shots = _testShots
            };

            Assert.Equal(2, target.ExtremeSpread);
            
        }

        [Fact]
        public void StdDev_Returns_Correct_Value()
        {
            var target = new ShotSession()
            {
                Shots = _testShots
            };

            double expected = StandardDeviation(_testShots.Select(s => s.Speed).ToList());
            expected = Math.Round(expected, ShotSession.RoundingPrecision);
            Assert.Equal(expected, target.StdDev);
        }

        private double StandardDeviation(ICollection<double> values)
        {
            double average = values.Average();
            double sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / values.Count);
            return sd;
        }
    }
}
