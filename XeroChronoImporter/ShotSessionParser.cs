using Dynastream.Fit;
using File = Dynastream.Fit.File;

namespace XeroChronoImporter
{
    public class ShotSessionParser
    {
        private const int SpeedRoundingPrecision = 1;

        private readonly FitMessages _messages;

        public bool IsShotSessionFile
        {
            get
            {
                return FirstFileIdMesg != null && (FirstFileIdMesg.GetType() ?? File.Invalid) == ShotSessionDecoder.ShotSessionFile;
            }
        }

        private FileIdMesg? FirstFileIdMesg => _messages.FileIdMesgs.FirstOrDefault();

        public ShotSessionParser(FitMessages messages)
        {
            _messages = messages;
        }

        public ShotSession ParseShotSession()
        {
            if (!IsShotSessionFile)
            {
                throw new Exception($"Expected FIT File Type: ShotSession, received File Type: {FirstFileIdMesg?.GetType()}");
            }

            var fitSession = _messages.ChronoShotSessionMesgs.FirstOrDefault();

            if (fitSession == null) { throw new ArgumentNullException(nameof(fitSession)); }

            var shots = _messages.ChronoShotDataMesgs;

            var shotSession = new ShotSession() 
            { 
                StartTime = fitSession.GetTimestamp().GetDateTime(), 
                Shots = new List<Shot>(shots.Count)
            };

            foreach (var shotData in shots)
            {
                double value = shotData.GetShotSpeed() ?? 0.0;
                var shotNum = (UInt16)shotData.GetFieldValue("ShotNum");

                var shot = new Shot()
                {
                    Speed = Math.Round(value, SpeedRoundingPrecision),
                    Unit = shotData.GetField("ShotSpeed").Units,
                    Timestamp = shotData.GetTimestamp().GetDateTime(),
                    ShotNumber = shotNum
                };

                shotSession.Shots.Add(shot);
            }

            return shotSession;
        }
    }
}
