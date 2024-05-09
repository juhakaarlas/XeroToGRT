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
            mesgBroadcaster.RecordMesgEvent += OnRecordMesg;

            // Connect FitListener to get lists of each message type with FitMessages
            decoder.MesgEvent += _fitListener.OnMesg;

            // Decode the FIT File
            try
            {
                bool readOK = decoder.Read(inputStream);

                if (readOK && FitMessages.ChronoShotSessionMesgs.Count > 0)
                {
                    Console.WriteLine("Sessions");
                    foreach (var session in FitMessages.ChronoShotSessionMesgs)
                    {
                        Console.WriteLine($"{session.Name} {session.Num} {session.LocalNum} {session.GetAvgSpeed()}");
                    }
                }

                if (readOK && FitMessages.ChronoShotDataMesgs.Count > 0)
                {
                    Console.WriteLine("Shots");
                    foreach (var item in FitMessages.ChronoShotDataMesgs)
                    {
                        Console.WriteLine($"{item.Name} {item.Num} {item.LocalNum} {item.GetShotNum()} {item.GetShotSpeed()}");
                    }
                }

                return readOK;
            }
        
            catch (Exception ex) when (
                ex is FileTypeException || 
                ex is FitException)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public void OnFileIdMesg(object sender, MesgEventArgs e)
        {
            if (e == null) { return; }

            var fileIdMsg = e.mesg as FileIdMesg;

            if (fileIdMsg == null) { return; }

            Console.WriteLine($"file type: {fileIdMsg.GetType()}");

            if ((e.mesg as FileIdMesg).GetType() != fileType)
            {
                throw new FileTypeException($"Expected FIT File Type: {fileType}, received File Type: {(e.mesg as FileIdMesg).GetType()}");
            }
        }

        public void OnRecordMesg(object sender, MesgEventArgs e)
        {
            foreach (Field field in e.mesg.Fields)
            {
                if (field.Name.ToLower() != "unknown")
                {
                    RecordFieldNames.Add(field.Name);
                }
            }

            foreach (DeveloperField devField in e.mesg.DeveloperFields)
            {
                RecordDeveloperFieldNames.Add(devField.Name);
            }
        }
    }
}