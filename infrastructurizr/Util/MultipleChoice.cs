using System;

namespace infrastructurizr.Util
{
    public class MultipleChoice
    {
        public static string Ask(string defaultValue = null, params string[] options)
        {
            const int startX = 0;
            var startY = Console.CursorTop;
            const int optionsPerLine = 1;
            const int spacingPerLine = 14;

            int currentSelection = 0;

            ConsoleKey key;
            Console.CursorVisible = false;

            do
            {
                for (int i = 0; i < options.Length; i++)
                {
                    Console.SetCursorPosition(startX + (i % optionsPerLine) * spacingPerLine, startY + i / optionsPerLine);
                    Console.Write((i == currentSelection ? "(x)" : "( )") + " " + options[i]);
                }

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        {
                            if (currentSelection % optionsPerLine > 0)
                                currentSelection--;
                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            if (currentSelection % optionsPerLine < optionsPerLine - 1)
                                currentSelection++;
                            break;
                        }
                    case ConsoleKey.UpArrow:
                        {
                            if (currentSelection >= optionsPerLine)
                                currentSelection -= optionsPerLine;
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (currentSelection + optionsPerLine < options.Length)
                                currentSelection += optionsPerLine;
                            break;
                        }
                    case ConsoleKey.Escape:
                        {
                            Leave(options, startY);
                            return defaultValue;
                        }
                }
            } while (key != ConsoleKey.Enter);


            Leave(options, startY);
            return options[currentSelection];
        }

        private static void Leave(string[] options, int startY)
        {
            for (int i = 0; i < options.Length; i++)
            {
                Console.SetCursorPosition(0, startY + i);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, startY);
            Console.CursorVisible = true;
        }
    }
}