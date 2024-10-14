using System.IO.Pipes;

namespace TimeTracker.Worker;

public class TimerTrackerPipe
{
    private const string PipeName = "TimeTrackingPipe";

    public async Task StartPipeServerAsync()
    {
        while (true)
        {
using (var pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                try
                {
                    Console.WriteLine("[Server] En attente de connexion d'un client...");
                    
                    // Attendre la connexion d'un client
                    await pipeServer.WaitForConnectionAsync();

                    Console.WriteLine("[Server] Client connecté.");

                    // Lire la requête du client
                    StreamReader reader = new StreamReader(pipeServer);
                    string request = await reader.ReadLineAsync();
                    Console.WriteLine($"[Server] Reçu: {request}");

                    // Traiter la requête du client
                    string response = HandleRequest(request);

                    // Envoyer une réponse au client
                    StreamWriter writer = new StreamWriter(pipeServer) { AutoFlush = true };
                    await writer.WriteLineAsync(response);
                    await writer.FlushAsync();  // S'assurer que la réponse est bien envoyée
                    Console.WriteLine($"[Server] Réponse envoyée : {response}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"[Server] Erreur : {ex.Message}");
                }
                finally
                {
                    // S'assurer que le pipe est déconnecté proprement
                    if (pipeServer.IsConnected)
                    {
                        pipeServer.Disconnect();
                    }
                }
            }
        }
    }

    // Simule la gestion de la requête (par exemple obtenir le timer actuel)
    private string HandleRequest(string request)
    {
        if (request.StartsWith("start"))
        {
            // Extraire les arguments et démarrer un nouveau timer
            return "Timer démarré avec succès.";
        }
        else if (request == "end")
        {
            // Arrêter le timer actuel
            return "Timer arrêté.";
        }
        else if (request.StartsWith("switch"))
        {
            // Arrêter le timer actuel et démarrer un nouveau timer
            return "Timer changé avec succès.";
        }
        else
        {
            return "Commande inconnue.";
        }
    }
}