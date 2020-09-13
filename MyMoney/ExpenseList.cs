using System;
using System.Collections.Generic;
using System.Text;

namespace MyMoney
{
    public class ExpenseList:NotifiableObject
    {
        private string _name;
        public string Name { 
            set {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
            get => _name; 
        }
        private DateTime _createdDate;
        public DateTime CreatedDate
        {
            set
            {
                if (value != _createdDate)
                {
                    _createdDate = value;
                    OnPropertyChanged("CreatedDate");
                }
            }
            get => _createdDate;
        }
    }
}
