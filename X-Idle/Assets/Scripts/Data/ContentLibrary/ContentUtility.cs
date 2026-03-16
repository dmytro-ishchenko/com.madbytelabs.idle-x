using System;

namespace Data.ContentLibrary
{
    public static class ContentUtility
    {
        public static string GetId()
        {
            string newId = DateTime.Now.Ticks.ToString("x");
            newId += Guid.NewGuid().ToString().GetHashCode().ToString("x");

            return newId;
        }
    }
}