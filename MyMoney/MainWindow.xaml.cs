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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyMoney.Controllers;
using MyMoney.Models;

namespace MyMoney
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyContext myContext = new MyContext();
        
        ObservableCollection<ExpenseList> ExpenseLists;
        
        ExpenseListManager ExpenseListManager;
        ExpensesManager ExpensesManager;
        //bool doesUserClick = false;
        public MainWindow()
        {
            InitializeComponent();
            ExpensesManager = new ExpensesManager(myContext);
            ExpenseListManager = new ExpenseListManager(myContext);
            ExpenseListManager.Initialize();
            ExpenseLists = ExpenseListManager.ExpenseLists;
            ExpenseList_Viewer.ExpensesManager = ExpensesManager;
            ExpenseList_Viewer.myContext = myContext;
            ListOfExpenseListViewer.ItemsSource = ExpenseLists;
            var ExpenseListIdToOpen = Cookie.GetCookie(myContext, "ExpenseListIdToOpen");
            if (ExpenseListIdToOpen == "")
            {
                ListOfExpenseListViewer.SelectedIndex = 0;
            }
            else
            {
                int expectId = int.Parse(ExpenseListIdToOpen);
                int index = 0;
                while (index < ExpenseLists.Count)
                {
                    if (ExpenseLists[index].Id == expectId) break;
                    index++;
                }
                ListOfExpenseListViewer.SelectedIndex = index == ExpenseLists.Count ? 0 : index;
            }

            //doesUserClick = true;
        }
        private void CreatingButton_Click(object sender, RoutedEventArgs e)
        {
            ExpenseListManager.Add(NewExpenseListTitle.Text);
            NewExpenseListTitle.Text = "";
            CreatingButton.Visibility = Visibility.Collapsed;
            CreatingPanel.Visibility = Visibility.Visible;
        }
        private void AddNewExpenseList(object sender, RoutedEventArgs e)
        {
            Exception exception = ExpenseListManager.Add(NewExpenseListTitle.Text);
            if (exception != null) MessageBox.Show(exception.Message);
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
            if (ListOfExpenseListViewer.SelectedIndex == -1)
            {
                
            }
            else
            {
                var expenseList = ExpenseLists[ListOfExpenseListViewer.SelectedIndex];
                ExpenseList_Viewer.ExpenseList = expenseList;
                Cookie.SetCookie(myContext, "ExpenseListIdToOpen", ListOfExpenseListViewer.SelectedIndex.ToString());
            }
        }
        


    }
    public class IntToVNDCurrency : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int cost = int.Parse(value.ToString());
            if (cost == 0) return "0 đ";
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
    public class ExpenseTypeEnumToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ExpensesManager.totalExpenseOfAllTypes[(int)value].Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
