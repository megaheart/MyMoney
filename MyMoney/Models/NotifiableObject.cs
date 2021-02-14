using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MyMoney
{
    public abstract class NotifiableObject : INotifyPropertyChanged
    {
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [field: NonSerialized] public event PropertyChangedEventHandler PropertyChanged;
    }
}
