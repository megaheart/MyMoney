using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyMoney.Models
{
    public class Tag : NotifiableObject
    {
        [BsonId]
        public int Id { set; get; }
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
        private string _color = "#808080";
        public string Color
        {
            set
            {
                if (value != _color)
                {
                    _color = string.IsNullOrEmpty(value) ? "#808080" : value;
                    OnPropertyChanged("Color");
                }
            }
            get => _color;
        }
    }
}
