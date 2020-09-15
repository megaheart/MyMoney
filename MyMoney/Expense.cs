using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyMoney
{
    public class Expense:NotifiableObject
    {
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
                    OnPropertyChanged("CreatedDate");
                }
            }
            get => _time;
        }
        public int ExpenseListId { set; get; }
        public int Id { set; get; }
    }
}
