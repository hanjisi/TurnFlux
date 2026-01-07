using System;
using System.Buffers;
using TurnFlux.Frame;

namespace TurnFlux.Logging
{
    public class ConsoleFluxLogger : IFluxLogger
    {
        public void Log(string message, FluxLogLevel level = FluxLogLevel.Info)
        {
            ConsoleColor color;
            switch (level)
            {
                case FluxLogLevel.Trace:
                    color = ConsoleColor.DarkGray;
                    break;
                case FluxLogLevel.Debug:
                    color = ConsoleColor.Gray;
                    break;
                case FluxLogLevel.Info:
                    color = ConsoleColor.White;
                    break;
                case FluxLogLevel.Warning:
                    color = ConsoleColor.Yellow;
                    break;
                case FluxLogLevel.Error:
                    color = ConsoleColor.Red;
                    break;
                default:
                    color = ConsoleColor.Gray;
                    break;
            }

            WriteToConsole(message, level.ToString().ToUpper(), color);
        }

        public void TraceFrame<TFrame>(FluxDirection direction, TFrame frame) where TFrame : IFluxFrame
        {
            string dirStr;
            ConsoleColor color;
            switch (direction)
            {
                case FluxDirection.Push:
                    dirStr = ">>> PUSH";
                    color = ConsoleColor.Cyan;
                    break;
                case FluxDirection.Receive:
                    dirStr = "<<< RECV";
                    color = ConsoleColor.Green;
                    break;
                default:
                    dirStr = "UNK";
                    color = ConsoleColor.Gray;
                    break;
            }

            var hex = BitConverter.ToString(frame.Payload.ToArray()).Replace("-", " ");
            var message = $"{dirStr} | {hex}";

            WriteToConsole(message, "FRAME", color);
        }


        public void TraceTurn(string token, bool isSuccess, TimeSpan duration)
        {
            var color = isSuccess ? ConsoleColor.Magenta : ConsoleColor.Red;
            var status = isSuccess ? "SUCCESS" : "FAILED";
            var message = $"[TURN] {token} | {status} | Latency: {duration.TotalMilliseconds:F2}ms";

            WriteToConsole(message, "TRANS", color);
        }

        private void WriteToConsole(string message, string tag, ConsoleColor color)
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now:HH:mm:ss.fff}] ");

            Console.ForegroundColor = color;
            Console.WriteLine($"[{tag}] {message}");

            Console.ForegroundColor = originalColor;
        }
    }
}
