using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MyMoney.Models
{
    public interface ISerializableWithLiteDB
    {
        BsonValue Serialize();
    }
    /// <summary>
    /// Amount use International System of Units
    /// </summary>
    public struct Amount : ISerializableWithLiteDB, IComparable<Amount>
    {
        public static readonly Amount NULL = new Amount(null, 0) { Value = float.NaN };
        public float Value { get; private set; }
        public FullUnitInfomation FullUnitInfomation { get; private set; }
        public string UnitWithoutPrefix => FullUnitInfomation == null ? "" : AmountHelper.UnitInfos[FullUnitInfomation.UnitId].Name;
        public string Unit => FullUnitInfomation == null ? "" : AmountHelper.LevelAndPrefixs[FullUnitInfomation.Level] + AmountHelper.UnitInfos[FullUnitInfomation.UnitId].Name;
        public Amount(FullUnitInfomation fullUnitInfomation, float value)
        {
            if (float.IsNaN(value)) throw new Exception("Can't create new Amount with NaN Value property");
            if (float.IsInfinity(value)) throw new Exception("Can't create new Amount with Infinity Value property");
            if (float.IsSubnormal(value)) throw new Exception("I don't know what subnormal number mean");
            if (value < 0) throw new Exception("Amount's value mustn't be less then zero");
            FullUnitInfomation = fullUnitInfomation;
            Value = value;
        }
        public Amount(float value, string unit)
        {
            if (float.IsNaN(value)) throw new Exception("Can't create new Amount with NaN Value property");
            if (float.IsInfinity(value)) throw new Exception("Can't create new Amount with Infinity Value property");
            if (float.IsSubnormal(value)) throw new Exception("I don't know what subnormal number mean");
            if (value < 0) throw new Exception("Amount's value mustn't be less then zero");
            FullUnitInfomation fullUnitInfomation;
            if (string.IsNullOrEmpty(unit))
            {
                FullUnitInfomation = null;
                Value = value;
            }
            else if (AmountHelper.FullUnitNameAndFullUnitInfomations.TryGetValue(unit, out fullUnitInfomation))
            {
                FullUnitInfomation = fullUnitInfomation;
                Value = value;
            }
            else throw new Exception("Unit (\"" + unit + "\") doesn't exist in System Of Units");
        }
        //public class BsonValueStructureOfAmount
        //{
        //    public string Unit { set; get; }
        //    public float Value { set; get; }
        //}
        //public Amount(BsonValue value)
        //{
        //    if (string.IsNullOrEmpty(value.AsString))
        //    {
        //        FullUnitInfomation = null;
        //        Value = float.NaN;
        //        return;
        //    }
        //    string unit = value["Unit"].AsString;
        //    float _value = (float)value["Value"].AsDouble;
        //    FullUnitInfomation fullUnitInfomation;
        //    if (string.IsNullOrEmpty(unit))
        //    {
        //        FullUnitInfomation = null;
        //        Value = _value;
        //    }
        //    else if (AmountHelper.FullUnitNameAndFullUnitInfomations.TryGetValue(unit, out fullUnitInfomation))
        //    {
        //        FullUnitInfomation = fullUnitInfomation;
        //        Value = _value;
        //    }
        //    else throw new Exception("Unit doesn't exist in System Of Units");
        //}
        public BsonValue Serialize() => FullUnitInfomation == null && float.IsNaN(Value) ?
            new BsonValue("") : new BsonValue(ToString());

        public Amount Add(Amount other)
        {
            if (float.IsNaN(this.Value) || float.IsNaN(other.Value)) throw new Exception("Can't add NULL Amount and other Amount.");
            if (FullUnitInfomation == null || other.FullUnitInfomation == null)
            {
                if (FullUnitInfomation != other.FullUnitInfomation) throw new Exception("Adding Amount which don't have unit and another which has unit can't be executed.");
                return new Amount(FullUnitInfomation, Value + other.Value);
            }
            float valueOfOther = other.Value;
            if (other.FullUnitInfomation.Level != FullUnitInfomation.Level || FullUnitInfomation.UnitId != other.FullUnitInfomation.UnitId)
            {
                if (!AmountHelper.TryConvert(other.FullUnitInfomation.UnitId, other.FullUnitInfomation.Level, other.Value, this.FullUnitInfomation.UnitId, this.FullUnitInfomation.Level, out valueOfOther))
                {
                    throw new InvalidOperationException("The adding two Amounts whose quantities is different can't be executed. ["
                        + AmountHelper.Quantities[AmountHelper.UnitInfos[FullUnitInfomation.UnitId].QuatityId] + " with "
                        + AmountHelper.Quantities[AmountHelper.UnitInfos[other.FullUnitInfomation.UnitId].QuatityId] + "]");
                }
            }
            return new Amount(FullUnitInfomation, Value + valueOfOther);
        }
        public Amount Divide(int k)
        {
            if (k <= 0) throw new InvalidOperationException("Can't divide amount by zero or negative number");
            return new Amount(FullUnitInfomation, Value / k);
        }

        public int CompareTo(Amount other)
        {
            if (float.IsNaN(this.Value) || float.IsNaN(other.Value)) throw new Exception("Can't compare NULL Amount to other Amount.");
            if (FullUnitInfomation == null || other.FullUnitInfomation == null)
            {
                if (FullUnitInfomation != other.FullUnitInfomation) throw new Exception("Can't compare Amount which don't have unit to another which has unit.");
                if (Value == other.Value) return 0;
                if (other.Value > Value) return -1;
                return 1;
            }
            float valueOfOther = other.Value;
            if (other.FullUnitInfomation.Level != FullUnitInfomation.Level || FullUnitInfomation.UnitId != other.FullUnitInfomation.UnitId)
            {
                if (!AmountHelper.TryConvert(other.FullUnitInfomation.UnitId, other.FullUnitInfomation.Level, other.Value, this.FullUnitInfomation.UnitId, this.FullUnitInfomation.Level, out valueOfOther))
                {
                    throw new InvalidOperationException("The adding two Amounts whose quantities is different can't be executed. ["
                        + AmountHelper.Quantities[AmountHelper.UnitInfos[FullUnitInfomation.UnitId].QuatityId] + " with "
                        + AmountHelper.Quantities[AmountHelper.UnitInfos[other.FullUnitInfomation.UnitId].QuatityId] + "]");
                }
            }
            if (Value == valueOfOther) return 0;
            if (valueOfOther > Value) return -1;
            return 1;
            //throw new Exception("Can't compare two Amounts which have different units with each other. [" + other.UnitWithoutPrefix + " with " + UnitWithoutPrefix + "]");
        }
        public bool Equals(Amount other)
        {
            if (FullUnitInfomation == null || other.FullUnitInfomation == null)
            {
                if (FullUnitInfomation != other.FullUnitInfomation) return false;
                return Value == other.Value;
            }
            float valueOfOther = other.Value;
            if (other.FullUnitInfomation.Level != FullUnitInfomation.Level || FullUnitInfomation.UnitId != other.FullUnitInfomation.UnitId)
            {
                if (!AmountHelper.TryConvert(other.FullUnitInfomation.UnitId, other.FullUnitInfomation.Level, other.Value, this.FullUnitInfomation.UnitId, this.FullUnitInfomation.Level, out valueOfOther))
                {
                    throw new InvalidOperationException("The adding two Amounts whose quantities is different can't be executed. ["
                        + AmountHelper.Quantities[AmountHelper.UnitInfos[FullUnitInfomation.UnitId].QuatityId] + " with "
                        + AmountHelper.Quantities[AmountHelper.UnitInfos[other.FullUnitInfomation.UnitId].QuatityId] + "]");
                }
            }
            return Value == valueOfOther;
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType().IsEquivalentTo(typeof(Amount))) return this.Equals((Amount)obj);
            return base.Equals(obj);
        }
        public override string ToString()
        {
            if (float.IsNaN(Value))
            {
                if (FullUnitInfomation == null) return "";
                else throw new Exception("I don't know how this situation happens");
            }
            if (FullUnitInfomation == null) return Value.ToString();
            return Value + " " + Unit;
        }
        private static Regex regex = new Regex(@"^\s*(?<value>\d+(\.\d+)?)\s*(?<unit>[^\s]*)\s*$");
        public static Amount Parse(string s)
        {
            if (string.IsNullOrEmpty(s)) return NULL;
            Match match = regex.Match(s);
            if (match == Match.Empty) throw new Exception("Unable to parse this string (\"" + s + "\") to amount");
            return new Amount(float.Parse(match.Groups["value"].Value), match.Groups["unit"].Value);
        }
        public static bool TryParse(string s, out Amount amount)
        {
            amount = NULL;
            if (string.IsNullOrEmpty(s)) return true;
            Match match = regex.Match(s);
            if (match == Match.Empty) return false;
            amount = new Amount(float.Parse(match.Groups["value"].Value), match.Groups["unit"].Value);
            return true;
        }
        public static Amount operator +(Amount d, Amount t)
        {
            if (float.IsNaN(d.Value) || float.IsNaN(t.Value)) throw new Exception("Can't add NULL Amount and other Amount.");
            if (d.FullUnitInfomation == null || t.FullUnitInfomation == null)
            {
                if (d.FullUnitInfomation != t.FullUnitInfomation) throw new Exception("Adding Amount which don't have unit and another which has unit can't be executed.");
                return new Amount(d.FullUnitInfomation, d.Value + t.Value);
            }
            float valueOfOther = t.Value;
            if (t.FullUnitInfomation.Level != d.FullUnitInfomation.Level || d.FullUnitInfomation.UnitId != t.FullUnitInfomation.UnitId)
            {
                if (!AmountHelper.TryConvert(t.FullUnitInfomation.UnitId, t.FullUnitInfomation.Level, t.Value, d.FullUnitInfomation.UnitId, d.FullUnitInfomation.Level, out valueOfOther))
                {
                    throw new InvalidOperationException("The adding two Amounts whose quantities is different can't be executed. ["
                        + AmountHelper.Quantities[AmountHelper.UnitInfos[d.FullUnitInfomation.UnitId].QuatityId] + " with "
                        + AmountHelper.Quantities[AmountHelper.UnitInfos[t.FullUnitInfomation.UnitId].QuatityId] + "]");
                }
            }
            return new Amount(d.FullUnitInfomation, d.Value + valueOfOther);
        }
        public static Amount operator -(Amount d, Amount t)
        {
            if (float.IsNaN(d.Value) || float.IsNaN(t.Value)) throw new Exception("Can't subtract NULL Amount from other Amount, and vice versa.");
            if (d.FullUnitInfomation == null || t.FullUnitInfomation == null)
            {
                if (d.FullUnitInfomation != t.FullUnitInfomation) throw new Exception("Can't subtract Amount which don't have unit from another which has unit, and vice versa.");
                return new Amount(d.FullUnitInfomation, d.Value - t.Value);
            }
            float valueOfOther = t.Value;
            if (t.FullUnitInfomation.Level != d.FullUnitInfomation.Level || d.FullUnitInfomation.UnitId != t.FullUnitInfomation.UnitId)
            {
                if (!AmountHelper.TryConvert(t.FullUnitInfomation.UnitId, t.FullUnitInfomation.Level, t.Value, d.FullUnitInfomation.UnitId, d.FullUnitInfomation.Level, out valueOfOther))
                {
                    throw new InvalidOperationException("The subtracting two Amounts whose quantities is different can't be executed. ["
                        + AmountHelper.Quantities[AmountHelper.UnitInfos[d.FullUnitInfomation.UnitId].QuatityId] + " with "
                        + AmountHelper.Quantities[AmountHelper.UnitInfos[t.FullUnitInfomation.UnitId].QuatityId] + "]");
                }
            }
            valueOfOther = d.Value - valueOfOther;
            if (valueOfOther < 0) throw new InvalidOperationException("Subtrahend mustn't be less then minus Amount");
            return new Amount(d.FullUnitInfomation, valueOfOther);
        }
        public static Amount operator *(Amount d, float t)
        {
            if (t < 0) throw new InvalidOperationException("Can't multiple amount by negative number");
            return new Amount(d.FullUnitInfomation, d.Value * t);
        }
        public static Amount operator *(Amount d, int t)
        {
            if (t < 0) throw new InvalidOperationException("Can't multiple amount by negative number");
            return new Amount(d.FullUnitInfomation, d.Value * t);
        }
        public static Amount operator *(float d, Amount t)
        {
            return t * d;
        }
        public static Amount operator *(int d, Amount t)
        {
            return t * d;
        }
        public static Amount operator /(Amount d, float t)
        {
            if (t <= 0) throw new InvalidOperationException("Can't divide amount by zero or negative number");
            return new Amount(d.FullUnitInfomation, d.Value / t);
        }
        public static Amount operator /(Amount d, int t)
        {
            if (t <= 0) throw new InvalidOperationException("Can't divide amount by zero or negative number");
            return new Amount(d.FullUnitInfomation, d.Value / t);
        }
        public static bool operator ==(Amount d1, Amount d2)
        {
            return d1.Equals(d2);
        }
        public static bool operator !=(Amount d1, Amount d2)
        {
            return !d1.Equals(d2);
        }
        public static bool operator <(Amount t1, Amount t2)
        {
            return t1.CompareTo(t2) < 0;
        }
        public static bool operator >(Amount t1, Amount t2)
        {
            return t1.CompareTo(t2) > 0;
        }
        public static bool operator <=(Amount t1, Amount t2)
        {
            return t1.CompareTo(t2) <= 0;
        }
        public static bool operator >=(Amount t1, Amount t2)
        {
            return t1.CompareTo(t2) >= 0;
        }
    }
}
