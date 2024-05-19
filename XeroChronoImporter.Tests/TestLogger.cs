namespace XeroChronoImporter.Tests
{
    internal class TestLogger : ILogger
    {
        public ICollection<string> Messages { get;  }

        public TestLogger()
        {
            Messages = new List<string>();
        }

        public void Log(string message)
        {
            Messages.Add(message);
        }
    }
}
