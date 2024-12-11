using System;
using System.Collections.Generic;
using System.IO;
using Sys = Cosmos.System;

namespace Projekt_Betriebssystem
{
    internal static class Dateisystem
    {
        private static Dictionary<string, int> filePermissions = new Dictionary<string, int>();
        private static Sys.FileSystem.CosmosVFS LocalFileSystem = new Sys.FileSystem.CosmosVFS();

        public static void Init()
        {
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(LocalFileSystem);
        }

        //Speichert die Permissions für die Dateien in die System-Datei
        public static void SavePermissions()
        {
            string permissionsFile = Path.Combine(Kernel.currentPath, "permissions.sys");

            try
            {
                using (StreamWriter writer = new StreamWriter(permissionsFile))
                {
                    foreach (var entry in filePermissions)
                    {
                        writer.WriteLine($"{entry.Key}:{entry.Value}");
                    }
                }
                Console.WriteLine("Dateiberechtigungen wurden gespeichert.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern der Dateiberechtigungen: {ex.Message}");
            }
        }

        //Lädt die Permissions aus der System-Datei
        public static void LoadPermissions()
        {
            string permissionsFile = Path.Combine(Kernel.currentPath, "permissions.sys");

            if (!File.Exists(permissionsFile))
            {
                Console.WriteLine("\nKeine gespeicherten Dateiberechtigungen gefunden.");
                return;
            }

            try
            {
                foreach (var line in File.ReadAllLines(permissionsFile))
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int permission))
                    {
                        filePermissions[parts[0]] = permission;
                    }
                }
                Console.WriteLine("\nDateiberechtigungen wurden geladen.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFehler beim Laden der Dateiberechtigungen: {ex.Message}");
            }
        }

        //Gibt den Typ des Filesystems zurück
        public static string GetFileSystemType()
        {
            return LocalFileSystem.GetFileSystemType(Kernel.currentPath);
        }

        //Listet alle verfügabaren Dateien auf, auf die der aktuelle Benutzer Zugriff hat
        public static void ListFiles(int currentPermission)
        {
            Console.WriteLine("Verfuegbare Dateien:");
            foreach (var file in filePermissions)
            {
                if (file.Value >= currentPermission && file.Key != "permissions.sys")
                {
                    Console.WriteLine($"{file.Key} (Permission: {file.Value})");
                }
            }
        }

        //Erstellt eine neue Datei und legt Permission fest
        public static bool CreateNewFile(string file, int permission, int currentPermission)
        {
            string fileName = file;

            //Überprüft, ob der Benutzer die Berechtigung hat
            if (permission < currentPermission)
            {
                Console.WriteLine("Sie koennen keine Dateien mit hoeheren Berechtigungen als Ihre eigenen erstellen.");
                return false;
            }

            //Überprüft, ob der Dateiname gültig ist
            if (!ForbiddenChars.Check(fileName))
            {
                Console.WriteLine("Der Dateiname enthaelt ungueltige Zeichen! Datei konnte nicht erstellt werden.");
                return false;
            }

            //Überprüft, ob die Datei auf .txt endet
            if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Datei konnte nicht erstellt werden! Es sind nur .txt-Dateien erlaubt.");
                return false;
            }

            // Überprüft, ob die Datei bereits existiert
            if (File.Exists(Kernel.currentPath + fileName))
            {
                Console.WriteLine("Datei existiert bereits!");
                return false;
            }

            //die Datei wird erstellt und die Permissions werden gespeichert
            try
            {
                File.Create(Kernel.currentPath + fileName).Dispose();
                filePermissions[fileName] = permission; 
                SavePermissions();
                Console.WriteLine("Datei erfolgreich erstellt mit Permission: " + permission);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Erstellen der Datei: {ex.Message}");
                return false;
            }
        }

        //Zum Löschen der File 
        //Prüfung auf Permissions
        public static void DeleteFile(string file, int currentPermission)
        {
            if (!filePermissions.ContainsKey(file) || filePermissions[file] < currentPermission)
            {
                Console.WriteLine("Zugriff verweigert!");
                return;
            }

            try
            {
                File.Delete(Kernel.currentPath + file);
                filePermissions.Remove(file);
                SavePermissions();
                Console.WriteLine("Datei erfolgreich geloescht!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Loeschen der Datei: {ex.Message}");
            }
        }

        public static void FileEditor(string dataFile, int currentPermission)
        {
            string inputPath = Path.Combine(Kernel.currentPath, dataFile);

            //Prüft, ob die Datei existiert
            if (!File.Exists(inputPath))
            {
                Console.WriteLine("Die angegebene Datei existiert nicht.");
                return;
            }

            //Berechtigungen überprüfen
            if (!filePermissions.ContainsKey(dataFile) || filePermissions[dataFile] < currentPermission)
            {
                Console.WriteLine("Zugriff verweigert!");
                return;
            }

            //den Inhalt der Datei lesen
            string fileContent;
            try
            {
                fileContent = File.ReadAllText(inputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Lesen der Datei: {ex.Message}");
                return;
            }

            //Anweisungen und bisherigen Inhalt anzeigen
            Console.WriteLine("Druecken Sie ENTER, um den neuen Inhalt zu speichern.");
            Console.WriteLine("--- Bisheriger Inhalt ---");
            Console.WriteLine(fileContent);
            Console.WriteLine("--- Neuer Inhalt: ---");

            //Benutzer darf neuen Inhalt schreiben
            Console.Write(">> ");
            string newContent = Console.ReadLine();

            try
            {
                //Datei speichern und beenden
                File.WriteAllText(inputPath, newContent);
                Console.WriteLine("Die Datei wurde erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern der Datei: {ex.Message}");
            }
        }

        //Liest den Inhalt einer Datei aus --> Read Befehl im Command Handler
        public static void ReadSpecificFile(string file, int currentPermission)
        {
            if (!filePermissions.ContainsKey(file) || filePermissions[file] < currentPermission)
            {
                Console.WriteLine("Zugriff verweigert!");
                return;
            }

            try
            {
                Console.WriteLine(File.ReadAllText(Kernel.currentPath + file));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Lesen der Datei: {ex.Message}");
            }
        }
    }
}
