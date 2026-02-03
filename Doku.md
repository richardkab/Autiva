# Technische Dokumentation: Autiva App

**Entwickler:** Richard Kabanov  
**System:** .NET 10 / MAUI  
**Status:** Prototyp (in aktiver Verfeinerung)

---

### 1. Motivation und Hintergrund
Während meiner wöchentlichen Fahrzeugchecks im Praktikum wurde mir klar, wie fehleranfällig und unhandlich die Erfassung auf Papier ("Klemmbrett-System") ist. 
**Die Lösung:** Eine mobile Anwendung, die Daten direkt vor Ort validiert und ein standardisiertes PDF-Protokoll erzeugt. Das Projekt startete als "Autocheck Up" und wird nun unter dem Namen **Autiva** weiterentwickelt.

### 2. Software-Architektur (MVVM)
Um den Code wartbar zu halten, folgt die App dem **MVVM-Muster** (Model-View-ViewModel). Dies ermöglicht eine saubere Trennung zwischen der Programmlogik und der grafischen Oberfläche:

* **Models:** Klassen wie `Vehicle.cs` und `CheckReport.cs` definieren die Datenstruktur.
* **Views (Pages):** XAML-Dateien steuern das Layout für Android und Windows.
* **Services:** Die Klasse `AutivaDb.cs` kapselt die gesamte Datenbank-Logik.

### 3. Datenfluss und Persistenz
Die App nutzt **SQLite** (`sqlite-net-pcl`) für die lokale Datenspeicherung. Dies ist entscheidend, da in Werkstätten oder auf Parkplätzen oft keine stabile Internetverbindung besteht.

**Datenverarbeitung:**
1. Der Nutzer gibt Daten in der `AddCarPage` oder `AutivaCheckPage` ein.
2. Die Daten werden validiert und über den `ServiceLocator` an die Datenbank-Klasse übergeben.
3. Die Speicherung erfolgt **asynchron**, damit die App während des Schreibvorgangs nicht einfriert.



### 4. PDF-Generierung (Reporting)
Ein zentraler Bestandteil ist der Export der Prüfergebnisse. Hierfür wird die **Syncfusion .NET MAUI Library** verwendet.
* Die App liest die aktuellen Properties des `CheckReport`-Objekts aus.
* Es wird ein PDF-Grid erstellt, das die Daten in tabellarischer Form anordnet.
* Das fertige Dokument wird im lokalen Dateisystem des Endgeräts abgelegt.

### 5. Reflexion der Entwicklung (Ehrlich)
Da dies mein erstes größeres Projekt mit .NET MAUI ist, standen zwei Herausforderungen im Fokus:
1.  **Asynchrone Programmierung:** Sicherzustellen, dass Datenbankzugriffe die UI nicht blockieren.
2.  **PDF-Layout:** Die korrekte Positionierung der Elemente im generierten Dokument.

Bei diesen komplexen Themen habe ich intensiv mit Dokumentationen gearbeitet und gezielt KI-Tools zur Fehlersuche und zum Verständnis von Syntax-Problemen eingesetzt. 
Mein Ziel war es nicht, fertigen Code zu kopieren, sondern die dahinterliegende Logik zu verstehen.

### 6. Ausblick
Das Projekt ist ein "Living Project". Aktuell arbeite ich an:
* Optimierung der Benutzeroberfläche für verschiedene Bildschirmgrößen.
* Erweiterung der Validierungslogik bei der Dateneingabe.
