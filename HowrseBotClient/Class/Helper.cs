using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HowrseBotClient.Model;

namespace HowrseBotClient.Class
{
    public static class Helper
    {
        public static ButtonClickCoordinationsModel GetRandomClickCoordsTakingCareButton()
        {
            Random rnd = new();
            int xClickCoord = rnd.Next(10, 80);
            int yClickCoord = rnd.Next(10, 72);

            return new ButtonClickCoordinationsModel { X = xClickCoord, Y = yClickCoord };
        }
    }
}
