using Dynastream.Fit;

namespace XeroChronoImporter
{
    public class XeroParser
    {
        public static ShotSession? Process(string path)
        {
            Console.WriteLine("FIT Decoder for Garmin Xero C1 Pro");
           
            try
            {
                // Attempt to open the input file
                FileStream fileStream = new FileStream(path, FileMode.Open);
                Console.WriteLine($"Opening {path}");

                // Create our FIT Decoder
                ShotSessionDecoder fitDecoder = new ShotSessionDecoder(fileStream, ShotSessionDecoder.ShotSessionFile);

                // Decode the FIT file
                try
                {
                    Console.WriteLine("Decoding...");
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

                
                // Check the time zone offset in the Activity message.
                var timestamp = fitDecoder.FitMessages.ChronoShotSessionMesgs.FirstOrDefault().GetTimestamp().GetDateTime();
                
                Console.WriteLine($"The timestamp this file is {timestamp}");

                var sessionParser = new ShotSessionParser(fitDecoder.FitMessages);

                var session = sessionParser.ParseShotSession();

                Console.WriteLine($"Session parsed with {session.ShotCount} shots");

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
