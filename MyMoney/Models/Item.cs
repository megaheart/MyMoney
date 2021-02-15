using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyMoney.Models
{
    public class Item : NotifiableObject
    {
        private string _name;
        public string Name
        {
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
            get => _name;
        }
        [BsonId]
        public int Id { set; get; }
    }
}
