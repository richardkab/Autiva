using Autiva.Helpers;
using Autiva.Services;

namespace Autiva.Pages;

public partial class AutivaHome : ContentPage
{
    private readonly AutivaDb _db;

    public AutivaHome()
    {
        InitializeComponent();
        // Dienst über den zentralen Locator beziehen (DI)
        _db = ServiceLocator.GetRequiredService<AutivaDb>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Beim Laden der Seite Liste aktualisieren
        await LoadVehiclesAsync();
    }

    private async Task LoadVehiclesAsync()
    {
        try
        {
            var vehicles = await _db.GetVehiclesAsync();

            // Daten an UI binden
            VehiclesCollection.ItemsSource = vehicles;
            VehiclesCountLabel.Text = vehicles?.Count.ToString() ?? "0";
            EmptyLabel.IsVisible = (vehicles == null || vehicles.Count == 0);
            LastUpdateLabel.Text = $"• {DateTime.Now:dd.MM.yyyy HH:mm}";
        }
        catch (Exception ex)
        {
            // Fehlermeldung bei Datenbankproblemen
            await DisplayAlertAsync("Fehler", ex.Message, "OK");
        }
        finally
        {
            // Refresh-Animation beenden
            VehiclesRefresh.IsRefreshing = false;
        }
    }

    private async void OnRefresh(object sender, EventArgs e)
        => await LoadVehiclesAsync();

    private async void OnAddCar(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("AddCarPage");

    private async void OnOpenReports(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("ReportsPage");

    private async void OnOpenVehicle(object sender, EventArgs e)
    {
        // ID aus dem Button-Parameter lesen und an Detailseite übergeben
        if (sender is Button btn && btn.CommandParameter is int id)
            await Shell.Current.GoToAsync($"VehicleDetailsPage?id={id}");
    }

    private async void OnStartCheckForVehicle(object sender, EventArgs e)
    {
        // ID für neuen Check-Vorgang übermitteln
        if (sender is Button btn && btn.CommandParameter is int id)
            await Shell.Current.GoToAsync($"AutivaCheckPage?id={id}");
    }
}