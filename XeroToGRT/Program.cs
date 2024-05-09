using XeroChronoImporter;

namespace XeroToGRT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parser.Process(args[0]);
        }
    }
}
