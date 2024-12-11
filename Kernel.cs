using System;
using Sys = Cosmos.System;

namespace Projekt_Betriebssystem
{
    //Main Klasse
    public class Kernel : Sys.Kernel
    {
        //Initialisiert den LoginManager und den CommandHandler
        public static CommandManager cmdHandler = new CommandManager();
        public static LoginManager loginManager = new LoginManager();

        //Schafft Platz für die Startzeit
        private static DateTime momentOfStart;

        //Initialisiert das Dateisystem
        public static Sys.FileSystem.CosmosVFS fs = new Cosmos.System.FileSystem.CosmosVFS();
        public static string currentPath = @"0:\"; //Startdirectory im Cosmos

        //Initialisiert den HistoryManager um eingegebene Befehle zu speichern
        private CommandHistoryManager historyManager;

        //Vorbereitung des Systems
        protected override void BeforeRun()
        {
            //Initialisiert die Startzeit
            momentOfStart = DateTime.Now;

            //Legt CharSet und Keyboard Layout fest
            Sys.KeyboardManager.SetKeyLayout(new Cosmos.System.ScanMaps.DE_Standard());
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //Baut den Inhalt der Konsole auf und bereitet alles wichtige vor
            UI.Init();
            Dateisystem.Init();
            Dateisystem.LoadPermissions();
            loginManager.Init();

            cmdHandler = new CommandManager();
            historyManager = new CommandHistoryManager();

            //System Befehle
            cmdHandler.RegisterCommand(new HelpCommand(cmdHandler));    //Hilfe
            cmdHandler.RegisterCommand(new EchoCommand());              //Ausgabe der Kommandozeile 
            cmdHandler.RegisterCommand(new ShutdownCommand());          //Herunterfahren & Speichern
            cmdHandler.RegisterCommand(new UptimeCommand());            //Uptime anzeigen
            //Dateiverwaltung
            cmdHandler.RegisterCommand(new CreateFileCommand());        //Datei erstellen
            cmdHandler.RegisterCommand(new ReadFileCommand());          //Datei lesen
            cmdHandler.RegisterCommand(new EditFileCommand());          //Datei bearbeiten
            cmdHandler.RegisterCommand(new DeleteFileCommand());        //Datei löschen
            cmdHandler.RegisterCommand(new ListCommand());              //Existierende Dateien auflisten
            //Userverwaltung
            cmdHandler.RegisterCommand(new ListUserCommand());          //Liste aller existierenden User
            cmdHandler.RegisterCommand(new AddUserCommand());           //Neuen Nutzer erstellen
            cmdHandler.RegisterCommand(new DeleteUserCommand());        //User löschen
            cmdHandler.RegisterCommand(new ChangePasswordCommand());    //Passwort ändern
            cmdHandler.RegisterCommand(new ChangePermissionCommand());  //Permission ändern
            cmdHandler.RegisterCommand(new LogoutCommand(this));        //Ausloggen 
        }

        //Loop --> eigentliche Funktion des Systems
        protected override void Run()
        {
            loginManager.Login(); //Abfrage ob Benutzer angemeldet --> Login entsprechend ausführen oder überspringen
            UI.PrintCurrentPosition();

            //Eingaben werden verwaltet
            string input = historyManager.ReadLineWithHistory();
            cmdHandler.ExecuteCommand(input); 
        }

        //Führt den Logout aus
        public void Logout()
        {
            loginManager.Logout();
        }

        //Gibt die Runtime des Systems aus
        public static TimeSpan GetUptime()
        {
            return DateTime.Now - momentOfStart;
        }
    }
}
