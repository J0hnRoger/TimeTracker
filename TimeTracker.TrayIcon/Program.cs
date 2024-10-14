namespace TimeTracker.TrayIcon;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Créer et démarrer l'application tray
        using (var trayApp = new TrayApplication())
        {
            Application.Run();
        }
    }
}