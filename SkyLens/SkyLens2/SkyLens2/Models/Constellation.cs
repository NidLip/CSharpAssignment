public class Constellation
{
    public string IAUCode { get; set; }
    public string LatinName { get; set; }
    public string EnglishName { get; set; }
    public string FrenchName { get; set; }
    public string Season { get; set; }
    public string NameOrigin { get; set; }
    
    public string Declination { get; set; }
    public string RightAscension { get; set; }
    public string PrincipalStar { get; set; }
    public double AreaInDegrees { get; set; }
    public string AreaPercentOfCelestialSphere { get; set; }
    public string CelestialEquatorZone { get; set; }
    public string EclipticZone { get; set; }
    public string MilkyWayZone { get; set; }
    public string HemisphereQuadrant { get; set; }

    // Simplified image info
    public string ImageUrl { get; set; }

    public override string ToString()
    {
        return $"{LatinName} ({EnglishName}) - Principal Star: {PrincipalStar}, Area: {AreaInDegrees}°";
    }
}