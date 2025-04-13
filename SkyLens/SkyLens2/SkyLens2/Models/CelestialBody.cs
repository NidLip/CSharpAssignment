using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SkyLens2.Models
{
    public class HorizonResponse
    {
        [JsonPropertyName("signature")]
        public Signature Signature { get; set; } = new Signature();
    
        [JsonPropertyName("result")]
        public string RawResult { get; set; }
    
        [JsonPropertyName("data")]
        public List<EphemerisData> EphemerisData { get; set; } = new List<EphemerisData>();
    }

    public class Signature
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }
        
        [JsonPropertyName("source")]
        public string Source { get; set; }
    }

    public class CelestialBody
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EphemerisData EphemerisData { get; set; } = new EphemerisData();
        // Initialize with default to prevent null references
        public HorizonResponse Data { get; set; } = new HorizonResponse();
    
        // Add calculated properties for direct access
        public double RightAscension => Data.EphemerisData.FirstOrDefault()?.RightAscension ?? 0;
        public double Declination => Data.EphemerisData.FirstOrDefault()?.Declination ?? 0;
        public double Distance => Data.EphemerisData.FirstOrDefault()?.Distance ?? 0;
        public double Magnitude => Data.EphemerisData.FirstOrDefault()?.Magnitude ?? 0;
    }

    public class EphemerisData
    {
        public double RightAscension { get; set; }
        public double Declination { get; set; }
        public double Distance { get; set; }
        public double Magnitude { get; set; }
    }
}