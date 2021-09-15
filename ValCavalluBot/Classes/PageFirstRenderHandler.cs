using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace ValCavalluBot.Classes
{
    public static class PageFirstRenderHandler
    {
        private static List<string> _firstRenderPages = new();
                 
        public static bool IsFirstRender(this string componentName)
        {
            if (_firstRenderPages.Contains(componentName))
            {
                return false;
            }

            return true;
        }

        public static void ComponentSetRendered(string pageName)
        {
            if (!_firstRenderPages.Contains(pageName))
            {
                _firstRenderPages.Add(pageName);
            }
        }
    }
}
