namespace XeroChronoImporter.Tests
{
    public class XeroFitParserTests
    {
        private const string TestFitFile = "xero-test.fit";

        [Fact]
        public void CanReadFit()
        {
            var testee = new XeroFitParser(new ConsoleLogger());
            var actual = testee.Process(TestUtils.GetTestDataFilePath(TestFitFile));

            Assert.NotNull(actual);
            Assert.Equal(7, actual.ShotCount);
            Assert.Equal(SpeedUnit.Mps, actual.SpeedUnit);
            Assert.Equal(WeightUnit.Grains, actual.WeightUnit);
        }

        [Fact]
        public void Logs_Nothing_When_Not_Verbose()
        {
            var logger = new TestLogger();
            var testee = new XeroFitParser(logger);

            testee.Process(TestUtils.GetTestDataFilePath(TestFitFile));

            Assert.Empty(logger.Messages);
        }

        [Fact]
        public void Logs_When_Verbose()
        {
            var logger = new TestLogger();
            var testee = new XeroFitParser(logger);
            testee.Verbose = true;

            testee.Process(TestUtils.GetTestDataFilePath(TestFitFile));

            Assert.NotEmpty(logger.Messages);
            Assert.True(logger.Messages.Contains("FIT Decoder for Garmin Xero C1 Pro"));
        }
    }
}
