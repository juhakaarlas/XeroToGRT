﻿using XeroChronoImporter;

namespace XeroChronoImporter.Tests
{
    public class XeroCsvReaderTests
    {
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

        private const string ShotsData = "1, 286.2, -2.9, 384.8, 2.7, 18.56.42, , X, \"\" \r\n" +
                                         "2, 289.5, 0.4, 393.8, 2.7, 18.56.47, , X, \"\" \r\n" +
                                         "3, 289.0, -0.1, 392.3, 2.7, 18.56.51, , , \"\" \r\n" +
                                         "-,,,,,,\r\n";


        [Fact]
        public void ParseDateTimeString_Returns_Correct_Value()
        {
            var expectedDate = new DateOnly(2024, 5, 3);
            var expectedTime = new TimeOnly(18, 56);
            string inputDate = "3. May 2024 at 18.56";

            (var date, var time) = XeroCsvReader.ParseDateTimeString(inputDate);
            Assert.Equal(expectedDate, date);
            Assert.Equal(expectedTime, time);
        }

        [Fact]
        public void ReadSessionHeader_Returns_Correct_Session()
        {
            var target = new XeroCsvReader();
            using (var reader = new StreamReader(GenerateStreamFromString("\"Pistol Cartridge, 145,0 gr\"")))
            {
                var actual = target.ReadSessionHeader(reader);
                Assert.NotNull(actual);
                Assert.Equal("Pistol Cartridge", actual.CartridgeType);

            }
        }

        [Theory]
        [InlineData(ValidMpsShotsHeader, "m/s")]
        [InlineData(ValidFpsShotsHeader, "FPS")]
        public void GetSpeedUnitFromShotsHeader_Returns_Correct_Value(string input, string expected)
        {
            var target = new XeroCsvReader();
            using (var reader = new StreamReader(GenerateStreamFromString(input)))
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
            var target = new XeroCsvReader();
            using (var reader = new StreamReader(GenerateStreamFromString(input)))
            {
                var ex = Assert.Throws<FormatException>( () => target.GetSpeedUnitFromShotsHeader(reader));
                Assert.Equal(exceptionMessage, ex.Message);
            }
        }

        [Theory]
        [InlineData("MPS", "m/s")]
        [InlineData("FPS", "FPS")]
        public void ConvertSpeedUnit_Returns_Correct_Value_For_MPS(string input, string expected)
        {
            Assert.Equal(expected, XeroCsvReader.ConvertSpeedUnit(input));
        }

        [Fact]
        public void ReadSessionData_Returns_Correct_Values()
        {
            var target = new XeroCsvReader();
            var session = new ShotSession();

            using (var reader = new StreamReader(GenerateStreamFromString(ValidSessionData)))
            {
                var actual = target.ReadSessionData(reader, session);

                Assert.Equal(new DateTime(2024, 5, 3, 18, 56, 0), actual.StartTime);
                Assert.Equal(145, actual.Weight);
                Assert.Equal(WeightUnit.Grains, actual.WeightUnit);
                Assert.Equal("3.5 gr sub, suppressor", actual.Note);
            }
        }

        [Fact]
        public void ReadShots_Returns_Correct_Values()
        {
            var target = new XeroCsvReader();

            using (var reader = new StreamReader(GenerateStreamFromString(ShotsData)))
            {
                var actual = target.ReadShots(reader, "m/s");
                Assert.Equal(3, actual.Count);
                Assert.Equal(1, actual[0].ShotNumber);
                Assert.Equal(286.2, actual[0].Speed);
                Assert.Equal("m/s", actual[0].Unit);
                Assert.True(actual[0].ColdBore);
                Assert.False(actual[0].CleanBore);
                Assert.Equal(new TimeOnly(18, 56, 42), actual[0].Time);
            }
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
