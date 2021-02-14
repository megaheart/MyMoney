using System;
using System.Collections.Generic;
using System.Text;

namespace MyMoney.Models
{
    //Power of unit: m -> 1, m^2 -> 2, ㎥ -> 3
    //ConversionRatio of unit: the ratio to convert to default unit
    public class unitConfig
    {
        public prefix[] Prefixs { set; get; }
        public Dictionary<string, quantity> Quantities { set; get; }
    }
    public class prefix
    {
        public string Name { set; get; }
        public sbyte Level { set; get; }
    }
    public class quantity
    {
        public defaultUnit DefaultUnit { set; get; }
        public otherUnitDetails[] OtherUnits { set; get; }
    }
    public class defaultUnit
    {
        public string Name { set; get; }
        public sbyte Power { set; get; }
    }
    public class otherUnitDetails
    {
        public string Name { set; get; }
        public Scope<string> Region { set; get; }
        public float ConversionRatio { set; get; }
        public bool PrefixAddingEnable { set; get; }
        public List<string> ValidPrefixs { set; get; }
        public sbyte Power { set; get; }
    }
    public class Scope<T>
    {
        private List<T> _except;
        public List<T> Except
        {
            set
            {
                if (_include != null) throw new Exception("Include property has been set");
                _except = value;
            }
            get => _except;
        }
        private List<T> _include;
        public List<T> Include
        {
            set
            {
                if (_except != null) throw new Exception("Except property has been set");
                _include = value;
            }
            get => _include;
        }
        public bool IsInScope(T o)
        {
            if (_except != null)
            {
                return !_except.Contains(o);
            }
            if (_include != null)
            {
                return _include.Contains(o);
            }
            throw new Exception("Include property or Except property hasn't been set");
        }
    }
}
