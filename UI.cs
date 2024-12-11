using System;

namespace Projekt_Betriebssystem
{
    internal static class UI
    {
        //Print leert und schreibt BuddaOS in Regenbogenfarben in die Konsole
        public static void Init()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" _               _     _       _____ _____ ");

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("| |             | |   | |     |  _  /  ___|");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("| |__  _   _  __| | __| | __ _| | | \\ `--. ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("| '_ \\| | | |/ _` |/ _` |/ _` | | | |`--. \\");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("| |_) | |_| | (_| | (_| | (_| \\ \\_/ /\\__/ /");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("|_.__/ \\__,_|\\__,_|\\__,_|\\__,_|\\___/\\____/ ");

            Console.ResetColor();
        }

        //Schreibt Username in Blau und setzt die Farbe zurück auf Weiß für die Eingabe
        public static void PrintCurrentPosition() 
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{Kernel.loginManager.GetCurrentUser()}@buddaOS:");  
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
