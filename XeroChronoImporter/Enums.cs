using System.ComponentModel;

namespace XeroChronoImporter
{
    public enum SpeedUnit
    {
        [Description("m/s")]
        Mps,
        [Description("fps")]
        Fps,
        [Description("km/h")]
        Kph,
        [Description("mph")]
        Mph
    }

    public enum WeightUnit
    {
        [Description("Grains")]
        Grains,
        [Description("Grams")]
        Grams
    }
}
