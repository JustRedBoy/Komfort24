using System;

namespace Web.Models
{
    public static class Houses
    {
        public enum House
        {
            House_20_1,
            House_20_2,
            House_22_2,
            House_24_2,
            House_26_1,
            House_26_2
        }

        public static (int houseNumber, int part, string fullHouseNumber, string fullAdress) GetHouseInfo(House house) => house switch
        {
            House.House_20_1 => (20, 1, "20/1", "Пишоновская, 20/1"),
            House.House_20_2 => (20, 2, "20/2", "Пишоновская, 20/2"),
            House.House_22_2 => (22, 2, "22/2", "Пишоновская, 22/2"),
            House.House_24_2 => (24, 2, "24/2", "Пишоновская, 24/2"),
            House.House_26_1 => (26, 1, "26/1", "Пишоновская, 26/1"),
            House.House_26_2 => (26, 2, "26/2", "Пишоновская, 26/2"),
            _ => throw new ArgumentException("Недопустимый дом")
        };

        public static House GetHouse(string accountId)
        {
            int numAcc = int.Parse(accountId);
            if(numAcc < 7637 || numAcc > 8305 || (numAcc >= 7733 && numAcc <= 7826))
            {
                throw new ArgumentException("Недопустимый аккаунт");
            }
            if (numAcc >= 7637 && numAcc <= 7732) { return House.House_20_1; }
            else if (numAcc >= 7827 && numAcc <= 7922) { return House.House_24_2; }
            else if (numAcc >= 7923 && numAcc <= 8018) { return House.House_22_2; }
            else if (numAcc >= 8019 && numAcc <= 8114) { return House.House_26_2; }
            else if (numAcc >= 8115 && numAcc <= 8210) { return House.House_26_1; }
            else { return House.House_20_2; }
        }

        public static int GetNumRow(Houses.House houseId, string accountId)
        {
            int start = GetHouseStart(houseId) - 1;
            int numRow = int.Parse(accountId) - start;
            if (accountId.Contains("/2"))
            {
                numRow++;
            }
            return numRow;
        }

        private static int GetHouseStart(House houseId) => houseId switch
        {
            House.House_20_1 => 7637,
            House.House_20_2 => 8210,
            House.House_22_2 => 7923,
            House.House_24_2 => 7827,
            House.House_26_1 => 8115,
            House.House_26_2 => 8019,
            _ => throw new ArgumentException("Недопустимый аккаунт")
        };
    }
}
