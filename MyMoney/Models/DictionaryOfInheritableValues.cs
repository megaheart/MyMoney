using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace MyMoney.Models
{
    public class DictionaryOfInheritableValues:IDisposable
    {
        private Dictionary<string, BaseValue> propertiesDict = new Dictionary<string, BaseValue>();
        //_insertedOrRemoveBaseValue( name : string, basevalue : BaseValue)
        private event Action<string, BaseValue> _insertedOrRemoveBaseValue;
        private event Action _baseChanged;
        //Which is created from this Dictionary
        private Dictionary<string, ValueDepending> valueDependings = new Dictionary<string, ValueDepending>();
        private DictionaryOfInheritableValues _base;
        public DictionaryOfInheritableValues BaseOn
        {
            set
            {
                if(value != _base)
                {
                    if(_base != null)
                    {
                        _base._insertedOrRemoveBaseValue -= OnBaseInsertedOrRemovedBaseValue;
                        _base._baseChanged -= ReloadValueDependings;
                    }
                    _base = value;
                    if(value != null)
                    {
                        _base._insertedOrRemoveBaseValue += OnBaseInsertedOrRemovedBaseValue;
                        _base._baseChanged += ReloadValueDependings;
                    }
                    ReloadValueDependings();
                }
            }
        }
        private void ReloadValueDependings()
        {
            foreach (var i in valueDependings)
            {
                i.Value.BaseOn = GetBaseValueOf(i.Key);
            }
            _baseChanged?.Invoke();
        }
        private void OnBaseInsertedOrRemovedBaseValue(string name, BaseValue baseValueOfNameAfterActing)
        {
            ValueDepending valueD;
            if (valueDependings.TryGetValue(name, out valueD))
            {
                valueD.BaseOn = baseValueOfNameAfterActing;
            }
            if (!propertiesDict.TryGetValue(name, out _))
            {
                _insertedOrRemoveBaseValue(name, baseValueOfNameAfterActing);
            }
        }
        private BaseValue _getBaseValueOf(string name)
        {
            BaseValue output;
            if (propertiesDict.TryGetValue(name, out output))
            {
                return output;
            }
            else if (_base != null)
                return _base._getBaseValueOf(name);
            else return null;
        }
        private BaseValue GetBaseValueOf(string name)
        {
            if (_base == null) return null;
            else
                return _base._getBaseValueOf(name);
        }
        public BaseValue InsertInheritableValue(string name, object originalValue)
        {
            BaseValue baseValue = new BaseValue();
            baseValue.Value = originalValue;
            propertiesDict.Add(name, baseValue);
            _insertedOrRemoveBaseValue(name, baseValue);
            return baseValue;
        }
        public bool RemoveInheritableValue(string name)
        {
            if (propertiesDict.Remove(name))
            {
                BaseValue baseValue = GetBaseValueOf(name);
                _insertedOrRemoveBaseValue(name, baseValue);
                return true;
            }
            return false;
        }
        public ValueDepending GetValueOf(string name)
        {
            BaseValue baseValue = GetBaseValueOf(name);
            return new ValueDepending(baseValue);
        }

        public void Dispose()
        {
            propertiesDict = null;
            valueDependings = null;
            _base = null;
        }
    }
}
