namespace XeroChronoImporter.Tests
{
    public  class XeroParser
    {
        private const string TestCsvFile = "xero-import.csv";

        [Fact]
        public void Can_Process_Csv_File()
        {
            using (var fileStream = ReadTestDataFile(TestCsvFile))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    var target = new XeroCsvReader();
                    var session = target.ReadXeroCsvFile(reader);

                    Assert.NotNull(session);
                }
            }

        }

        private FileStream ReadTestDataFile(string filePath)
        {
            var path = Path.IsPathFullyQualified(filePath)
            ? filePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory() + "/TestData", filePath);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"Could not find file at path: {path}");
            }

            return File.OpenRead(path);
        }
    }
}
