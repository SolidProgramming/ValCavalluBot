using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class HowrseAfterActionResponseModel
    {
        public string ErrorsText { get; set; }
        public string MessageText { get; set; }
        public List<object> Errors { get; set; }
        public object Message { get; set; }
        public Blocks Blocks { get; set; }
        public AddClass AddClass { get; set; }
        public int ChevalSante { get; set; }
        public int ChevalMoral { get; set; }
        public int ChevalEnergie { get; set; }
        public double CoefficientEnergieEndurance { get; set; }
        public double CoefficientEnergievitesse { get; set; }
        public double CoefficientEnergieGalop { get; set; }
        public double CoefficientEnergieTrot { get; set; }
        public double CoefficientEnergieSaut { get; set; }
        public double CoefficientEnergieDressage { get; set; }
        public int ChevalTemps { get; set; }
        public bool HourWarning { get; set; }
        public object ChevalSpecialisation { get; set; }
        public int VarsB1 { get; set; }
        public int VarsB2 { get; set; }
        public int VarsB3 { get; set; }
        public int VarsB4 { get; set; }
        public int VarsB5 { get; set; }
        public int VarsB6 { get; set; }
        public int VarsE1 { get; set; }
        public int VarsE2 { get; set; }
        public int VarsE3 { get; set; }
        public int VarsE4 { get; set; }
        public int VarsE5 { get; set; }
        public int VarsE6 { get; set; }
        public int VarsPlage { get; set; }
        public object MarketingOperation { get; set; }
    }   
}
