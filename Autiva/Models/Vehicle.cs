using SQLite;

namespace Autiva.Models;

/// <summary>
/// Repräsentiert ein Fahrzeug in der Datenbank.
/// </summary>
public class Vehicle
{
    // Eindeutige ID, wird von SQLite automatisch hochgezählt
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string LicensePlate { get; set; } = "";
    public string MakeModel { get; set; } = "";
    public string Vin { get; set; } = "";
    public string Notes { get; set; } = "";

    public int MileageKm { get; set; }
    public DateTime? LastCheckDate { get; set; }
    public string LastInspector { get; set; } = "";

    // Zeitpunkt der Anlage des Datensatzes
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Hilfseigenschaft für die UI-Darstellung. 
    /// Wird nicht in der Datenbank gespeichert (Ignore-Attribut).
    /// </summary>
    [Ignore]
    public string DisplayName => $"{LicensePlate} • {MakeModel}";
}