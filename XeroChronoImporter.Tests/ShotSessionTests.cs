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
    }
}