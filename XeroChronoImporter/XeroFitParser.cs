using Dynastream.Fit;

namespace XeroChronoImporter
{
    public class XeroFitParser : IXeroParser
    {
        public bool Verbose {  get; set; }

        private ILogger? _logger;

        public XeroFitParser(ILogger logger)
        {
            Verbose = false;
            _logger = logger;
        }

        public ShotSession? Process(string path)
        {
            LogOutput("FIT Decoder for Garmin Xero C1 Pro");
            return ProcessFitFile(path);
        }

        private ShotSession? ProcessFitFile(string path)
        {
            // Attempt to open the input file
            FileStream fileStream = new FileStream(path, FileMode.Open);

            LogOutput($"Opening {path}");

            // Create our FIT Decoder
            ShotSessionDecoder fitDecoder = new ShotSessionDecoder(fileStream, ShotSessionDecoder.ShotSessionFile);
            fitDecoder.Verbose = Verbose;

            // Decode the FIT file
            try
            {
                LogOutput($"Decoding {path}...");
                fitDecoder.Decode();
            }
            finally
            {
                fileStream.Close();
            }

            var sessionParser = new ShotSessionParser(fitDecoder.FitMessages);
            var session = sessionParser.ParseShotSession();

            LogOutput($"Session parsed with {session.ShotCount} shots");

            return session;
        }

        private void LogOutput(string message)
        {
            if (!Verbose || _logger == null) return;

            _logger.Log(message);
        }
    }
}
