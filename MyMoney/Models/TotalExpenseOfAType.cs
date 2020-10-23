using System;
using System.Collections.Generic;
using System.Text;

namespace MyMoney.Models
{
    public class TotalExpenseOfAType:NotifiableObject
    {
        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (value != name)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                if (value != _value)
                {
                    _value = value;
                    OnPropertyChanged("Value");
                }
            }
        }
    }
}
