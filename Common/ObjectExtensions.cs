using System;

namespace Common
{
    public static class ObjectExtensions
    {
        public static int AsInt(this object self)
        {
            return Convert.ToInt32(self);
        }
    }
}