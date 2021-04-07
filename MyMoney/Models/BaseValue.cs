using System;
using System.Collections.Generic;
using System.Text;

namespace MyMoney.Models
{
    public delegate void BaseValueChanged(object newValue);
    public class BaseValue
    {
        public event BaseValueChanged ValueChanged;
        private object _value;
        public object Value
        {
            set
            {
                if(value != _value)
                {
                    _value = value;
                    ValueChanged?.Invoke(value);
                }
            }
            get => _value;
        }
    }
}
