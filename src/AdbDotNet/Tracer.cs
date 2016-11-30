namespace Vurdalakov
{
    using System;

    internal static class Tracer
    {
        public static void Trace(String format, params Object[] args)
        {
            var message = String.Format(format, args);
            Trace(message);
        }

        public static void Trace(String message)
        {
            System.Diagnostics.Trace.WriteLine(message);
        }
    }
}
