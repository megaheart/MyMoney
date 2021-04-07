using MyMoney.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace MyMoney.Viewers
{
    public class WindowWithSharing:Window
    {
        public DictionaryOfInheritableValues InheritableValuesDict = new DictionaryOfInheritableValues();
        public DictionaryOfInheritableValues InheritableValuesBaseOn
        {
            set => InheritableValuesDict.BaseOn = value;
            get => throw new NotImplementedException();
        }
    }
}
