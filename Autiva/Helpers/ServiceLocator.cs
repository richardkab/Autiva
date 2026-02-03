using System;

namespace Autiva.Helpers;

public static class ServiceLocator
{
    public static IServiceProvider? Services { get; set; }

    public static T GetRequiredService<T>() where T : notnull
    {
        if (Services == null)
            throw new InvalidOperationException("ServiceLocator.Services ist noch nicht gesetzt. Prüfe MauiProgram.");

        var svc = Services.GetService(typeof(T));
        if (svc is not T typed)
            throw new InvalidOperationException($"Service {typeof(T).Name} wurde nicht registriert.");

        return typed;
    }
}
