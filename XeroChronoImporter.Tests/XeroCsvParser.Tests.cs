namespace XeroChronoImporter.Tests
{
    public class XeroCsvParserTests
    {
        /** These constants are for testing the individual section parsers more efficiently.
            For full CSV file testing, there is a dedicated test case which loads the included CSV file.
        */
        private const string ValidMpsShotsHeader = "#,SPEED (MPS),Δ AVG (MPS),KE (J),POWER FACTOR (N⋅S),TIME,CLEAN BORE,COLD BORE,SHOT NOTES";
        private const string ValidFpsShotsHeader = "#,SPEED (FPS),Δ AVG (FPS),KE (J),POWER FACTOR (N⋅S),TIME,CLEAN BORE,COLD BORE,SHOT NOTES";
        private const string InvalidShotsHeader = "#, (MPS),Δ AVG (MPS),KE (J),POWER FACTOR (N⋅S),TIME,CLEAN BORE,COLD BORE,SHOT NOTES";

        private const string ValidSessionData = "AVERAGE SPEED,289.1,,,,,,,\r\n" +
                                                "STD DEV,2.0,,,,,,,\r\n" +
                                                "SPREAD,6.2,,,,,,,\r\n" +
                                                "AVERAGE POWER FACTOR,2.7,,,,,,,\r\n" +
                                                "PROJECTILE WEIGHT(GRAINS),145.0,,,,,,,\r\n" +
                                                "SESSION NOTE,\"3.5 gr sub, suppressor\",,,,,,,\r\n" +
                                                "-,,,,,,\r\n" +
                                                "DATE, \"3. May 2024 at 18.56\",,,,,\r\n" +
                                                "All shots included in the calculations,,,,,,,﻿";

        private const string InvalidSessionData = "STD DEV,2.0,,,,,,,\r\n" +
                                                  "SPREAD,6.2,,,,,,,\r\n";

        private const string BadDateSessionData = "AVERAGE SPEED,289.1,,,,,,,\r\n" +
                                                "STD DEV,2.0,,,,,,,\r\n" +
                                                "SPREAD,6.2,,,,,,,\r\n" +
                                                "AVERAGE POWER FACTOR,2.7,,,,,,,\r\n" +
                                                "PROJECTILE WEIGHT(GRAINS),145.0,,,,,,,\r\n" +
                                                "SESSION NOTE,\"3.5 gr sub, suppressor\",,,,,,,\r\n" +
                                                "-,,,,,,\r\n" +
                                                "DAT, \"3. May 2024 at 18.56\",,,,,\r\n" +
                                                "All shots included in the calculations,,,,,,,﻿";


        private const string ShotsData = "1, 286.2, -2.9, 384.8, 2.7, 18.56.42, , X, \"\" \r\n" +
                                         "2, 289.5, 0.4, 393.8, 2.7, 18.56.47, , X, \"\" \r\n" +
                                         "3, 289.0, -0.1, 392.3, 2.7, 18.56.51, , , \"\" \r\n" +
                                         "-,,,,,,\r\n";
        
        private const string TestCsvFile = "xero-import.csv";

        private TestLogger _testLogger;

        public XeroCsvParserTests()
        {
            _testLogger = new TestLogger();
        }

        [Fact]
        public void ParseDateTimeString_Returns_Correct_Value()
        {
            var expectedDate = new DateOnly(2024, 5, 3);
            var expectedTime = new TimeOnly(18, 56);
            string inputDate = "3. May 2024 at 18.56";

            (var date, var time) = XeroCsvParser.ParseDateTimeString(inputDate);
            Assert.Equal(expectedDate, date);
            Assert.Equal(expectedTime, time);
        }

        [Fact]
        public void ReadSessionHeader_Returns_Correct_Session()
        {
            var target = new XeroCsvParser(_testLogger);
            using (var reader = new StreamReader(TestUtils.GenerateStreamFromString("\"Pistol Cartridge, 145,0 gr\"")))
            {
                var actual = target.ReadSessionHeader(reader);
                Assert.NotNull(actual);
                Assert.Equal("Pistol Cartridge", actual.CartridgeType);

            }
        }

        [Fact]
        public void ReadSessionHeader_Returns_Null_For_Empty_Stream()
        {
            var target = new XeroCsvParser(_testLogger);
            using (var reader = new StreamReader(TestUtils.GenerateStreamFromString("")))
            {
                Assert.Null(target.ReadSessionHeader(reader));
            }
        }

        [Theory]
        [InlineData(ValidMpsShotsHeader, SpeedUnit.Mps)]
        [InlineData(ValidFpsShotsHeader, SpeedUnit.Fps)]
        public void GetSpeedUnitFromShotsHeader_Returns_Correct_Value(string input, SpeedUnit expected)
        {
            var target = new XeroCsvParser(_testLogger);
            using (var reader = new StreamReader(TestUtils.GenerateStreamFromString(input)))
            {
                var actual = target.GetSpeedUnitFromShotsHeader(reader);
                Assert.NotNull(actual);
                Assert.Equal(expected, actual);
            }
        }

        [Theory]
        [InlineData(InvalidShotsHeader, "The CSV file has an unexpected shot list header.")]
        public void GetSpeedUnitFromShotsHeader_Throws_With_Incorrect_Value(string input, string exceptionMessage)
        {
            var target = new XeroCsvParser(_testLogger);
            using (var reader = new StreamReader(TestUtils.GenerateStreamFromString(input)))
            {
                var ex = Assert.Throws<FormatException>( () => target.GetSpeedUnitFromShotsHeader(reader));
                Assert.Equal(exceptionMessage, ex.Message);
            }
        }

        [Theory]
        [InlineData("MPS", SpeedUnit.Mps)]
        [InlineData("FPS", SpeedUnit.Fps)]
        public void ConvertSpeedUnit_Returns_Correct_Value_For_MPS(string input, SpeedUnit expected)
        {
            Assert.Equal(expected, XeroCsvParser.ConvertSpeedUnit(input));
        }

        [Fact]
        public void ReadSessionData_Returns_Correct_Values()
        {
            var target = new XeroCsvParser(_testLogger);
            var session = new ShotSession();

            using (var reader = new StreamReader(TestUtils.GenerateStreamFromString(ValidSessionData)))
            {
                var actual = target.ReadSessionData(reader, session);

                Assert.Equal(new DateTime(2024, 5, 3, 18, 56, 0), actual.StartTime);
                Assert.Equal(145, actual.Weight);
                Assert.Equal(WeightUnit.Grains, actual.WeightUnit);
                Assert.Equal("3.5 gr sub, suppressor", actual.Note);
            }
        }

        [Fact]
        public void ReadSessionData_Throws_With_Incorrect_Fields()
        {
            var target = new XeroCsvParser(_testLogger);
            var session = new ShotSession();

            using (var reader = new StreamReader(TestUtils.GenerateStreamFromString(InvalidSessionData)))
            {
                Assert.Throws<FormatException>(() => target.ReadSessionData(reader, session));
            }
        }

        [Fact]
        public void ReadSessionData_Throws_With_Bad_Date_Field()
        {
            var target = new XeroCsvParser(_testLogger);
            var session = new ShotSession();

            using (var reader = new StreamReader(TestUtils.GenerateStreamFromString(BadDateSessionData)))
            {
                Assert.Throws<FormatException>(() => target.ReadSessionData(reader, session));
            }
        }

        [Fact]
        public void ReadShots_Returns_Correct_Values()
        {
            var target = new XeroCsvParser(_testLogger);

            using (var reader = new StreamReader(TestUtils.GenerateStreamFromString(ShotsData)))
            {
                var actual = target.ReadShots(reader, SpeedUnit.Mps);
                Assert.Equal(3, actual.Count);
                Assert.Equal(1, actual[0].ShotNumber);
                Assert.Equal(286.2, actual[0].Speed);
                Assert.Equal(SpeedUnit.Mps, actual[0].Unit);
                Assert.True(actual[0].ColdBore);
                Assert.False(actual[0].CleanBore);
                Assert.Equal(new TimeOnly(18, 56, 42), actual[0].Time);
            }
        }

        [Fact]
        public void Can_Read_Csv_File()
        {
            using (var fileStream = TestUtils.GetTestDataFileStream(TestCsvFile))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    var target = new XeroCsvParser(_testLogger);
                    var session = target.ReadXeroCsvFile(reader);

                    Assert.NotNull(session);
                    Assert.Equal(10, session.ShotCount);
                    Assert.Equal("Pistol Cartridge", session.CartridgeType);
                    Assert.Equal(SpeedUnit.Mps, session.SpeedUnit);
                }
            }
        }

        [Fact]
        public void Process_Throws_When_File_Doesnt_Exist() 
        {
            var target = new XeroCsvParser(_testLogger);
            Assert.Throws<FileNotFoundException>(() => target.Process("nope"));
        }

        [Fact]
        public void Can_Process_Csv_File()
        {
            var target = new XeroCsvParser(_testLogger);
            var session = target.Process(TestUtils.GetTestDataFilePath(TestCsvFile));

            Assert.NotNull(session);
            Assert.Equal(10, session.ShotCount);
            Assert.Equal("Pistol Cartridge", session.CartridgeType);
            Assert.Equal(SpeedUnit.Mps, session.SpeedUnit);
        }

        [Fact]
        public void Logs_Nothing_If_Not_Verbose()
        {
            var target = new XeroCsvParser(_testLogger);
            var session = target.Process(TestUtils.GetTestDataFilePath(TestCsvFile));
            Assert.Empty(_testLogger.Messages);
        }

        [Fact]
        public void Logs_When_Verbose()
        {
            var target = new XeroCsvParser(_testLogger)
            {
                Verbose = true
            };

            var session = target.Process(TestUtils.GetTestDataFilePath(TestCsvFile));

            Assert.NotEmpty(_testLogger.Messages);
            Assert.True(_testLogger.Messages.Contains("Processing CSV file..."));
            Assert.True(_testLogger.Messages.Contains("Processing shot data..."));
            Assert.True(_testLogger.Messages.Contains("1, 337.4, -3.6, 457.3, 2.7, 18.19.39, , , \"\" "));

        }
        
    }
}
