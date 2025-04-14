namespace SkyLens2.Models;
using System;

public class Planet
{
    public String Name {get; set;}
    public double Radius { get; set;}
    public double Mass { get; set;}
    public double Distance { get; set;}
    public bool IsVisible { get; set;}
    
    public Planet(String name, double radius, double mass, double distance, bool isVisible=true)
        {
        this.Name = name;
        this.Radius = radius;
        this.Mass = mass;
        this.Distance = distance;
        this.IsVisible = isVisible;
        }
}
