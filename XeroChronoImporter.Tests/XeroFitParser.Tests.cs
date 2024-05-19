namespace XeroChronoImporter.Tests
{
    public class XeroFitParserTests
    {
        private const string TestFitFile = "xero-test.fit";

        [Fact]
        public void CanReadFit()
        {
            var testee = new XeroFitParser();
            var actual = testee.Process(TestUtils.GetTestDataFilePath(TestFitFile));

            Assert.NotNull(actual);
        }
    }
}
