using Autiva.Helpers;
using Autiva.Models;
using Autiva.Services;

namespace Autiva.Pages;

public partial class AddCarPage : ContentPage
{
    private readonly AutivaDb _db;

    public AddCarPage()
    {
        InitializeComponent();
        _db = ServiceLocator.GetRequiredService<AutivaDb>();

        // Standardwert für Prüfer setzen, um Tipparbeit zu sparen
        if (string.IsNullOrWhiteSpace(InspectorEntry.Text))
            InspectorEntry.Text = "Richard";
    }

    private async void OnBack(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");

    private async void OnSave(object sender, EventArgs e)
    {
        try
        {
            StatusLabel.Text = "";

            // Eingaben bereinigen (Trim)
            var plate = (LicensePlateEntry.Text ?? "").Trim();
            var makeModel = (MakeModelEntry.Text ?? "").Trim();

            // Validierung der Pflichtfelder
            if (string.IsNullOrWhiteSpace(plate))
            {
                await DisplayAlertAsync("Eingabe fehlt", "Bitte das Kennzeichen angeben.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(makeModel))
            {
                await DisplayAlertAsync("Eingabe fehlt", "Bitte Marke und Modell angeben.", "OK");
                return;
            }

            // Kilometerstand parsen und validieren
            int mileageKm = 0;
            var mileageText = (MileageEntry.Text ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(mileageText))
            {
                if (!int.TryParse(mileageText, out mileageKm) || mileageKm < 0)
                {
                    await DisplayAlertAsync("Ungültiger Wert", "Der Kilometerstand muss eine positive Zahl sein.", "OK");
                    return;
                }
            }

            // Neues Fahrzeug-Objekt erstellen
            var vehicle = new Vehicle
            {
                LicensePlate = plate.ToUpperInvariant(), // Kennzeichen immer in Großbuchstaben
                MakeModel = makeModel,
                Vin = (VinEntry.Text ?? "").Trim(),
                Notes = (NotesEditor.Text ?? "").Trim(),
                MileageKm = mileageKm,
                LastInspector = (InspectorEntry.Text ?? "Unbekannt").Trim(),
                CreatedAt = DateTime.Now
            };

            // In lokaler SQLite Datenbank speichern
            await _db.AddVehicleAsync(vehicle);

            StatusLabel.Text = $"Fahrzeug {vehicle.LicensePlate} erfolgreich angelegt.";

            // Zurück zur Übersicht
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fehler beim Speichern", ex.Message, "OK");
        }
    }
}