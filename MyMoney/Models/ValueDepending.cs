using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyMoney.Models
{
    public class ValueDepending:IDisposable
    {
        private BaseValue _base;
        public BaseValue BaseOn
        {
            set
            {
                if (_base != value)
                {
                    if (_base != null) _base.ValueChanged -= OnBaseValueChanged;
                    if (value != null)
                    {
                        if (_base == null || _base.Value != value.Value)
                        {
                            OnBaseValueChanged(value.Value);
                        }
                        value.ValueChanged += OnBaseValueChanged;
                    }
                    else if (_base != null && _base.Value != null)
                    {
                        OnBaseValueChanged(null);
                    }
                    _base = value;
                }
            }
        }
        public ValueDepending(BaseValue baseValue)
        {
            _base = baseValue;
            if (_base != null)
                baseValue.ValueChanged += OnBaseValueChanged;
        }
        public void OnBaseValueChanged(object newValue)
        {
            _valueChanged?.Invoke(newValue);
        }
        public event BaseValueChanged _valueChanged;
        public event BaseValueChanged ValueChanged
        {
            add
            {
                _valueChanged += value;
                if (_base != null)
                    value?.Invoke(_base.Value);
            }
            remove
            {
                _valueChanged -= value;
            }
        }
        public void Dispose()
        {
            _base.ValueChanged -= OnBaseValueChanged;
            _base = null;
        }

    }
}
