using SQLite;

namespace Autiva.Models;

/// <summary>
/// Repräsentiert ein einzelnes Prüfprotokoll für ein Fahrzeug.
/// Enthält alle technischen Messwerte und den Zustand der Warnleuchten.
/// </summary>
public class CheckReport
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Fremdschlüssel zum Fahrzeug (Vehicle.Id)
    public int VehicleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Inspector { get; set; } = "";

    public int MileageKm { get; set; }

    // --- Reifenprüfung (Druck in Bar / Profiltiefe in mm) ---
    // FR = Front Right (Vorne Rechts)
    public double TireFR_PressureBar { get; set; }
    public double TireFR_TreadMm { get; set; }

    // RR = Rear Right (Hinten Rechts)
    public double TireRR_PressureBar { get; set; }
    public double TireRR_TreadMm { get; set; }

    // RL = Rear Left (Hinten Links)
    public double TireRL_PressureBar { get; set; }
    public double TireRL_TreadMm { get; set; }

    // FL = Front Left (Vorne Links)
    public double TireFL_PressureBar { get; set; }
    public double TireFL_TreadMm { get; set; }

    // --- Ölstand & Dichtigkeit ---
    // Nutzt das OilStatus-Enum (0=OK, 1=To Check, 2=Urgent)
    public int OilStatus { get; set; }
    public bool OilLeakUnderCar { get; set; }
    public bool OilUnknown { get; set; }

    // --- Aktive Warnleuchten im Cockpit ---
    public bool Warn_EngineLight { get; set; }
    public bool Warn_AbsEsp { get; set; }
    public bool Warn_TirePressure { get; set; }
    public bool Warn_BatteryLight { get; set; }
    public bool Warn_OilLight { get; set; }

    // --- Zusätzliche Dokumentation ---
    public string OpticText { get; set; } = ""; // Optik & Innenraum
    public string Notes { get; set; } = "";      // Allgemeine Notizen

    // Sicherheitsausstattung
    public bool Fleet_EmergencyKitPresent { get; set; }
}