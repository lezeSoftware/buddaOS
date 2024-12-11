using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

//Zwei Klassen Command und CommandManager
namespace Projekt_Betriebssystem
{
    //Deklarierung wie ein Command aufgebaut sein muss (Template)
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract string NameAlias { get; }
        public abstract string Description { get; }
        public abstract void Execute(string[] args);
    }

    //für die Aufrufe der Commands
    public class CommandManager
    {
        private List<Command> commands = new List<Command>();

        public void RegisterCommand(Command command)
        {
            commands.Add(command);
        }

        public void ExecuteCommand(string input)
        {
            string[] parts = input.Split(' ');
            string commandName = parts[0];  //Command
            string[] args = parts.Skip(1).ToArray();  //Argumente beginnend bei 0

            Command command = commands.FirstOrDefault(c => c.Name == commandName); //Funktion sucht das erste Objekt mit dem Namen:commandName

            if (command != null)
            {
                command.Execute(args);
            }
            else
            {
                Command commandalias = commands.FirstOrDefault(c => c.NameAlias == commandName);
                if (commandalias != null)
                {
                    commandalias.Execute(args);
                }
                else
                {
                    Console.WriteLine("Unbekannter Befehl. Geben Sie 'help' ein fuer eine Liste der Befehle.");
                }
            }
        }

        public void ShowHelp()
        {
            Console.WriteLine("Verfuegbare Befehle:");
            foreach (var command in commands)
            {
                Console.WriteLine($"{command.Name} ({command.NameAlias}) - {command.Description}");
            }
        }

        public string Complete(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var matchingCommands = commands
                .Where(c => c.Name.StartsWith(input, StringComparison.OrdinalIgnoreCase) || c.NameAlias.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Name)
                .ToList();

            if (matchingCommands.Count == 1)
            {
                //eindeutig
                return matchingCommands[0];
            }
            else if (matchingCommands.Count > 1)
            {
                Console.WriteLine();
                Console.WriteLine("Bekannte Befehle:");
                foreach (var cmd in matchingCommands)
                {
                    Console.WriteLine(cmd);
                }

                UI.PrintCurrentPosition();
                return null; 
            }
            //keine Übereinstimmung
            return input;
        }
    }

    //Für jeden Command eine eigene Klasse abgeleitet von Command
    //Commands müssen im Kernel deklariert werden
    public class HelpCommand : Command
    {
        private CommandManager commandManager;

        public HelpCommand(CommandManager commandManager)
        {
            this.commandManager = commandManager;
        }

        public override string Name => "help";
        public override string NameAlias => "h";
        public override string Description => "Zeigt eine Liste aller Befehle an.";

        public override void Execute(string[] args)
        {
            commandManager.ShowHelp();
        }
    }

    public class EchoCommand : Command
    {
        public override string Name => "echo";
        public override string NameAlias => "e";
        public override string Description => "Gibt die eingegebenen Argumente aus.";

        public override void Execute(string[] args)
        {
            Console.WriteLine(string.Join(" ", args));
        }
    }

    public class LogoutCommand : Command
    {
        private Kernel kernel;

        public LogoutCommand(Kernel kernel)
        {
            this.kernel = kernel;
        }

        public override string Name => "logout";
        public override string NameAlias => "lo";
        public override string Description => "Meldet den Benutzer ab.";

        public override void Execute(string[] args)
        {
            Console.WriteLine("Sie wurden erfolgreich abgemeldet.");
            Thread.Sleep(2000);
            UI.Init();
            this.kernel.Logout();
        }
    }

    public class ShutdownCommand : Command
    {
        public override string Name => "shutdown";

        public override string NameAlias => "sd";
        public override string Description => "Faehrt das System herunter.";

        public override void Execute(string[] args)
        {
            Console.WriteLine("Das System wird heruntergefahren...");
            Dateisystem.SavePermissions();
            Kernel.loginManager.SaveUsers();
            Thread.Sleep(500);

            Console.Clear();
            Console.WriteLine("\n\n\n\n___________       \r\n\\_   _____/ ______\r\n |    __)_ /  ___/\r\n |        \\\\___ \\ \r\n/_______  /____  >\r\n        \\/     \\/ ");
            Thread.Sleep(1000);
            Console.Clear();
            Console.WriteLine("\n\n\n\n___________        .__            __   \r\n\\_   _____/ ______ |  |__ _____ _/  |_ \r\n |    __)_ /  ___/ |  |  \\\\__  \\\\   __\\\r\n |        \\\\___ \\  |   Y  \\/ __ \\|  |  \r\n/_______  /____  > |___|  (____  /__|  \r\n        \\/     \\/       \\/     \\/    ");
            Thread.Sleep(1000);
            Console.Clear();
            Console.WriteLine("\n\n\n\n___________        .__            __           .__       .__     \r\n\\_   _____/ ______ |  |__ _____ _/  |_    _____|__| ____ |  |__  \r\n |    __)_ /  ___/ |  |  \\\\__  \\\\   __\\  /  ___/  |/ ___\\|  |  \\ \r\n |        \\\\___ \\  |   Y  \\/ __ \\|  |    \\___ \\|  \\  \\___|   Y  \\\r\n/_______  /____  > |___|  (____  /__|   /____  >__|\\___  >___|  /\r\n        \\/     \\/       \\/     \\/            \\/        \\/     \\/");
            Thread.Sleep(1000);
            Console.Clear();
            Console.WriteLine("\n\n\n\n___________        .__            __           .__       .__     \r\n\\_   _____/ ______ |  |__ _____ _/  |_    _____|__| ____ |  |__  \r\n |    __)_ /  ___/ |  |  \\\\__  \\\\   __\\  /  ___/  |/ ___\\|  |  \\ \r\n |        \\\\___ \\  |   Y  \\/ __ \\|  |    \\___ \\|  \\  \\___|   Y  \\\r\n/_______  /____  > |___|  (____  /__|   /____  >__|\\___  >___|  /\r\n        \\/     \\/       \\/     \\/            \\/        \\/     \\/ \r\n                                 ___.             .___  .___           .___\r\n_____   __ __  ______ ____   ____\\_ |__  __ __  __| _/__| _/____     __| _/\r\n\\__  \\ |  |  \\/  ___// ___\\_/ __ \\| __ \\|  |  \\/ __ |/ __ |\\__  \\   / __ | \r\n / __ \\|  |  /\\___ \\/ /_/  >  ___/| \\_\\ \\  |  / /_/ / /_/ | / __ \\_/ /_/ | \r\n(____  /____//____  >___  / \\___  >___  /____/\\____ \\____ |(____  /\\____ | \r\n     \\/           \\/_____/      \\/    \\/           \\/    \\/     \\/      \\/ ");

            Thread.Sleep(3000);

            Cosmos.System.Power.Shutdown();
        }
    }

    public class CreateFileCommand : Command
    {
        public override string Name => "create";
        public override string NameAlias => "c";
        public override string Description => "Erstellt eine neue Datei.";

        public override void Execute(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Bitte geben Sie einen Dateinamen und eine Berechtigung (1 =Admin, 2 = Employee, 3 = Guest) an.");
                return;
            }

            string fileName = args[0];
            if (!int.TryParse(args[1], out int permission) || permission < 1 || permission > 3)
            {
                Console.WriteLine("Ungueltige Berechtigung. Erlaubt sind 1 (Admin), 2 (Employee) oder 3 (Guest).");
                return;
            }

            int userPermission = Kernel.loginManager.GetUserPermission();

            Dateisystem.CreateNewFile(fileName, permission, userPermission);
        }
    }

    public class ReadFileCommand : Command
    {
        public override string Name => "read";

        public override string NameAlias => "r";
        public override string Description => "Liest den Inhalt einer Datei.";

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Bitte geben Sie einen Dateinamen an.");
                return;
            }

            string fileName = args[0];
            int userPermission = Kernel.loginManager.GetUserPermission();

            Dateisystem.ReadSpecificFile(fileName, userPermission);
        }
    }

    public class DeleteFileCommand : Command
    {
        public override string Name => "delete";

        public override string NameAlias => "del";
        public override string Description => "Loescht eine Datei.";

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Bitte geben Sie einen Dateinamen an.");
                return;
            }

            string fileName = args[0];
            int userPermission = Kernel.loginManager.GetUserPermission();

            Dateisystem.DeleteFile(fileName, userPermission);
        }
    }

    public class EditFileCommand : Command
    {
        public override string Name => "edit";
        public override string NameAlias => "ed";
        public override string Description => "Bearbeitet den Inhalt einer Datei.";

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Bitte geben Sie den Namen der Datei an.");
                return;
            }

            string fileName = args[0];
            int userPermission = Kernel.loginManager.GetUserPermission();

            Dateisystem.FileEditor(fileName, userPermission);
        }
    }


    public class ListCommand : Command
    {
        public override string Name => "list";

        public override string NameAlias => "ls";
        public override string Description => "Listet alle Dateien im aktuellen Verzeichnis auf.";

        public override void Execute(string[] args)
        {
            int userPermission = Kernel.loginManager.GetUserPermission();
            Dateisystem.ListFiles(userPermission);
        }
    }

    public class ListUserCommand : Command
    {
        public override string Name => "listuser";
        public override string NameAlias => "lu";
        public override string Description => "Zeigt eine Liste aller Benutzer an.";

        public override void Execute(string[] args)
        {
            Kernel.loginManager.ListUsers();
        }
    }

    public class AddUserCommand : Command
    {
        public override string Name => "adduser";
        public override string NameAlias => "au";
        public override string Description => "Erstellt einen neuen Benutzer.";

        public override void Execute(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Verwendung: adduser <Benutzername> <Passwort> <Berechtigung>");
                return;
            }

            string username = args[0];
            string password = args[1];
            if (!int.TryParse(args[2], out int permission) || permission < 1 || permission > 3)
            {
                Console.WriteLine("Ungueltige Berechtigung. Erlaubt sind 1 (Admin), 2 (Employee) oder 3 (Guest).");
                return;
            }

            Kernel.loginManager.AddUser(username, password, permission);
        }
    }

    public class DeleteUserCommand : Command
    {
        public override string Name => "deleteuser";
        public override string NameAlias => "du";
        public override string Description => "Loescht einen Benutzer.";

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Verwendung: deleteuser <Benutzername>");
                return;
            }

            string username = args[0];
            Kernel.loginManager.DeleteUser(username);
        }
    }

    public class ChangePasswordCommand : Command
    {
        public override string Name => "changepw";
        public override string NameAlias => "cpw";
        public override string Description => "Aendert das Passwort eines Benutzers.";

        public override void Execute(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Verwendung: changepw <Benutzername> <Neues Passwort>");
                return;
            }

            string username = args[0];
            string newPassword = args[1];
            Kernel.loginManager.ChangePassword(username, newPassword);
        }
    }

    public class ChangePermissionCommand : Command
    {
        public override string Name => "changeperm";
        public override string NameAlias => "cperm";
        public override string Description => "Aendert die Berechtigung eines Benutzers.";

        public override void Execute(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Verwendung: changeperm <Benutzername> <Neue Berechtigung>");
                return;
            }

            string username = args[0];
            if (!int.TryParse(args[1], out int newPermission) || newPermission < 1 || newPermission > 3)
            {
                Console.WriteLine("Ungueltige Berechtigung. Erlaubt sind 1 (Admin), 2 (Employee) oder 3 (Guest).");
                return;
            }

            Kernel.loginManager.ChangePermission(username, newPermission);
        }
    }

    public class UptimeCommand : Command
    {
        public override string Name => "uptime";
        public override string NameAlias => "ut";
        public override string Description => "Zeigt die Laufzeit des Systems seit dem Start.";

        public override void Execute(string[] args)
        {
            TimeSpan uptime = Kernel.GetUptime();

            Console.WriteLine($"Das System laeuft seit: {uptime.Days} Tagen, {uptime.Hours} Stunden, {uptime.Minutes} Minuten und {uptime.Seconds} Sekunden.");
        }
    }
}
