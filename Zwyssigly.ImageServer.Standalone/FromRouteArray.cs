using System;

namespace Zwyssigly.ImageServer.Standalone
{
    public class FromRouteArray
    {
        static char[] separators = new[] { ',', ';' };
        public static string[] Parse(string ids)
            => ids?.Split(separators, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
    }
}
