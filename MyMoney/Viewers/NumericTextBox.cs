using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyMoney.Viewers
{
    public class NumbericTextBox : TextBox
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(NumbericTextBox), new PropertyMetadata(0, ValuePropertyChanged));
        public static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumbericTextBox ctrl = d as NumbericTextBox;
            ctrl.DisplayValue((int)e.NewValue);
        }
        public int Value { get => (int)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public new string Text { get => throw new Exception(); set { } }
        //private static TextCompositionEventParameters GetKeyCompositionEventParameters(TextCompositionEventArgs e)
        //    => new TextCompositionEventParameters(e.ControlText, e.SystemText, e.Text, e.Handled);
        private int HowManyDotsBefore(int index)
        {
            int dotsNumber = base.Text.Length / 4;
            dotsNumber -= (base.Text.Length - index + 1) / 4;
            return dotsNumber;
        }
        private void DisplayValue(int value)
        {
            if (value == 0)
            {
                base.Text = "";
                return;
            }
            string output = value.ToString();
            value = output.Length - 3;
            while (value > 0)
            {
                output = output.Insert(value, ".");
                value -= 3;
            }
            base.Text = output;
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Handled) return;
            if (e.Key != Key.Back && e.Key != Key.Delete) return;
            if (e.Key == Key.Delete && base.Text.Length == CaretIndex) return;
            e.Handled = true;
            int lengthFromCaretIndexToEnd = base.Text.Length - CaretIndex;
            int power = lengthFromCaretIndexToEnd - (lengthFromCaretIndexToEnd) / 4;
            int a = (int)Math.Round(Math.Pow(10, power));
            int mod = Value % a;
            switch (e.Key)
            {
                case Key.Back:
                    Value = Value / (a * 10) * a + mod;
                    int caretIndex = base.Text.Length - lengthFromCaretIndexToEnd - 1;
                    if (caretIndex > -1)
                    {
                        if (base.Text[caretIndex] == '.') CaretIndex = caretIndex;
                        else CaretIndex = caretIndex + 1;
                    }
                    else CaretIndex = 0;
                    break;
                case Key.Delete:
                    bool isInFrontOfDot = base.Text[CaretIndex] == '.';
                    Value = (Value - mod) / 10 + mod % (a / 10);
                    if (isInFrontOfDot) CaretIndex = base.Text.Length - lengthFromCaretIndexToEnd + 2;
                    else CaretIndex = base.Text.Length - lengthFromCaretIndexToEnd + 1;
                    break;
            }
        }
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            int x;
            e.Handled = !int.TryParse(e.Text, out x) || (string.IsNullOrEmpty(base.Text) && e.Text == "0");
            if (e.Handled) return;
            //MyConsole.WriteLine(JsonSerializer.Serialize(GetKeyCompositionEventParameters(e1), typeof(TextCompositionEventParameters), new JsonSerializerOptions() { WriteIndented = true }));
            base.OnPreviewTextInput(e);
            if (e.Handled) return;
            e.Handled = true;
            int lengthFromCaretIndexToEnd = base.Text.Length - CaretIndex;
            int power = lengthFromCaretIndexToEnd - (lengthFromCaretIndexToEnd) / 4;
            int a = (int)Math.Round(Math.Pow(10, power));
            int mod = Value % a;
            long newValue = ((long)Value - mod) * 10 + x * a + mod;
            if (newValue > int.MaxValue) return;
            Value = (int)newValue;
            CaretIndex = base.Text.Length - lengthFromCaretIndexToEnd;
        }
        protected override void OnInitialized(EventArgs e1)
        {
            base.OnInitialized(e1);
            DataObject.AddPastingHandler(this, new DataObjectPastingEventHandler((sender, e) =>
            {
                if (e.DataObject.GetDataPresent(typeof(string)))
                {
                    if (!int.TryParse(e.DataObject.GetData(typeof(string)) as string, out _))
                        e.CancelCommand();
                }
                else e.CancelCommand();
            }));
        }
    }
}
