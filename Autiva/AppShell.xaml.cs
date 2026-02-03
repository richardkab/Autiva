using Autiva.Pages;

namespace Autiva;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Explizite Registrierung der Unterseiten für die Navigation via Shell.Current.GoToAsync
        // Das ermöglicht das Übergeben von Parametern (wie die Fahrzeug-ID) zwischen den Seiten.
        Routing.RegisterRoute(nameof(AddCarPage), typeof(AddCarPage));
        Routing.RegisterRoute(nameof(ReportsPage), typeof(ReportsPage));
        Routing.RegisterRoute(nameof(AutivaCheckPage), typeof(AutivaCheckPage));
        Routing.RegisterRoute(nameof(VehicleDetailsPage), typeof(VehicleDetailsPage));
    }
}