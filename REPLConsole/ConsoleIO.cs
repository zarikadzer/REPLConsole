namespace REPLConsole
{
    using System;
    using System.Diagnostics;
    using System.IO;

    internal class ConsoleIO
    {
        public static readonly ConsoleIO Default = new ConsoleIO(Console.Out, Console.Error, Console.In);

        public TextWriter Error { get; }
        public TextWriter Out { get; }
        public TextReader In { get; }

        public ConsoleIO(TextWriter output, TextWriter error, TextReader input)
        {
            Debug.Assert(output != null);
            Debug.Assert(input != null);

            Out = output;
            Error = error;
            In = input;
        }

        public virtual ConsoleColor ForegroundColor
        {
            set
            {
                Console.ForegroundColor = value;
            }
        }

        public virtual void ResetColor() => Console.ResetColor();

        public virtual void WriteErrorLine(string format, params object[] args)
        {
            WriteColoredLine(Error, ConsoleColor.Red, format, args);
        }

        public virtual void WriteErrorLine(object format)
        {
            WriteColoredLine(Error, ConsoleColor.Red, format);
        }

        public virtual void WriteError(string format, params object[] args)
        {
            WriteColored(Error, ConsoleColor.Red, format, args);
        }

        public virtual void WriteLineInfo(string format, params object[] args)
        {
            WriteColoredLine(Error, ConsoleColor.DarkGreen, format, args);
        }

        public virtual void WriteLineInfo(object format)
        {
            WriteColoredLine(Error, ConsoleColor.DarkGreen, format);
        }

        public virtual void WriteInfo(object format, params object[] args)
        {
            WriteColored(Error, ConsoleColor.DarkGreen, format, args);
        }

        private void WriteColoredLine(TextWriter writer, ConsoleColor color, string format, params object[] args)
        {
            try
            {
                ForegroundColor = color;
                writer.WriteLine(format, args);
            }
            finally
            {
                ResetColor();
            }
        }

        private void WriteColoredLine(TextWriter writer, ConsoleColor color, object format)
        {
            try
            {
                ForegroundColor = color;
                writer.WriteLine(format);
            }
            finally
            {
                ResetColor();
            }
        }

        private void WriteColored(TextWriter writer, ConsoleColor color, object format, params object[] args)
        {
            try
            {
                var assmsCount = System.AppDomain.CurrentDomain.GetAssemblies().Length.ToString().PadRight(5);
                ForegroundColor = color;
                writer.Write(assmsCount + format.ToString(), args);
            }
            finally
            {
                ResetColor();
            }
        }
    }
}
