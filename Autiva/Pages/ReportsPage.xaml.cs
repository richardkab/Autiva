using Autiva.Helpers;
using Autiva.Models;
using Autiva.Services;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Globalization;
using SfDrawing = Syncfusion.Drawing;

namespace Autiva.Pages;

public partial class ReportsPage : ContentPage
{
    private readonly AutivaDb _db;
    private List<Vehicle> _vehicles = new();
    private Vehicle? _selectedVehicle;
    private CheckReport? _selectedReport;

    public ReportsPage()
    {
        InitializeComponent();
        _db = ServiceLocator.GetRequiredService<AutivaDb>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadVehiclesAsync();
    }

    // Fahrzeuge für den Picker laden
    private async Task LoadVehiclesAsync()
    {
        _vehicles = await _db.GetVehiclesAsync();
        VehiclePicker.ItemsSource = _vehicles;
        VehiclePicker.ItemDisplayBinding = new Binding(nameof(Vehicle.DisplayName));
    }

    // Wenn ein Fahrzeug gewählt wird, die zugehörigen Berichte laden
    private async void OnVehicleChanged(object sender, EventArgs e)
    {
        _selectedReport = null;
        if (VehiclePicker.SelectedIndex < 0) return;

        _selectedVehicle = _vehicles[VehiclePicker.SelectedIndex];
        ReportsCollection.ItemsSource = await _db.GetReportsByVehicleAsync(_selectedVehicle.Id);
    }

    private void OnReportSelected(object sender, SelectionChangedEventArgs e)
        => _selectedReport = e.CurrentSelection?.FirstOrDefault() as CheckReport;

    private async void OnExportSelectedPdf(object sender, EventArgs e)
    {
        try
        {
            if (_selectedVehicle == null || _selectedReport == null)
            {
                await DisplayAlertAsync("Hinweis", "Bitte Fahrzeug und Check auswählen.", "OK");
                return;
            }

            // PDF-Erstellung starten
            var pdfPath = await ExportPdfAsync(_selectedVehicle, _selectedReport);

            LastPdfLabel.Text = $"Gespeichert unter:\n{pdfPath}";
            await DisplayAlertAsync("PDF erstellt", "Das Protokoll wurde erfolgreich gespeichert.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fehler", "PDF konnte nicht erstellt werden: " + ex.Message, "OK");
        }
    }

    // --- PDF LOGIK (Syncfusion) ---
    private async Task<string> ExportPdfAsync(Vehicle vehicle, CheckReport report)
    {
        // Zielverzeichnis im lokalen App-Speicher
        var exportDir = Path.Combine(FileSystem.AppDataDirectory, "exports");
        Directory.CreateDirectory(exportDir);

        // Dateiname generieren
        var fileName = $"{vehicle.LicensePlate.Replace(" ", "_")}_{report.CreatedAt:yyyy-MM-dd}.pdf";
        var path = Path.Combine(exportDir, fileName);

        using var document = new PdfDocument();
        var page = document.Pages.Add();
        var g = page.Graphics;

        // Schriftarten definieren
        var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
        var hFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
        var font = new PdfStandardFont(PdfFontFamily.Helvetica, 11);

        float margin = 20;
        float y = 20;

        // Hilfsfunktion: Prüft, ob auf der Seite noch Platz ist, sonst neue Seite
        void EnsureSpace(float needed)
        {
            if (y + needed > page.GetClientSize().Height - 20)
            {
                page = document.Pages.Add();
                g = page.Graphics;
                y = 20;
            }
        }

        // --- PDF INHALT ZEICHNEN ---
        g.DrawString("AUTIVA - Prüfprotokoll", titleFont, PdfBrushes.Black, new SfDrawing.PointF(margin, y));
        y += 40;

        // Stammdaten
        EnsureSpace(60);
        g.DrawString($"Fahrzeug: {vehicle.DisplayName}", hFont, PdfBrushes.Black, new SfDrawing.PointF(margin, y));
        y += 20;
        g.DrawString($"Datum: {report.CreatedAt:dd.MM.yyyy HH:mm} | Prüfer: {report.Inspector}", font, PdfBrushes.Black, new SfDrawing.PointF(margin, y));
        y += 30;

        // Reifen-Tabelle (vereinfacht gezeichnet)
        g.DrawString("Reifenprüfung:", hFont, PdfBrushes.Black, new SfDrawing.PointF(margin, y));
        y += 20;
        DrawTireLine(g, "Vorne Rechts", report.TireFR_PressureBar, report.TireFR_TreadMm, ref y, margin, font);
        DrawTireLine(g, "Vorne Links", report.TireFL_PressureBar, report.TireFL_TreadMm, ref y, margin, font);
        // ... weitere Reifen analog ...

        y += 20;
        g.DrawString($"Öl-Status: {OilStatusText(report.OilStatus)}", font, PdfBrushes.Black, new SfDrawing.PointF(margin, y));

        // Speichern
        using (var stream = File.Create(path))
            document.Save(stream);

        return path;
    }

    private void DrawTireLine(PdfGraphics g, string pos, double p, double t, ref float y, float margin, PdfStandardFont f)
    {
        g.DrawString($"{pos}: {p:0.0} bar / {t:0.0} mm", f, PdfBrushes.Black, new SfDrawing.PointF(margin + 10, y));
        y += 15;
    }

    private static string OilStatusText(int s) => s switch { 0 => "OK", 1 => "Prüfen", 2 => "Dringend", _ => "Unbekannt" };

    private async void OnBack(object sender, EventArgs e) => await Shell.Current.GoToAsync("..");
}