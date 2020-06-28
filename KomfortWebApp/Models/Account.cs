using System;
using System.Collections.Generic;

namespace KomfortWebApp.Models
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

        public Account(IList<IList<object>> values)
        {
            AccountId = values[0][1].ToString();
            HouseId = Houses.GetHouse(AccountId);
            FlatNumber = values[0][0].ToString();
            FlatOwner = values[0][2].ToString();

            WerStateStart = GetNumberValue(values[0][3]) - GetNumberValue(values[0][4]);
            WerSquare = GetNumberValue(values[0][5]);
            WerForMonth = GetNumberValue(values[0][6]);
            WerPayment = GetNumberValue(values[0][15]) + GetNumberValue(values[0][14]) - GetNumberValue(values[0][12]) - GetNumberValue(values[0][10]);

            WaterCurrentValue = (int)GetNumberValue(values[0][7]);
            WaterPrevValue = (int)GetNumberValue(values[0][8]);
            WaterPayment = GetNumberValue(values[0][10]);

            HeatingStateStart = GetNumberValue(values[1][0]) - GetNumberValue(values[1][1]);
            HeatingSquare = GetNumberValue(values[1][2]);
            Type = GetHeatingType(values[1][3].ToString());
            HeatingCurrentValue = GetNumberValue(values[1][4]);
            HeatingPayment = GetNumberValue(values[1][10]) + GetNumberValue(values[1][11]) - GetNumberValue(values[1][8]);
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
