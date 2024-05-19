namespace XeroChronoImporter.Tests
{
    public class TestUtils
    {
        public static string GetTestDataFilePath(string fileName)
        {
            return Path.Combine(Directory.GetCurrentDirectory() + "/TestData", fileName);
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static FileStream GetTestDataFileStream(string fileName)
        {
            var filePath = GetTestDataFilePath(fileName);

            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"Could not find file at path: {filePath}");
            }

            return File.OpenRead(filePath);
        }
    }
}
