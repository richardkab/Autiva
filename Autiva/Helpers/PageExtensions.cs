namespace Autiva.Helpers;

/// <summary>
/// Statische Hilfsklasse für Erweiterungsmethoden der MAUI-Page-Klasse.
/// </summary>
public static class PageExtensions
{
    /// <summary>
    /// Eine benutzerdefinierte Bestätigungsabfrage, die das Standard-ActionSheet nutzt.
    /// Erleichtert den Aufruf von Ja/Nein-Dialogen im gesamten Projekt.
    /// </summary>
    /// <param name="page">Die aktuelle Seite (this-Referenz)</param>
    /// <param name="title">Überschrift des Dialogs</param>
    /// <param name="message">Inhalt der Abfrage (wird hier als Titel des Sheets genutzt)</param>
    /// <param name="acceptText">Text für die Bestätigung (Standard: OK)</param>
    /// <param name="cancelText">Text für den Abbruch (Standard: Abbrechen)</param>
    /// <returns>True, wenn der Nutzer die Bestätigung gewählt hat</returns>
    public static async Task<bool> ConfirmAsync(this Page page, string title, string message, string acceptText = "OK", string cancelText = "Abbrechen")
    {
        // Nutzt das native ActionSheet, um eine saubere Auswahl zu erzwingen
        var result = await page.DisplayActionSheetAsync(title, cancelText, null, acceptText);

        // Gibt true zurück, wenn der Nutzer auf den Bestätigungs-Button geklickt hat
        return result == acceptText;
    }
}