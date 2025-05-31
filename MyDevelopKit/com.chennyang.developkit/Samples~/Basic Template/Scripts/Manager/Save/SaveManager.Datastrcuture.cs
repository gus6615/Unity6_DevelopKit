using System;

namespace DevelopKit.BasicTemplate
{
    [Serializable]
    public class UserData
    {
        public string Name;
        public int Level;
        public int Exp;
        public int Gold;
        public Equipment[] Equipments { get; }

        public UserData(string name, int level, int exp, int gold)
        {
            Name = name;
            Level = level;
            Exp = exp;
            Gold = gold;
            Equipments = new Equipment[2];
            Equipments[0] = new Equipment("Sword", 0, 10, 100);
            Equipments[1] = new Equipment("Shield", 1, 5, 50);
        }
    }
    
    [Serializable]
    public class Equipment
    {
        public string Name;
        public int ID;
        public int Power;
        public int Price;
        
        public Equipment(string name, int id, int power, int price)
        {
            Name = name;
            ID = id;
            Power = power;
            Price = price;
        }
    }
}