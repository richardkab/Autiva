using Autiva.Helpers;
using Autiva.Models;
using Autiva.Services;

namespace Autiva.Pages;

[QueryProperty(nameof(VehicleId), "id")]
public partial class VehicleDetailsPage : ContentPage
{
    private readonly AutivaDb _db;
    private Vehicle? _vehicle;

    // ID wird über die Shell-Navigation übergeben
    public string VehicleId { get; set; } = "";

    public VehicleDetailsPage()
    {
        InitializeComponent();
        _db = ServiceLocator.GetRequiredService<AutivaDb>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadVehicleDataAsync();
    }

    // Lädt das Fahrzeug basierend auf der übergebenen ID aus der Datenbank
    private async Task LoadVehicleDataAsync()
    {
        if (!int.TryParse(VehicleId, out var id))
        {
            await DisplayAlertAsync("Fehler", "Ungültige Fahrzeug-ID.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        _vehicle = await _db.GetVehicleAsync(id);
        if (_vehicle == null)
        {
            await DisplayAlertAsync("Nicht gefunden", "Das Fahrzeug existiert nicht mehr.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        // UI mit den Datenbankwerten füllen
        HeaderLabel.Text = $"ID: {_vehicle.Id} • erstellt: {_vehicle.CreatedAt:dd.MM.yyyy HH:mm}";
        LicensePlateEntry.Text = _vehicle.LicensePlate;
        MakeModelEntry.Text = _vehicle.MakeModel;
        VinEntry.Text = _vehicle.Vin;
        NotesEditor.Text = _vehicle.Notes;
    }

    private async void OnBack(object sender, EventArgs e) => await Shell.Current.GoToAsync("..");

    // Speichert Änderungen am Fahrzeugdatensatz
    private async void OnSave(object sender, EventArgs e)
    {
        if (_vehicle == null) return;

        var plate = (LicensePlateEntry.Text ?? "").Trim();
        var makeModel = (MakeModelEntry.Text ?? "").Trim();

        if (string.IsNullOrWhiteSpace(plate) || string.IsNullOrWhiteSpace(makeModel))
        {
            await DisplayAlertAsync("Eingabe fehlt", "Kennzeichen und Marke sind Pflichtfelder.", "OK");
            return;
        }

        // Objekt aktualisieren und in DB schreiben
        _vehicle.LicensePlate = plate.ToUpperInvariant();
        _vehicle.MakeModel = makeModel;
        _vehicle.Vin = (VinEntry.Text ?? "").Trim();
        _vehicle.Notes = (NotesEditor.Text ?? "").Trim();

        await _db.UpdateVehicleAsync(_vehicle);
        await DisplayAlertAsync("Erfolg", "Fahrzeugdaten wurden aktualisiert.", "OK");
    }

    // Löscht das Fahrzeug nach einer Sicherheitsabfrage
    private async void OnDelete(object sender, EventArgs e)
    {
        if (_vehicle == null) return;

        bool confirm = await DisplayAlertAsync(
            "Fahrzeug löschen?",
            $"Soll „{_vehicle.LicensePlate}“ wirklich dauerhaft gelöscht werden?",
            "Ja, löschen",
            "Abbrechen");

        if (!confirm) return;

        await _db.DeleteVehicleAsync(_vehicle.Id);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnStartCheck(object sender, EventArgs e)
    {
        if (_vehicle == null) return;
        await Shell.Current.GoToAsync($"{nameof(AutivaCheckPage)}?id={_vehicle.Id}");
    }
}