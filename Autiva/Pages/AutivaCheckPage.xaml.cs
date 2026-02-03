using Autiva.Helpers;
using Autiva.Models;
using Autiva.Services;
using System.Globalization;

namespace Autiva.Pages;

[QueryProperty(nameof(VehicleId), "id")]
public partial class AutivaCheckPage : ContentPage
{
    private readonly AutivaDb _db;
    public string VehicleId { get; set; } = "";
    private int _vehicleId;
    private Vehicle? _vehicle;

    public AutivaCheckPage()
    {
        InitializeComponent();
        _db = ServiceLocator.GetRequiredService<AutivaDb>();
        SetTab("tires"); // Start-Tab
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadVehicleData();
    }

    private async Task LoadVehicleData()
    {
        if (int.TryParse(VehicleId, out _vehicleId))
        {
            _vehicle = await _db.GetVehicleAsync(_vehicleId);
            if (_vehicle != null)
            {
                VehicleInfoLabel.Text = $"{_vehicle.MakeModel} ({_vehicle.LicensePlate})";
                InspectorEntry.Text = _vehicle.LastInspector;
                MileageEntry.Text = _vehicle.MileageKm.ToString();
            }
        }
    }

    // Tab-Umschaltung
    private void OnTabTires(object sender, EventArgs e) => SetTab("tires");
    private void OnTabOil(object sender, EventArgs e) => SetTab("oil");
    private void OnTabDmg(object sender, EventArgs e) => SetTab("dmg");
    private void OnTabFinal(object sender, EventArgs e) => SetTab("final");

    private void SetTab(string tab)
    {
        PanelTires.IsVisible = tab == "tires";
        PanelOil.IsVisible = tab == "oil";
        PanelDmg.IsVisible = tab == "dmg";
        PanelFinal.IsVisible = tab == "final";
        UpdateTabColors();
    }

    private void UpdateTabColors()
    {
        var active = (Color)Application.Current.Resources["Blue"];
        var inactive = (Color)Application.Current.Resources["Card"];

        TabBtnTires.BackgroundColor = PanelTires.IsVisible ? active : inactive;
        TabBtnOil.BackgroundColor = PanelOil.IsVisible ? active : inactive;
        TabBtnDmg.BackgroundColor = PanelDmg.IsVisible ? active : inactive;
        TabBtnFinal.BackgroundColor = PanelFinal.IsVisible ? active : inactive;
    }

    // DIE SPEICHER-METHODE (Fehler behoben & vervollständigt)
    private async void OnSaveCheck(object sender, EventArgs e)
    {
        try
        {
            var report = new CheckReport
            {
                VehicleId = _vehicleId,
                CreatedAt = DateTime.Now,
                Inspector = InspectorEntry.Text ?? "",
                MileageKm = int.TryParse(MileageEntry.Text, out var m) ? m : 0,

                // Reifen-Daten parsen
                TireFR_PressureBar = double.TryParse(FR_Pressure.Text, out var p1) ? p1 : 0,
                TireFR_TreadMm = double.TryParse(FR_Tread.Text, out var t1) ? t1 : 0,
                TireRR_PressureBar = double.TryParse(RR_Pressure.Text, out var p2) ? p2 : 0,
                TireRR_TreadMm = double.TryParse(RR_Tread.Text, out var t2) ? t2 : 0,
                TireRL_PressureBar = double.TryParse(RL_Pressure.Text, out var p3) ? p3 : 0,
                TireRL_TreadMm = double.TryParse(RL_Tread.Text, out var t3) ? t3 : 0,
                TireFL_PressureBar = double.TryParse(FL_Pressure.Text, out var p4) ? p4 : 0,
                TireFL_TreadMm = double.TryParse(FL_Tread.Text, out var t4) ? t4 : 0,

                // Öl-Status
                OilStatus = OilOk.IsChecked ? 0 : (OilToCheck.IsChecked ? 1 : 2),
                OilLeakUnderCar = OilLeak.IsChecked,
                OilUnknown = OilUnknown.IsChecked,

                // Warnungen
                Warn_EngineLight = WarnEngine.IsChecked,
                Warn_AbsEsp = WarnAbsEsp.IsChecked,
                Warn_TirePressure = WarnTirePressure.IsChecked,
                Warn_BatteryLight = WarnBattery.IsChecked,
                Warn_OilLight = WarnOil.IsChecked,

                OpticText = OpticEditor.Text ?? "",
                Notes = NotesEditor.Text ?? ""
            };

            await _db.SaveCheckReportAsync(report);

            // Fahrzeug-Stammdaten mit aktuellem Check aktualisieren
            if (_vehicle != null)
            {
                _vehicle.LastCheckDate = report.CreatedAt;
                _vehicle.MileageKm = report.MileageKm;
                _vehicle.LastInspector = report.Inspector;
                await _db.UpdateVehicleAsync(_vehicle);
            }

            await DisplayAlertAsync("Gespeichert", "Der Check wurde erfolgreich abgeschlossen.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fehler", "Check konnte nicht gespeichert werden: " + ex.Message, "OK");
        }
    }
}