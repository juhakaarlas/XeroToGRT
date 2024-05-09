using Dynastream.Fit;

namespace XeroChronoImporter
{
    public class XeroParser
    {
        public static ShotSession? Process(string path, bool verbose)
        {
            if (verbose)
            {
                Console.WriteLine("FIT Decoder for Garmin Xero C1 Pro");
            }
           
            try
            {
                // Attempt to open the input file
                FileStream fileStream = new FileStream(path, FileMode.Open);
                
                if (verbose)
                {
                    Console.WriteLine($"Opening {path}");
                }

                // Create our FIT Decoder
                ShotSessionDecoder fitDecoder = new ShotSessionDecoder(fileStream, ShotSessionDecoder.ShotSessionFile);
                fitDecoder.Verbose = verbose;

                // Decode the FIT file
                try
                {
                    if (verbose)
                    {
                        Console.WriteLine($"Decoding {path}...");
                    }
                    
                    fitDecoder.Decode();
                }
                catch (FileTypeException ex)
                {
                    Console.WriteLine("Decoder caught FileTypeException: " + ex.Message);
                    return null;
                }
                catch (FitException ex)
                {
                    Console.WriteLine("Decoder caught FitException: " + ex.Message);
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Decoder caught Exception: " + ex.Message);
                    return null;
                }
                finally
                {
                    fileStream.Close();
                }
                
                if (verbose)
                {
                    var timestamp = fitDecoder.FitMessages.ChronoShotSessionMesgs.FirstOrDefault().GetTimestamp().GetDateTime();
                    Console.WriteLine($"The timestamp in this file is {timestamp}");
                }

                var sessionParser = new ShotSessionParser(fitDecoder.FitMessages);
                var session = sessionParser.ParseShotSession();

                if (verbose)
                {
                    Console.WriteLine($"Session parsed with {session.ShotCount} shots");
                }

                return session;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex}");
            }
            return null;
        }
    }
}
