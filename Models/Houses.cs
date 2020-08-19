using System;

namespace Models
{
    public static class Houses
    {
        public static int Count { get => Enum.GetNames(typeof(House)).Length; }

        /// <summary>
        /// Enumeration of existing houses
        /// </summary>
        public enum House
        {
            House_20_1,
            House_20_2,
            House_22_2,
            House_24_2,
            House_26_1,
            House_26_2
        }

        /// <summary>
        /// Get information about the house
        /// </summary>
        /// <param name="houseNum"></param>
        /// <returns>Tuple with house number, part of house, full house number and full adress</returns>
        public static (int houseNumber, int part, string fullHouseNumber, string fullAdress) GetHouseInfo(int houseNum) => houseNum switch
        {
            0 => (20, 1, "20/1", "ул. Пишоновская, 20/1"),
            1 => (20, 2, "20/2", "ул. Пишоновская, 20/2"),
            2 => (22, 2, "22/2", "ул. Пишоновская, 22/2"),
            3 => (24, 2, "24/2", "ул. Пишоновская, 24/2"),
            4 => (26, 1, "26/1", "ул. Пишоновская, 26/1"),
            5 => (26, 2, "26/2", "ул. Пишоновская, 26/2"),
            _ => throw new ArgumentException("Недопустимый номер дома")
        };

        /// <summary>
        /// Get the number of flats in the house
        /// </summary>
        /// <param name="houseNum">House number for information</param>
        /// <returns>Number of flats in the house</returns>
        public static int GetNumFlats(int houseNum)
        {
            return houseNum == 3 ? 97 : 96;
        }

        /// <summary>
        /// Get the number of flats in the house
        /// </summary>
        /// <param name="house">House for information</param>
        /// <returns>Number of flats in the house</returns>
        public static int GetNumFlats(string house)
        {
            return house == "24/2" ? 97 : 96;
        }

        //public static int GetNumFlats(House house)
        //{
        //    return house == House.House_24_2 ? 97 : 96;
        //}

        //public static (int houseNumber, int part, string fullHouseNumber, string fullAdress) GetHouseInfo(House house) => house switch
        //{
        //    House.House_20_1 => (20, 1, "20/1", "Пишоновская, 20/1"),
        //    House.House_20_2 => (20, 2, "20/2", "Пишоновская, 20/2"),
        //    House.House_22_2 => (22, 2, "22/2", "Пишоновская, 22/2"),
        //    House.House_24_2 => (24, 2, "24/2", "Пишоновская, 24/2"),
        //    House.House_26_1 => (26, 1, "26/1", "Пишоновская, 26/1"),
        //    House.House_26_2 => (26, 2, "26/2", "Пишоновская, 26/2"),
        //    _ => throw new ArgumentException("Недопустимый номер дома")
        //};

        //private static int GetHouseStart(House house) => house switch
        //{
        //    House.House_20_1 => 7637,
        //    House.House_20_2 => 8210,
        //    House.House_22_2 => 7923,
        //    House.House_24_2 => 7827,
        //    House.House_26_1 => 8115,
        //    House.House_26_2 => 8019,
        //    _ => throw new ArgumentException("Недопустимый аккаунт")
        //};

        //public static House GetHouse(string accountId)
        //{
        //    int numAcc = int.Parse(accountId);
        //    if (numAcc < 7637 || numAcc > 8305 || (numAcc >= 7733 && numAcc <= 7826))
        //    {
        //        throw new ArgumentException("Недопустимый аккаунт");
        //    }
        //    if (numAcc >= 7637 && numAcc <= 7732) { return House.House_20_1; }
        //    else if (numAcc >= 7827 && numAcc <= 7922) { return House.House_24_2; }
        //    else if (numAcc >= 7923 && numAcc <= 8018) { return House.House_22_2; }
        //    else if (numAcc >= 8019 && numAcc <= 8114) { return House.House_26_2; }
        //    else if (numAcc >= 8115 && numAcc <= 8210) { return House.House_26_1; }
        //    else { return House.House_20_2; }
        //}

        //public static int GetNumRow(House houseId, string accountId)
        //{
        //    int start = GetHouseStart(houseId) - 1;
        //    int numRow = int.Parse(accountId) - start;
        //    if (accountId.Contains("/2"))
        //    {
        //        numRow++;
        //    }
        //    return numRow;
        //}
    }
}
