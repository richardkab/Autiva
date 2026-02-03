namespace Autiva.Models;

/// <summary>
/// Definiert den Zustand des Motoröls.
/// Wird im Prüfprotokoll verwendet, um den Wartungsbedarf zu signalisieren.
/// </summary>
public enum OilStatus
{
    // Alles in Ordnung (z.B. Stand und Farbe passen)
    Ok = 0,

    // Beobachten (z.B. Stand an der unteren Grenze)
    ToCheck = 1,

    // Kritisch (z.B. Ölstand zu niedrig oder Wechsel überfällig)
    Urgent = 2
}