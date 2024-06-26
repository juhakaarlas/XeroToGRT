﻿using Dynastream.Fit;

namespace XeroChronoImporter { 

    public class ShotSessionDecoder
    {
        public FitMessages FitMessages => _fitListener.FitMessages;

        public HashSet<string> RecordFieldNames = new HashSet<string>();
        public HashSet<string> RecordDeveloperFieldNames = new HashSet<string>();

        private FitListener _fitListener = new FitListener();
        private Stream inputStream;
        private Dynastream.Fit.File fileType;

        public const Dynastream.Fit.File ShotSessionFile = (Dynastream.Fit.File) 54;

        public bool Verbose { get; set; }

        public ShotSessionDecoder(Stream stream, Dynastream.Fit.File fileType)
        {
            inputStream = stream;
            this.fileType = fileType;
        }

        public bool Decode()
        {
            // Create the Decode Object
            Decode decoder = new Decode();

            // Check that this is a FIT file
            if (!decoder.IsFIT(inputStream))
            {
                throw new FileTypeException($"Expected FIT File Type: {fileType}, received a non FIT file.");
            }

            // Create the Message Broadcaster Object
            MesgBroadcaster mesgBroadcaster = new MesgBroadcaster();

            // Connect the the Decode and Message Broadcaster Objects
            decoder.MesgEvent += mesgBroadcaster.OnMesg;

            // Connect Message Broadcaster Events to their onMesg delegates
            mesgBroadcaster.FileIdMesgEvent += OnFileIdMesg;

            // Connect FitListener to get lists of each message type with FitMessages
            decoder.MesgEvent += _fitListener.OnMesg;

            if (Verbose)
            {
                mesgBroadcaster.ChronoShotSessionMesgEvent += OnChronoShotSessionMesg;
                mesgBroadcaster.ChronoShotDataMesgEvent += OnChronoShotDataMesg ;
            }

            // Decode the FIT File
            try
            {
                return decoder.Read(inputStream);
            }
        
            catch (Exception ex) when (
                ex is FileTypeException || 
                ex is FitException)
            {
                Console.Error.WriteLine(ex.Message);
                return false;
            }
        }

        private void OnChronoShotDataMesg(object sender, MesgEventArgs e)
        {
            var msg = e.mesg as ChronoShotDataMesg;

            if (msg == null) { return; }
            
            Console.WriteLine($"{msg.Name} {msg.Num} {msg.LocalNum} {msg.GetShotNum()} {msg.GetShotSpeed()}");
        }

        private void OnChronoShotSessionMesg(object sender, MesgEventArgs e)
        {
            var msg = e.mesg as ChronoShotSessionMesg;
            
            if (msg == null) { return; }

            Console.WriteLine($"Session {msg.Name} avg speed: {msg.GetAvgSpeed()}");
        }

        public void OnFileIdMesg(object sender, MesgEventArgs e)
        {
            if (e == null) { return; }

            var fileIdMsg = e.mesg as FileIdMesg;

            if (fileIdMsg == null) { return; }

            if (Verbose)
            {
                Console.WriteLine($"File type: {fileIdMsg.GetType()}");
            }

            if (fileIdMsg.GetType() != fileType)
            {
                throw new FileTypeException($"Expected FIT File Type: {fileType}, received File Type: {fileIdMsg.GetType()}");
            }
        }
    }
}
