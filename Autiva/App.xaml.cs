namespace Autiva;

/// <summary>
/// Hauptanwendungsklasse. Verwaltet den Lebenszyklus und den Zugriff auf Services.
/// </summary>
public partial class App : Application
{
    // Ermöglicht den statischen Zugriff auf den ServiceProvider (Dependency Injection)
    public static IServiceProvider Services { get; private set; } = default!;

    public App(IServiceProvider services)
    {
        InitializeComponent();
        Services = services;
    }

    /// <summary>
    /// Setzt das Startfenster der Anwendung mit der AppShell (Navigation).
    /// </summary>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}