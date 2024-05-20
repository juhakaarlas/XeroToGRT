namespace XeroChronoImporter.Tests
{
    public class ShotSessionDecoderTests
    {
        [Fact]
        public void It_Throws_When_Filetype_Is_Unexpected()
        {
            var testStream = TestUtils.GenerateStreamFromString("bogus");
            var testee = new ShotSessionDecoder(testStream, ShotSessionDecoder.ShotSessionFile);

            Assert.Throws<FileTypeException>(() => testee.Decode());
        }
    }
}
