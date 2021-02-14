using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MyMoney.Models
{
    public static class AmountHelper
    {
        /// <summary>
        /// Initialize AmountHelper before using
        /// </summary>
        /// <param name="config"></param>
        public static void Initialize(unitConfig config)
        {
            if (Quantities != null)
            {
                Debug.Print("You just can initialize once.");
                return;
            }
            var prefixAndItsLevel = new Dictionary<string, int>();
            foreach (var prefix in config.Prefixs)
            {
                LevelAndPrefixs.Add(prefix.Level, prefix.Name);
                prefixAndItsLevel.Add(prefix.Name, prefix.Level);
            }
            var allPrefixes = LevelAndPrefixs.Values.ToArray();
            List<string> quantities = new List<string>();
            List<UnitInfomation> unitInfos = new List<UnitInfomation>();
            foreach (var _quantity in config.Quantities)
            {
                quantities.Add(_quantity.Key);
                var unitInfo = new UnitInfomation(_quantity.Value.DefaultUnit.Name, quantities.Count - 1, 1, true, allPrefixes, _quantity.Value.DefaultUnit.Power);
                unitInfos.Add(unitInfo);
                foreach (var prefix in config.Prefixs)
                {
                    FullUnitNameAndFullUnitInfomations
                        .Add(prefix.Name + unitInfo.Name, new FullUnitInfomation(prefix.Level, unitInfos.Count - 1));
                }
                foreach (var other in _quantity.Value.OtherUnits)
                {
                    unitInfo = new UnitInfomation(other.Name, quantities.Count - 1, other.ConversionRatio, true, allPrefixes, other.Power);
                    unitInfos.Add(unitInfo);
                    if (other.PrefixAddingEnable)
                    {
                        foreach (var prefixName in other.ValidPrefixs)
                        {
                            FullUnitNameAndFullUnitInfomations
                                .Add(prefixName + unitInfo.Name, new FullUnitInfomation(prefixAndItsLevel[prefixName], unitInfos.Count - 1));
                        }
                    }
                    else
                        FullUnitNameAndFullUnitInfomations
                                .Add(unitInfo.Name, new FullUnitInfomation(0, unitInfos.Count - 1));
                }
            }
            Quantities = quantities.ToArray();
            UnitInfos = unitInfos.ToArray();
        }
        public static Dictionary<string, FullUnitInfomation> FullUnitNameAndFullUnitInfomations { get; private set; }///!!!!
            = new Dictionary<string, FullUnitInfomation>();
        public static UnitInfomation[] UnitInfos { get; private set; } //Index is Unit Id
        public static string[] Quantities { get; private set; } //Index is Quatities Id
        public static Dictionary<int, string> LevelAndPrefixs { get; private set; } = new Dictionary<int, string>();///!!!!
        //public static Dictionary<string, int> PrefixAndItsBaseTen { get; private set; } = new Dictionary<string, int>();
        /// <summary>
        /// Convert amount's value from a unit to another
        /// </summary>
        public static float Convert(int inputUnitId, int inputLevel, float inputValue, int outputUnitId, int outputLevel)
        {
            var inputUnitInfo = UnitInfos[inputUnitId];
            var outputUnitInfo = UnitInfos[outputUnitId];
            if (inputUnitInfo.QuatityId != -1 || outputUnitInfo.QuatityId != -1 || inputUnitInfo.QuatityId != outputUnitInfo.QuatityId)
                throw new Exception("Input's unit must have the same quantity as output's unit");
            var output = inputValue * inputUnitInfo.ConversionRatio;
            output *= MathF.Pow(10, inputUnitInfo.Power * inputLevel);
            output /= outputUnitInfo.ConversionRatio;
            output /= MathF.Pow(10, outputUnitInfo.Power * outputLevel);
            return output;
            //return inputValue * inputUnitInfo.ConversionRatio * MathF.Pow(10, inputUnitInfo.Power * inputLevel) 
            //    / outputUnitInfo.ConversionRatio / MathF.Pow(10, outputUnitInfo.Power * outputLevel);
        }
        /// <summary>
        /// Convert amount's value from a unit to another
        /// </summary>
        /// <returns>
        /// true if they belong to the same quantity, otherwise, return false.
        /// </returns>
        public static bool TryConvert(int inputUnitId, int inputLevel, float inputValue, int outputUnitId, int outputLevel, out float outputValue)
        {
            outputValue = float.NaN;
            var inputUnitInfo = UnitInfos[inputUnitId];
            var outputUnitInfo = UnitInfos[outputUnitId];
            if (inputUnitInfo.QuatityId != -1 || outputUnitInfo.QuatityId != -1 || inputUnitInfo.QuatityId != outputUnitInfo.QuatityId)
                return false;
            outputValue = inputValue * inputUnitInfo.ConversionRatio;
            outputValue *= MathF.Pow(10, inputUnitInfo.Power * inputLevel);
            outputValue /= outputUnitInfo.ConversionRatio;
            outputValue /= MathF.Pow(10, outputUnitInfo.Power * outputLevel);
            return true;
            //return inputValue * inputUnitInfo.ConversionRatio * MathF.Pow(10, inputUnitInfo.Power * inputLevel) 
            //    / outputUnitInfo.ConversionRatio / MathF.Pow(10, outputUnitInfo.Power * outputLevel);
        }
    }
    public class FullUnitInfomation
    {
        public FullUnitInfomation(int level, int unitId)
        {
            Level = level;
            UnitId = unitId;
        }
        public int Level { get; private set; }
        public int UnitId { get; private set; }
    }
    public class UnitInfomation
    {
        public UnitInfomation(string name, int quatityId/*, int unitId*/, float conversionRatio, bool prefixAddingEnable, string[] validPrefixs, sbyte power)
        {
            Name = name;
            QuatityId = quatityId;
            //UnitId = unitId;
            ConversionRatio = conversionRatio;
            PrefixAddingEnable = prefixAddingEnable;
            ValidPrefixs = validPrefixs;
            Power = power;
        }
        public string Name { get; private set; }
        public int QuatityId { get; private set; } = -1;
        //public int UnitId { get; private set; }
        public float ConversionRatio { get; private set; }
        public bool PrefixAddingEnable { get; private set; }
        public string[] ValidPrefixs { get; private set; }
        public sbyte Power { get; private set; }
    }
}
