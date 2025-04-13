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
        public string ImagePath { get; set; }
        public string PhysicalCharacteristics { get; set; }
        public string OrbitalParameters { get; set; }
        public string ExplorationMissions { get; set; }
        public string OrbitalPeriod { get; set; }
        public string SurfaceTemperature { get; set; }
        
        
        public EphemerisData EphemerisData { get; set; } = new EphemerisData();
        public HorizonResponse RawData { get; set; } = new HorizonResponse();
    
   
    }

    public class EphemerisData
    {
        public double RightAscension { get; set; }
        public double Declination { get; set; }
        public double Distance { get; set; }
        public double Magnitude { get; set; }
    }
}