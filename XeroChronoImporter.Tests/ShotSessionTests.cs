using XeroChronoImporter;

namespace XeroChronoImporter.Tests
{
    public class ShotSessionTests
    {
        private List<Shot> _testShots = new List<Shot> ()
        { 
            new Shot ()
            {
                Speed = 1,
                Unit = "m/s"
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

            Assert.Equal("m/s", target.SpeedUnit);
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
    }
}