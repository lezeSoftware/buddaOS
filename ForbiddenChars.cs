using System;

namespace Projekt_Betriebssystem
{
    internal static class ForbiddenChars
    {
        private static char[] forbiddenChars = new char[] //Zeichen ergänzen
        {
            '#',
            '+',
            '*',
            '´',
            '?',
            '!',
            '"',
            '^',
            '§',
            '$',
            '%',
            '&',
            '/',
            '(',
            '[',
            ')',
            ']',
            '=',
            '{',
            '}',
            '\\',
            '`',
            '´',
            '+',
            '~',
            'ä',
            'ü',
            'ö',
            'Ä',
            'Ü',
            'Ö'
        };

        public static bool Check(string txt) //Gibt false zurück wenn ein nichterlaubtes Zeichen in dem mitgegebenen String ist
        {
            foreach (char c in forbiddenChars)
            {
                if (txt.Contains(c))
                {
                    Console.WriteLine("Ungueltiges Zeichen: " + c);
                    return false;
                }
            }
            return true;
        }
    }
}
