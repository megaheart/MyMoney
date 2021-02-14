using System;
using System.Collections.Generic;
using System.Text;

namespace MyMoney.Models
{
    public class Tag : NotifiableObject
    {
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
                    _color = value;
                    OnPropertyChanged("Name");
                }
            }
            get => _color;
        }
    }
}
