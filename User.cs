using System.Collections.Generic;

namespace Projekt_Betriebssystem
{
    public class User
    {
        public string Username { get; }
        public string Password { get; set; } //Private Setter erlaubt Änderung innerhalb der Klasse
        public int Permission { get; set; } //Auch hier Setter, falls Berechtigungen geändert werden sollen

        private Dictionary<string, int> UserGroups_dict = new Dictionary<string, int>();

        //Konstruktor
        public User(string username, string password, int permission)
        {
            Username = username;
            Password = password;
            Permission = permission;
        }

        //Methode zum Ändern des Passworts (Optional, um die Kontrolle zu behalten)
        public void ChangePassword(string newPassword)
        {
            Password = newPassword;
        }

        public bool IsUserInGroup(string groupname)
        {
            return UserGroups_dict.ContainsKey(groupname);
        }
    }
}
