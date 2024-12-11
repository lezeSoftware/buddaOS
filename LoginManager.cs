using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Projekt_Betriebssystem
{
    public class LoginManager
    {
        private List<User> users = new List<User>();  //Liste von User-Objekten
        public User currentUser;  //Speichert den aktuell eingeloggten Benutzer

        //Initialisiert die Benutzerkonfiguration
        public void Init()
        {
            LoadUsers(); //Lädt Benutzer aus Datei (users.sys)
            if (users.Count == 0)
            {
                //Wenn keine Benutzer existieren, füge einen Standard-Admin hinzu
                users.Add(new User("Admin", "Admin123", 1));
                SaveUsers(); //Speichert den Standard-Admin
            }
        }

        //Wenn kein Benutzer angemeldet ist, muss sich angemeldet werden.
        //Wenn ein Benutzer angemeldet ist, verlasse die Methode.
        public void Login()
        {
            if (currentUser != null) //Benutzer ist angemeldet --> kein Login
            {
                return;
            }

            string usernameInput;
            string passwordInput;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\nBenutzername: ");
            Console.ForegroundColor = ConsoleColor.White;
            usernameInput = Console.ReadLine();

            //Prüft, ob der Benutzer existiert
            User user = FindUserByUsername(usernameInput);
            //Wenn der Benutzer nicht existiert
            if (user == null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Angegebener Nutzer existiert nicht!\nBitte erneut versuchen.");
                Login();
                return;
            }

            //Wenn Benutzer existiert, frage nach dem Passwort
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Passwort: ");
            Console.ForegroundColor = ConsoleColor.White;
            passwordInput = ReadPassword();

            //Prüft das Passwort
            //Wenn das eingegebene Passwort falsch ist
            if (user.Password != passwordInput)
            {
                Console.WriteLine("Passwort falsch!\nBitte erneut versuchen.");
                Login();
                return;
            }
            //Wenn das eingegebene Passwort richtig ist
            //Login erfolgreich
            currentUser = user;
            Console.WriteLine($"----------\nLogin erfolgreich!\nAngemeldet als {user.Username}!\n----------");
        }

        //Loggt den aktuellen Benutzer aus
        //Rest steht im CommandHandler
        public void Logout()
        {
            currentUser = null;
        }

        //Methode zur Passwort-Eingabe mit Maskierung (*)
        private string ReadPassword()
        {
            //Variablen für das Passwort, das angezeigte Passwort und für die Eingabetasten
            string password = string.Empty;
            string maskedInput = string.Empty;
            ConsoleKeyInfo key;
            
            do
            {
                //Tasteneingabe abfangen ohne das etwas in der Eingabe angezeigt wird
                key = Console.ReadKey(intercept: true);
                
                //Solange kein Backspace und Enter gedrückt werden, füge die Zeichen dem Passwort hinzu und zeige das Passwort versteckt an
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;   
                    maskedInput += "*";     
                    Console.Write("*");     
                }
                //Wenn Backspace gedrückt wird lösche das letzte Zeichen aus dem Passwort und aus dem versteckten Passwort
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);  
                    maskedInput = maskedInput.Substring(0, maskedInput.Length - 1); 

                    Console.SetCursorPosition(0, Console.CursorTop);    
                    Console.ForegroundColor = ConsoleColor.Green;   
                    Console.Write("Passwort: ");     
                    Console.ForegroundColor = ConsoleColor.White;   
                    Console.Write(maskedInput + " ");  
                    Console.SetCursorPosition("Passwort: ".Length + maskedInput.Length, Console.CursorTop);
                }
            }
            //Wiederholung der Passworteingabe bis Enter gedrückt wird
            while (key.Key != ConsoleKey.Enter);   

            Console.WriteLine();

            //Zurück an die Login() Methode
            return password;
        }

        //Sucht ein Benutzer mittels dem Benutzernamen
        private User FindUserByUsername(string username)
        {
            return users.Find(u => u.Username == username);
        }

        //Gibt die Permission des aktuellen Benutzers zurück
        //Wenn keine Permissions dem Benutzer zugeordnet sind, bekommt der Benutzer als Permission die 3
        public int GetUserPermission()
        {
            return currentUser != null ? currentUser.Permission : 3;
            
        }

        //Gibt den Namen des aktuell eingeloggten Benutzers zurück
        //Wenn kein Benutzer als aktueller Benutzer angegeben ist, wird der Benutzer als "Unbekannt" zurückgegeben
        public string GetCurrentUser()
        {
            return currentUser != null ? currentUser.Username : "Unbekannt";
        }

        //Listet alle verfügbaren Benutzer mit Permissions auf, wenn der Benutzer ein Admin (mit Permission: 1) ist
        public void ListUsers()
        {
            if (currentUser.Permission != 1) 
            {
                Console.WriteLine("Zugriff verweigert. Nur Admins duerfen Benutzer verwalten.");
                return;
            }

            Console.WriteLine("Liste aller Benutzer:");
            foreach (var user in users)
            {
                Console.WriteLine($"- {user.Username} (Permission: {user.Permission})");
            }
        }

        //Fügt einen Neuen Benutzer mit Benutzernamen, Passwort und Permission hinzu, wenn der aktuelle Benutzer ein Admin (mit Permission: 1) ist.
        //Prüfung ob Benutzername bereits existiert
        //Prüfung ob Username verbotene Zeichen beinhaltet
        //Prüfung ob Passwort verbotene Zeichen beinhaltet und Prüfung auf Passwort-Richtlinie
        public void AddUser(string username, string password, int permission)
        {
            if (currentUser.Permission != 1)
            {
                Console.WriteLine("Zugriff verweigert. Nur Admins duerfen Benutzer erstellen.");
                return;
            }

            if (users.Any(u => u.Username == username))
            {
                Console.WriteLine($"Ein Benutzer mit dem Namen {username} existiert bereits.");
                return;
            }

            if (!ForbiddenChars.Check(username) || !ForbiddenChars.Check(password))
            {
                Console.WriteLine("Es duerfen keine Sonderzeichen verwendet werden!");
                return;
            }

            if (!IsValidPassword(password))
            {
                Console.WriteLine("Das Passwort muss mindestens 8 Zeichen lang sein und mindestens eine Zahl, einen Grossbuchstaben und einen Kleinbuchstaben enthalten.");
                return;
            }

            users.Add(new User(username, password, permission));
            SaveUsers();
            Console.WriteLine($"Benutzer {username} wurde erfolgreich erstellt.");
        }

        //Entfernt Benutzer
        //Prüfung auf Admin und aktueller Benutzer
        //Prüfung auf Rechte
        public void DeleteUser(string username)
        {
            if (currentUser.Permission != 1)
            {
                Console.WriteLine("Zugriff verweigert. Nur Admins duerfen Benutzer loeschen.");
                return;
            }

            User userToDelete = users.FirstOrDefault(u => u.Username == username);

            if (userToDelete == null)
            {
                Console.WriteLine($"Benutzer {username} existiert nicht.");
                return;
            }

            if (userToDelete.Permission == 1 && userToDelete == currentUser)
            {
                Console.WriteLine("Ein Admin kann sich nicht selbst loeschen.");
                return;
            }

            users.Remove(userToDelete);
            SaveUsers();
            Console.WriteLine($"Benutzer {username} wurde erfolgreich geloescht.");
        }

        //Äjndert das Passwort eines Benutzers, wenn der aktuelle Benutzer ein Admin ist
        public void ChangePassword(string username, string newPassword)
        {
            if (currentUser.Permission != 1)
            {
                Console.WriteLine("Zugriff verweigert. Nur Admins duerfen Passwoerter aendern.");
                return;
            }

            User userToChange = users.FirstOrDefault(u => u.Username == username);

            if (userToChange == null)
            {
                Console.WriteLine($"Benutzer {username} existiert nicht.");
                return;
            }

            if (!IsValidPassword(newPassword))
            {
                Console.WriteLine("Das Passwort muss mindestens 8 Zeichen lang sein und mindestens eine Zahl, einen Grossbuchstaben und einen Kleinbuchstaben enthalten.");
                return;
            }

            userToChange.Password = newPassword;
            SaveUsers();
            Console.WriteLine($"Passwort fuer {username} wurde erfolgreich geaendert.");
        }

        //Ändert Permissions, wenn der aktuelle Benutzer ein Admin ist
        public void ChangePermission(string username, int newPermission)
        {
            if (currentUser.Permission != 1)
            {
                Console.WriteLine("Zugriff verweigert. Nur Admins duerfen Berechtigungen aendern.");
                return;
            }

            User userToChange = users.FirstOrDefault(u => u.Username == username);

            if (userToChange == null)
            {
                Console.WriteLine($"Benutzer {username} existiert nicht.");
                return;
            }

            if (userToChange == currentUser)
            {
                Console.WriteLine("Ein Admin kann seine eigene Berechtigung nicht aendern.");
                return;
            }

            userToChange.Permission = newPermission;
            SaveUsers();
            Console.WriteLine($"Berechtigung fuer {username} wurde erfolgreich geaendert.");
        }

        //Speichert alle Benutzer in einer UserFile, damit die Benutzer beim nächsten Neustart verfügbar sind
        public void SaveUsers()
        {
            string userFile = Path.Combine(Kernel.currentPath, "users.sys");

            try
            {
                using (StreamWriter writer = new StreamWriter(userFile))
                {
                    foreach (var user in users)
                    {
                        writer.WriteLine($"{user.Username}:{user.Password}:{user.Permission}");
                    }
                }
                Console.WriteLine("Benutzer wurden gespeichert.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern der Benutzer: {ex.Message}");
            }
        }

        //Lädt die UserFile beim Start (BeforeRun())
        public void LoadUsers()
        {
            string userFile = Path.Combine(Kernel.currentPath, "users.sys");

            if (!File.Exists(userFile))
            {
                Console.WriteLine("Keine gespeicherten Benutzer gefunden.");
                return;
            }

            try
            {
                users.Clear(); //Alte Liste löschen
                foreach (var line in File.ReadAllLines(userFile))
                {
                    var parts = line.Split(':');
                    if (parts.Length == 3 && int.TryParse(parts[2], out int permission))
                    {
                        users.Add(new User(parts[0], parts[1], permission));
                    }
                }
                Console.WriteLine("Benutzer wurden geladen.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Benutzer: {ex.Message}");
            }
        }

        //Prüft auf Passwort-Richtlinie
        private bool IsValidPassword(string password)
        {
            if (password.Length < 8)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);

            return hasUpper && hasLower && hasDigit;
        }
    }
}
