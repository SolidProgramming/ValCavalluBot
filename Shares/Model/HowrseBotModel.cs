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
        public event Action OnLoginSuccessful;
        public event Action<HowrseServerLoginResponseModel> OnLoginFailed;
        public event Action<BotClientStatus> OnBotStatusChanged;

        public BotSettingsModel Settings = new();
        public string Id { get; set; }
        private string HowrseUserId { get; set; }

        private QueModel Que = new();

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
        public async Task Login()
        {
            await Task.Run(() =>
            {
                Status = BotClientStatus.Working;
                string csrf = string.Empty;
                string auth_token = string.Empty;
                string html = string.Empty;

                html = Connection.Get("https://" + Settings.Server + "/");

                //tolower?
                auth_token = Regex.Match(html, "id=\"authentification(.{5})\" type").Groups[1].Value.ToLower();
                csrf = Regex.Match(html, "value=\"(.{32})\" name=").Groups[1].Value.ToLower();

                string serverResponse = Connection.Post("https://" + Settings.Server + "/site/doLogIn", auth_token + "=" + csrf + "&login=" + Settings.Credentials.HowrseUsername + "&password=" + Settings.Credentials.HowrsePassword + "&redirection=&isBoxStyle=");

                HowrseServerLoginResponseModel howrseServerLoginResponse = JsonConvert.DeserializeObject<HowrseServerLoginResponseModel>(serverResponse);

                if (howrseServerLoginResponse.errors.Count > 0)
                {
                    OnLoginFailed(howrseServerLoginResponse);
                    Status = BotClientStatus.Error;
                    return;
                }

                Connection.Get("https://" + Settings.Server + "/jeu/?identification=1&redirectionMobile=yes");
                html = Connection.Get("https://" + Settings.Server + "/jeu/");

                if (html.Contains("Equus"))
                {
                    HowrseUserId = Regex.Match(html, "href=\"/joueur/fiche/\\?id=(\\d+)\"><span class").Groups[1].Value;

                    if (html.Contains("/header/logo/vip/"))
                    {
                        Settings.HowrseAccountType = HowrseAccountType.VIP;
                    }
                    else if (html.Contains("/header/logo/pegase"))
                    {

                        Settings.HowrseAccountType = HowrseAccountType.Pegasus;
                    }
                    else
                    {
                        Settings.HowrseAccountType = HowrseAccountType.Normal;
                    }

                    OnLoginSuccessful();
                    Status = BotClientStatus.Idle;
                }
                else
                {
                    OnLoginFailed(new() { errorsText = "Username or Password wrong" });
                    Status = BotClientStatus.Error;
                }
            }).ConfigureAwait(true);
        }

        //TDOD: async
        public void AddToQue(HowrseTaskModel howrseTask)
        {
            Que.AddTask(howrseTask);
        }

        //TDOD: async
        public void WorkOnQue()
        {
            int queTaskCount = Que.Tasks.Count;
            int queStepCount = Que.Horses.Count;

            int runCount = 0;

            //ExitConditionSettingModel botExitCondition = 

        }
    }
}
