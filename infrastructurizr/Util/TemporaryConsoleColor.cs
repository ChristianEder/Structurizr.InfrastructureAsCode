using System;

namespace infrastructurizr.Util
{
    public class TemporaryConsoleColor : IDisposable
    {
        private readonly ConsoleColor _before;

        public TemporaryConsoleColor(ConsoleColor color)
        {
            _before = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        public void Dispose()
        {
            Console.ForegroundColor = _before;
        }
    }
}