using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shares.Enum;
using Shares;
using Shares.Model;

namespace Shares.Model
{
    public class HowrseBotModel
    {
        public event Action<BotClientStatus> OnBotStatusChanged;
        public event Action<BotClientCurrentAction> OnBotCurrentActionChanged;

        public Connection OwlientConnection = new();

        public BotSettingsModel Settings = new();
        public HTMLActionsModel HTMLActions = new();

        public string Id { get; set; }        
        public string SID { get; set; }

        private BotClientCurrentAction _CurrentAction = BotClientCurrentAction.Keine;
        private BotClientStatus _Status;
        public BotClientStatus Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
                OnBotStatusChanged(_Status);                
            }
        }        
        public BotClientCurrentAction CurrentAction
        {
            get
            {
                return _CurrentAction;
            }
            set
            {
                _CurrentAction = value;
                OnBotCurrentActionChanged(_CurrentAction);
            }
        }
    }
}
