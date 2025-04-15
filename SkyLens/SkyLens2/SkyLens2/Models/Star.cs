namespace SkyLens2.Models
{
    public class Star
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = ""; // Spectral class
        public double Distance { get; set; }
        public double AbsoluteMagnitude { get; set; }
        public double ApparentMagnitude { get; set; }
        public double Azimuth { get; set; }
        public double Altitude { get; set; }
        public string Constellation { get; set; } = "";
        public string RightAscension { get; set; } = "";
        public string Declination { get; set; } = "";
    }
}