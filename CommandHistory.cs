using Projekt_Betriebssystem;
using System;
using System.Collections.Generic;

public class CommandHistoryManager
{
    private readonly List<string> commandHistory = new List<string>();
    private int historyIndex = -1;

    //Fügt einen Command zur nach der Ausführung zur commandHistory hinzu
    public void AddCommandToHistory(string command)
    {
        if (!string.IsNullOrEmpty(command))
        {
            commandHistory.Add(command);
            historyIndex = commandHistory.Count;
        }
    }

    //Liest den Input einer Zeile und sorgt für das History-Handling mit den Pfeil-Tasten
    //Verwaltet auch die Vervollständigung mit der Tab-Taste
    public string ReadLineWithHistory()
    {
        string input = "";
        historyIndex = commandHistory.Count;

        while (true)
        {
            var keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                AddCommandToHistory(input);
                return input;
            }

            else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input.Remove(input.Length - 1); 

                Console.SetCursorPosition(0, Console.CursorTop);
                UI.PrintCurrentPosition();  
                Console.Write(input + " "); 
                string dummy = $"{Kernel.loginManager.GetCurrentUser()}@buddaOS:";
                Console.SetCursorPosition(dummy.Length + input.Length, Console.CursorTop); 
            }

            //Zum navigieren
            else if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                if (historyIndex > 0)
                {
                    historyIndex--;
                    DisplayHistoryInput(ref input);
                }
            }
            //Auch zum navigieren
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                if (historyIndex < commandHistory.Count - 1)
                {
                    historyIndex++;
                    DisplayHistoryInput(ref input);
                }
                else
                {
                    historyIndex = commandHistory.Count;
                    ClearCurrentLine(ref input);
                    UI.PrintCurrentPosition();
                }
            }

            //Zum Vervollständigen
            else if (keyInfo.Key == ConsoleKey.Tab)
            {
                string completedInput = Kernel.cmdHandler.Complete(input);

                //Nicht eindeutige Vervollständigung wurde gefunden (mehrer passende Möglichkeiten)
                if (completedInput == null)
                {
                    input = "";
                }
                //Eindeutige Vervollständigung wurde gefunden
                else if (completedInput != input)
                {
                    ClearCurrentLine(ref input);  
                    input = completedInput;       
                    UI.PrintCurrentPosition();    
                    Console.Write(input);         
                }
            }

            //Wenn keine Steuerungstaste gedrückt wurde
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                input += keyInfo.KeyChar;
                Console.Write(keyInfo.KeyChar);
            }
        }
    }

    //Zeigt einen History-Command an
    private void DisplayHistoryInput(ref string input)
    {
        ClearCurrentLine(ref input);
        input = commandHistory[historyIndex];

        UI.PrintCurrentPosition();
        Console.Write(input);
    }

    //Löscht die aktuelle Zeile
    private void ClearCurrentLine(ref string input)
    {
        Console.SetCursorPosition(0, Console.CursorTop); 
        Console.Write(new string(' ', Console.WindowWidth - 1)); 
        Console.SetCursorPosition(0, Console.CursorTop);
        input = "";
    }
}
