using System;
using System.Collections.Generic;

namespace Web.Models
{
    public class Account
    {
        public string AccountId { get; set; }
        public Houses.House HouseId { get; set; }
        public string FlatNumber { get; set; }
        public string FlatOwner { get; set; }

        #region Heating
        public double HeatingStateStart { get; set; }
        public HeatingType Type { get; set; }
        public double HeatingSquare { get; set; }
        public double HeatingCurrentValue { get; set; }
        public double HeatingPayment { get; set; }
        public enum HeatingType
        {
            Simple = 0,
            Gigacalories,
            Gigajoules,
            Megawatt,
            Kilowatt
        }
        #endregion

        #region Wer
        public double WerStateStart { get; set; }
        public double WerSquare { get; set; }
        public double WerForMonth { get; set; }
        public double WerPayment { get; set; }
        #endregion

        #region Water
        public int WaterPrevValue { get; set; }
        public int WaterCurrentValue { get; set; }
        public double WaterPayment { get; set; }
        #endregion

        //#region Garbage 
        //public double GarbageStateStart { get; set; }
        //public int Persons { get; set; }
        //#endregion

        public Account(IList<object> values)
        {
            FlatNumber = values[0].ToString();
            AccountId = values[1].ToString();
            HouseId = Houses.GetHouse(AccountId);
            FlatOwner = values[2].ToString();

            HeatingStateStart = Math.Round(GetNumberValue(values[3]) - GetNumberValue(values[4]), 2); // debet - credit
            HeatingSquare = GetNumberValue(values[5]);
            Type = GetHeatingType(values[6].ToString());
            HeatingCurrentValue = GetNumberValue(values[7]);
            HeatingPayment = Math.Round(GetNumberValue(values[13]) + GetNumberValue(values[14]) - GetNumberValue(values[11]), 2); // cashbox + bank - privileges

            WerStateStart = Math.Round(GetNumberValue(values[18]) - GetNumberValue(values[19]), 2); // debet - credit
            WerSquare = GetNumberValue(values[20]);
            WerForMonth = GetNumberValue(values[21]);
            WerPayment = Math.Round(GetNumberValue(values[29]) + GetNumberValue(values[30]) - GetNumberValue(values[25]) - GetNumberValue(values[27]), 2); // cashbox + bank - waterPayment - privileges

            WaterCurrentValue = (int)GetNumberValue(values[22]);
            WaterPrevValue = (int)GetNumberValue(values[23]);
            WaterPayment = GetNumberValue(values[24]);
        }

        private HeatingType GetHeatingType(string type) => type switch
        {
            "" => HeatingType.Simple,
            "Гкал" => HeatingType.Gigacalories,
            "ГДж" => HeatingType.Gigajoules,
            "кВт" => HeatingType.Kilowatt,
            "МВт" => HeatingType.Megawatt,
            _ => throw new ArgumentException("Недопустимый тип")
        };

        public double GetHeatingCoefficient() => Type switch
        {
            HeatingType.Simple => 1,
            HeatingType.Gigacalories => 1.1,
            HeatingType.Gigajoules => 1.1 / 4.187,
            HeatingType.Megawatt => 1.1 * 0.86,
            HeatingType.Kilowatt => 1.1 / 1162.2,
            _ => throw new ArgumentException("Недопустимый тип")
        };

        private double GetNumberValue(object obj) 
        {
            return string.IsNullOrEmpty(obj.ToString()) ? 0.0 : double.Parse(obj.ToString());
        }
    }
}
