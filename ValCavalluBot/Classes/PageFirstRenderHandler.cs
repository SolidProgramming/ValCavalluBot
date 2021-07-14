using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValCavalluBot.Classes
{
    public static class PageFirstRenderHandler
    {
        private static List<string> _firstRenderPages = new();
        private static string _moduleName;
         
        public static bool IsFirstRender(this string pageName)
        {
            if (_firstRenderPages.Contains(pageName))
            {
                return false;
            }

            return true;
        }

        public static void PageFirstRender(string pageName)
        {
            if (!_firstRenderPages.Contains(pageName))
            {
                _firstRenderPages.Add(pageName);
            }
        }
    }
}
