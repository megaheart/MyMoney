using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyMoney
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        DateTimeToString dateTimeToString = new DateTimeToString();
        ObservableCollection<ExpenseList> expenseLists = new ObservableCollection<ExpenseList>();
        ObservableCollection<Expense> expenses = new ObservableCollection<Expense>();
        private void CreatingButton_Click(object sender, RoutedEventArgs e)
        {
            CreatingButton.Visibility = Visibility.Collapsed;
            CreatingPanel.Visibility = Visibility.Visible;
        }
        private void AddNewExpenseList(object sender, RoutedEventArgs e)
        {
            
            CreatingButton.Visibility = Visibility.Visible;
            CreatingPanel.Visibility = Visibility.Collapsed;
        }
        private void CancelAddingNewExpenseList(object sender, RoutedEventArgs e)
        {
            CreatingButton.Visibility = Visibility.Visible;
            CreatingPanel.Visibility = Visibility.Collapsed;
        }

        private void ReloadExpensesViewer(object sender, SelectionChangedEventArgs e)
        {

        }
    }
    public class IntToVNDCurrency : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int cost = int.Parse(value.ToString());
            string output = " 000 đ";
            while (cost > 0)
            {
                string x = (cost % 1000).ToString();
                if (cost > 999) while (x.Length != 3) x = "0" + x;
                output = " " + x + output;
                cost /= 1000;
            }
            return output.Substring(1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DateTimeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = (DateTime)value;
            if(time.TimeOfDay == TimeSpan.Zero)
            {
                return time.ToString("dd/MM/yyyy");
            }
            return time.ToString("HH:mm dd/MM/yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
