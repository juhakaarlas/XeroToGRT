namespace XeroChronoImporter.Tests
{
    public class EnumExtensionsTests
    {
        private enum NoDesc
        {
            NoDesc
        }

        [Theory]
        [InlineData(SpeedUnit.Mps, "m/s")]
        [InlineData(SpeedUnit.Fps, "fps")]
        [InlineData(SpeedUnit.Kph, "km/h")]
        [InlineData(SpeedUnit.Mph, "mph")]
        [InlineData(NoDesc.NoDesc, null)]
        public void GetDescription_Returns_Correct_Value(Enum e, string expected)
        {
            Assert.Equal(expected, e.GetDescription());
        }
    }
}
