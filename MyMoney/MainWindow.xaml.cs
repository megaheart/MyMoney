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
        IntToVNDCurrency currencyConverter = new IntToVNDCurrency();
        DateTimeToString dateTimeDisplay = new DateTimeToString();
        ObservableCollection<ExpenseList> ExpenseLists;
        ObservableCollection<Expense> Expenses { get => ExpensesManager.Expenses; }
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
            var list = new TotalExpenseOfAType[ExpensesManager.totalExpenseOfAllTypes.Length];
            list[list.Length - 1] = ExpensesManager.totalExpenseOfAllTypes[0];
            Array.Copy(ExpensesManager.totalExpenseOfAllTypes, 1, list, 0, list.Length - 1);
            TotalExpenseOfAllTypeViewer.ItemsSource = list;
            NewExpense_Type.ItemsSource = list.Select(t =>
            {
                var index = t.Name.IndexOf('(');
                if (index > -1) return t.Name.Remove(index).Trim();
                return t.Name.Trim();
            });
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
                ExpenseList_Name.Text = "";
                ExpenseList_TotalExpense.Text = "";
                ExpensesViewer.ItemsSource = null;
                //ExpenseManager.ClearCache();
            }
            else
            {
                var expenseList = ExpenseLists[ListOfExpenseListViewer.SelectedIndex];
                ExpenseList_Name.Text = expenseList.Name;
                ExpensesManager.Initialize(expenseList.Id);
                ExpensesViewer.ItemsSource = Expenses;
                ExpenseList_TotalExpense.Text = currencyConverter.Convert(ExpensesManager.TotalExpense, null, null, null).ToString();
                Cookie.SetCookie(myContext, "ExpenseListIdToOpen", ListOfExpenseListViewer.SelectedIndex.ToString());
            }
        }
        private void CreatingExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListOfExpenseListViewer.SelectedIndex == -1) return;
            NewExpense_Item.Text = "";
            NewExpense_Price.Text = "";
            NewExpense_Time.Text = "";
            NewExpense_Type.SelectedIndex = NewExpense_Type.Items.Count - 1;
            CreatingNewExpenseBtn.Visibility = Visibility.Collapsed;
            CreatingNewExpensePanel2.Visibility = Visibility.Visible;
            CreatingNewExpensePanel.Visibility = Visibility.Visible;
        }

        private Expense ExpenseNeedUpdating = null;
        private void AddNewExpense(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewExpense_Item.Text))
            {
                MessageBox.Show("Title Of New Expense cannot be empty.");
                return;
            }
            int price;
            if (!int.TryParse(NewExpense_Price.Text.Trim(), out price))
            {
                MessageBox.Show("Price Of New Expense cannot be empty and must be number.");
                return;
            }
            int expenseTypeIndex = NewExpense_Type.SelectedIndex;
            if (expenseTypeIndex == NewExpense_Type.Items.Count - 1) expenseTypeIndex = 0;
            else expenseTypeIndex++;
            if (ExpenseNeedUpdating == null)
            {
                Exception exception = ExpensesManager.Add(NewExpense_Item.Text, ExpenseLists[ListOfExpenseListViewer.SelectedIndex].Id, price, NewExpense_Time.Text, expenseTypeIndex);
                if (exception != null) MessageBox.Show(exception.Message);
                ExpenseList_TotalExpense.Text = currencyConverter.Convert(ExpensesManager.TotalExpense, null, null, null).ToString();
            }
            else
            {
                Exception exception = ExpensesManager.Update(ExpenseNeedUpdating, NewExpense_Item.Text, price, NewExpense_Time.Text, expenseTypeIndex);
                if (exception != null) MessageBox.Show(exception.Message);
                ExpenseList_TotalExpense.Text = currencyConverter.Convert(ExpensesManager.TotalExpense, null, null, null).ToString();
                ExpenseNeedUpdating = null;
            }
            CreatingNewExpenseBtn.Visibility = Visibility.Visible;
            CreatingNewExpensePanel2.Visibility = Visibility.Collapsed;
            CreatingNewExpensePanel.Visibility = Visibility.Collapsed;
        }
        private void CancelAddingNewExpense(object sender, RoutedEventArgs e)
        {
            CreatingNewExpenseBtn.Visibility = Visibility.Visible;
            CreatingNewExpensePanel2.Visibility = Visibility.Collapsed;
            CreatingNewExpensePanel.Visibility = Visibility.Collapsed;
        }
        private void RemoveExpense(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("This expense will be deleted. Are you sure!", "Deleting Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                Button button = sender as Button;
                ExpensesManager.Remove(button.DataContext as Expense);
                ExpenseList_TotalExpense.Text = currencyConverter.Convert(ExpensesManager.TotalExpense, null, null, null).ToString();
            }
        }
        private void EditExpense(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ExpenseNeedUpdating = button.DataContext as Expense;
            NewExpense_Item.Text = ExpenseNeedUpdating.Item;
            NewExpense_Price.Text = ExpenseNeedUpdating.Price.ToString();
            NewExpense_Time.Text = dateTimeDisplay.Convert(ExpenseNeedUpdating.Time, null, null, null).ToString();
            int index = (int)ExpenseNeedUpdating.ExpenseType;
            if (index == 0) NewExpense_Type.SelectedIndex = NewExpense_Type.Items.Count - 1;
            else NewExpense_Type.SelectedIndex = index - 1;
            CreatingNewExpenseBtn.Visibility = Visibility.Collapsed;
            CreatingNewExpensePanel2.Visibility = Visibility.Visible;
            CreatingNewExpensePanel.Visibility = Visibility.Visible;
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
