using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO.Pipes;

namespace TimeTracker.CommandLine;

class Program
{
    private const string PipeName = "TimeTrackingPipe";

    static async Task<int> Main(string[] args)
    {
        // Créer la commande "start"
        var startCommand = new Command("start", "Lance un timer")
        {
            new Argument<int?>("taskId", "ID de la tâche à associer au timer (optionnel)"),
            new Option<TimeSpan?>("--duration", "Durée du timer"),
            new Option<string>("--end", "Heure de fin du timer (HH:mm)")
        };
        startCommand.Handler = CommandHandler.Create<int?, TimeSpan?, string>(StartTimer);

        // Créer la commande "end"
        var endCommand = new Command("end", "Arrête le timer actif");
        endCommand.Handler = CommandHandler.Create(EndTimer);

        // Créer la commande "switch"
        var switchCommand = new Command("switch", "Arrête le timer actif et démarre un nouveau timer")
        {
            new Argument<int>("taskId", "ID de la tâche à démarrer")
        };
        switchCommand.Handler = CommandHandler.Create<int>(SwitchTimer);

        // Créer la racine de la CLI avec les commandes
        var rootCommand = new RootCommand { startCommand, endCommand, switchCommand };

        return await rootCommand.InvokeAsync(args);
    }

    // Méthode pour lancer un timer
    static async Task StartTimer(int? taskId, TimeSpan? duration, string end)
    {
        string command = "start";

        if (taskId.HasValue)
            command += $" {taskId.Value}";

        if (duration.HasValue)
            command += $" --duration {duration.Value}";

        if (!string.IsNullOrEmpty(end))
            command += $" --end {end}";

        await SendCommandToService(command);
    }

    // Méthode pour arrêter le timer actif
    static async Task EndTimer()
    {
        await SendCommandToService("end");
    }

    // Méthode pour switcher un timer
    static async Task SwitchTimer(int taskId)
    {
        string command = $"switch {taskId}";
        await SendCommandToService(command);
    }

    // Méthode pour envoyer la commande via Named Pipes au service Windows
    static async Task SendCommandToService(string command)
    {
        using var pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

        try
        {
            Console.WriteLine("[Client] Tentative de connexion au serveur...");
            await pipeClient.ConnectAsync();
            Console.WriteLine("[Client] Connecté au serveur.");

            // Envoyer la commande au serveur
            using (StreamWriter writer = new StreamWriter(pipeClient))
            {
                await writer.WriteLineAsync(command);
                await writer.FlushAsync();
                Console.WriteLine($"[Client] Commande envoyée : {command}");
            }

            // Lire la réponse du serveur
            using (StreamReader reader = new(pipeClient, leaveOpen: true))
            {
                string? response = await reader.ReadLineAsync();
                if (response != null)
                {
                    Console.WriteLine($"[Client] Réponse du serveur : {response}");
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[Client] Erreur : {ex.Message}");
        }
        finally
        {
            // S'assurer que le client ferme correctement le pipe
            if (pipeClient.IsConnected)
            {
                Console.WriteLine("[Client] Fermeture du pipe...");
                pipeClient.Close();
            }
        }
    }
}