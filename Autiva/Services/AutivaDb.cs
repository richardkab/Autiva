using Autiva.Models;
using SQLite;

namespace Autiva.Services;

/// <summary>
/// Datenzugriffsschicht (DAL). Veraltet die SQLite-Verbindung und 
/// kapselt alle Datenbankoperationen asynchron.
/// </summary>
public class AutivaDb
{
    private readonly SQLiteAsyncConnection _db;
    private bool _initialized;

    public AutivaDb()
    {
        // Datenbankpfad im lokalen App-Datenverzeichnis ermitteln
        var path = Path.Combine(FileSystem.AppDataDirectory, "autiva.db3");
        _db = new SQLiteAsyncConnection(path);

        // Initialisierung der Tabellen im Hintergrund starten
        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        if (_initialized) return;
        _initialized = true;

        // Erstellt die Tabellen, falls sie noch nicht existieren
        await _db.CreateTableAsync<Vehicle>();
        await _db.CreateTableAsync<CheckReport>();

        // Mini-Migration: Stellt sicher, dass das Feld 'OpticText' existiert
        await TryAddColumnAsync("CheckReport", "OpticText", "TEXT");
    }

    /// <summary>
    /// Prüft, ob eine Spalte existiert und fügt sie bei Bedarf hinzu (Schema-Update).
    /// </summary>
    private async Task TryAddColumnAsync(string table, string column, string typeSql)
    {
        try
        {
            var info = await _db.QueryAsync<TableInfo>($"PRAGMA table_info({table})");
            if (info.Any(c => string.Equals(c.name, column, StringComparison.OrdinalIgnoreCase)))
                return;

            await _db.ExecuteAsync($"ALTER TABLE {table} ADD COLUMN {column} {typeSql}");
        }
        catch
        {
            // Fehler bei der Migration werden abgefangen, um den App-Start nicht zu blockieren
        }
    }

    // Hilfsklasse für PRAGMA-Abfragen
    private sealed class TableInfo
    {
        public string name { get; set; } = "";
    }

    // --- Fahrzeug-Operationen (CRUD) ---

    public async Task<List<Vehicle>> GetVehiclesAsync()
        => await _db.Table<Vehicle>()
            .OrderByDescending(v => v.LastCheckDate)
            .ToListAsync();

    public async Task<Vehicle?> GetVehicleAsync(int id)
        => await _db.Table<Vehicle>().Where(v => v.Id == id).FirstOrDefaultAsync();

    public async Task<int> AddVehicleAsync(Vehicle v)
        => await _db.InsertAsync(v);

    public async Task<int> UpdateVehicleAsync(Vehicle v)
        => await _db.UpdateAsync(v);

    public async Task<int> DeleteVehicleAsync(int id)
        => await _db.DeleteAsync<Vehicle>(id);

    // --- Berichts-Operationen ---

    /// <summary>
    /// Speichert einen neuen Prüfbericht in der Datenbank.
    /// </summary>
    public async Task<int> SaveCheckReportAsync(CheckReport r)
        => await _db.InsertAsync(r);

    public async Task<List<CheckReport>> GetReportsByVehicleAsync(int vehicleId)
        => await _db.Table<CheckReport>()
            .Where(r => r.VehicleId == vehicleId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<CheckReport?> GetReportAsync(int reportId)
        => await _db.Table<CheckReport>().Where(r => r.Id == reportId).FirstOrDefaultAsync();
}