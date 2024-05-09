using Dynastream.Fit;
using File = Dynastream.Fit.File;

namespace XeroChronoImporter
{
    public class ShotSessionParser
    {
        private const int SpeedRoundingPrecision = 1;

        private FitMessages _messages;

        public bool IsShotSessionFile => _firstFileIdMesg != null ? (_firstFileIdMesg.GetType() ?? File.Invalid) == ShotSessionDecoder.ShotSessionFile : false;

        public FileIdMesg _firstFileIdMesg => _messages.FileIdMesgs.FirstOrDefault();

        public ShotSessionParser(FitMessages messages)
        {
            _messages = messages;
        }

        public ShotSession ParseShotSession()
        {
            if (!IsShotSessionFile)
            {
                throw new Exception($"Expected FIT File Type: ShotSession, received File Type: {_firstFileIdMesg?.GetType()}");
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
                var shot = new Shot()
                {
                    Speed = Math.Round((double)shotData.GetShotSpeed(), SpeedRoundingPrecision),
                    Unit = shotData.GetField("ShotSpeed").Units,
                    Timestamp = shotData.GetTimestamp().GetDateTime()
                };

                var shotNum = (UInt16)shotData.GetFieldValue("ShotNum");
                shot.ShotNumber = checked((int)shotNum);
                shotSession.Shots.Add(shot);
            }

            return shotSession;
        }
    }
}
