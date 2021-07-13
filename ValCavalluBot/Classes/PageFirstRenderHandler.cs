using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValCavalluBot.Classes
{
    public static class PageFirstRenderHandler
    {
        private static Dictionary<string, bool> _firstRenderPages = new();
        private static string _moduleName;
         
        public static bool IsFirstRender(this string pageName)
        {
            return !_firstRenderPages[pageName];
        }

        public static void PageFirstRender(string pageName)
        {
            if (!_firstRenderPages.ContainsKey(pageName))
            {
                _firstRenderPages.Add(pageName, true);
            }
        }
    }
}
