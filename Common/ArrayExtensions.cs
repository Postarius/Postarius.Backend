using System;

namespace Common
{
    public static class ArrayExtensions
    {
        public static string AsString(this byte[] self)
        {
            return Convert.ToBase64String(self);
        }
    }
}