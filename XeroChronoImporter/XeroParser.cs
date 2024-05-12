using Dynastream.Fit;

namespace XeroChronoImporter
{
    public class XeroParser
    {
        public bool Verbose {  get; set; }

        public XeroParser()
        {
            Verbose = false;
        }

        public XeroParser(bool verbose) { 
            Verbose = verbose;
        }

        public ShotSession? Process(string path)
        {
            LogOutput("FIT Decoder for Garmin Xero C1 Pro");
           
            try
            {
                return ProcessFitFile(path);

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception {ex}");
            }
            return null;
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
            catch (FileTypeException ex)
            {
                Console.Error.WriteLine("Decoder caught FileTypeException: " + ex.Message);
                return null;
            }
            catch (FitException ex)
            {
                Console.Error.WriteLine("Decoder caught FitException: " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Decoder caught Exception: " + ex.Message);
                return null;
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
            if (!Verbose) return;

            Console.WriteLine(message);
        }
    }
}
