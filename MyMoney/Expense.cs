using System;
using System.Collections.Generic;
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
        private string _price;
        public string Price
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
    }
}
