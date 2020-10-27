using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyMoney
{
    public enum ExpenseType
    {
        Other = 0,
        Book = 1,
        CookingMaterial = 2,
        StreetFood = 3,
        Appliance = 4,
        Service = 5,
        Tranportation = 6
    }
    public class Expense:NotifiableObject
    {
        private ExpenseType expenseType;
        public ExpenseType ExpenseType
        {
            get => expenseType;
            set
            {
                if(value != expenseType)
                {
                    expenseType = value;
                    OnPropertyChanged("ExpenseType");
                }
            }
        }
        private string _item;
        public string Item
        {
            set
            {
                if (value != _item)
                {
                    _item = value;
                    OnPropertyChanged("Item");
                }
            }
            get => _item;
        }
        private int _price;
        public int Price
        {
            set
            {
                if (value != _price)
                {
                    _price = value;
                    OnPropertyChanged("Price");
                }
            }
            get => _price;
        }
        private DateTime _time;
        public DateTime Time
        {
            set
            {
                if (value != _time)
                {
                    _time = value;
                    OnPropertyChanged("Time");
                }
            }
            get => _time;
        }
        public int ExpenseListId { set; get; }
        public int Id { set; get; }
    }
}
