using System.IO.Pipes;
using System.Timers;
using Timer = System.Timers.Timer;

namespace TimeTracker.TrayIcon
{
    public class TrayApplication : IDisposable
    {
        private NotifyIcon _trayIcon;
        private Timer _timer;
        private int _elapsedTime;

        public TrayApplication()
        {
            // Initialiser l'icône dans le tray
            _trayIcon = new NotifyIcon()
            {
                Icon = SystemIcons.Application, // Icône par défaut, tu peux utiliser une autre icône
                Visible = true,
                Text = "Time Tracker - 0s"
            };

            // Ajouter un menu contextuel
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Changer de tâche", null, OnSwitchTask);
            contextMenu.Items.Add("Quitter", null, OnExit);
            _trayIcon.ContextMenuStrip = contextMenu;

            // Configurer le timer
            _timer = new Timer(1000); // Intervalle de 1 seconde (1000 ms)
            _timer.Elapsed += OnTimerTick;
            _timer.Start(); // Démarrer le timer

            // Initialiser le temps écoulé
            _elapsedTime = 0;
        }

        private async void OnSwitchTask(object sender, EventArgs e)
        {
            // Appeler le service pour changer la tâche via Named Pipe
            string response = await SendMessageToPipeAsync("SwitchTask");
            MessageBox.Show($"Service response: {response}");
        }

        private async Task<string> SendMessageToPipeAsync(string message)
        {
            try
            {
                using (var client = new NamedPipeClientStream(".", "TimeTrackingPipe", PipeDirection.InOut,
                           PipeOptions.Asynchronous))
                {
                    await client.ConnectAsync();

                    // Envoyer un message au service
                    using (StreamWriter writer = new StreamWriter(client) {AutoFlush = true})
                    {
                        await writer.WriteLineAsync(message);
                    }

                    // Lire la réponse
                    using (StreamReader reader = new StreamReader(client))
                    {
                        return await reader.ReadLineAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la communication avec le service: {ex.Message}");
                return string.Empty;
            }
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            // Incrémenter le temps écoulé
            _elapsedTime++;

            // Mettre à jour le texte de l'icône tray
            _trayIcon.Text = $"Time Tracker - {_elapsedTime}s";

            // Afficher un message ballon toutes les 10 secondes pour le fun
            if (_elapsedTime % 10 == 0)
            {
                _trayIcon.ShowBalloonTip(1000, "Time Tracker", $"Temps écoulé : {_elapsedTime} secondes",
                    ToolTipIcon.Info);
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            // Arrêter le timer et masquer l'icône tray avant de quitter
            _timer.Stop();
            _trayIcon.Visible = false;
            Application.Exit();
        }

        public void Dispose()
        {
            _trayIcon.Dispose();
            _timer.Dispose();
        }
    }
}