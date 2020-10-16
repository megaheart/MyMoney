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

namespace MyMoney
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyContext myContext = new MyContext();
        ObservableCollection<ExpenseList> ExpenseLists;
        ObservableCollection<Expense> Expenses;
        IntToVNDCurrency currencyConverter = new IntToVNDCurrency();
        DateTimeToString dateTimeDisplay = new DateTimeToString();
        int totalExpense = 0;
        bool doesUserClick = false;
        public MainWindow()
        {
            InitializeComponent();
            var lists = myContext.ListOfExpenseList.AsNoTracking().ToList();
            lists.Reverse();
            ExpenseLists = new ObservableCollection<ExpenseList>(lists);
            ListOfExpenseListViewer.ItemsSource = ExpenseLists;
            var ExpenseListIdToOpen = Cookie.GetCookie(myContext, "ExpenseListIdToOpen");
            if(ExpenseListIdToOpen == "")
            {
                ListOfExpenseListViewer.SelectedIndex = 0;
            }
            else
            {
                int expectId = int.Parse(ExpenseListIdToOpen);
                int index = 0;
                while(index < ExpenseLists.Count)
                {
                    if (ExpenseLists[index].Id == expectId) break;
                    index++;
                }
                ListOfExpenseListViewer.SelectedIndex = index == ExpenseLists.Count? 0 : index;
            }
            doesUserClick = true;
        }
        private void CreatingButton_Click(object sender, RoutedEventArgs e)
        {
            NewExpenseListTitle.Text = "";
            CreatingButton.Visibility = Visibility.Collapsed;
            CreatingPanel.Visibility = Visibility.Visible;
        }
        private void AddNewExpenseList(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewExpenseListTitle.Text))
            {
                MessageBox.Show("Title Of New Expense List cannot be empty.");
                return;
            }
            var eX = new ExpenseList() { CreatedDate = DateTime.Now, Id = 0, Name = NewExpenseListTitle.Text };
            myContext.ListOfExpenseList.Add(eX);
            myContext.SaveChangesAsync();
            ExpenseLists.Insert(0, eX);
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
            if(ListOfExpenseListViewer.SelectedIndex == -1)
            {
                ExpenseList_Name.Text = "";
                ExpenseList_TotalExpense.Text = "";
                ExpensesViewer.ItemsSource = null;
                Expenses = null;
            }
            else
            {
                var expenseList = ExpenseLists[ListOfExpenseListViewer.SelectedIndex];
                ExpenseList_Name.Text = expenseList.Name;
                var _expenses = myContext.Expenses.Where(e => e.ExpenseListId == expenseList.Id).AsNoTracking().ToList();
                _expenses.Reverse();
                Expenses = new ObservableCollection<Expense>(_expenses);
                ExpensesViewer.ItemsSource = Expenses;
                totalExpense = 0;
                for (var i = 0; i < Expenses.Count; i++)
                {
                    totalExpense += Expenses[i].Price;
                }
                ExpenseList_TotalExpense.Text = currencyConverter.Convert(totalExpense, null, null, null).ToString();
            }
        }
        private void CreatingExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListOfExpenseListViewer.SelectedIndex == -1) return;
            NewExpense_Item.Text = "";
            NewExpense_Price.Text = "";
            NewExpense_Time.Text = "";
            CreatingNewExpenseBtn.Visibility = Visibility.Collapsed;
            CreatingNewExpensePanel2.Visibility = Visibility.Visible;
            CreatingNewExpensePanel.Visibility = Visibility.Visible;
        }
        private static bool IsNumber(char c) { int x = c - '0'; return x > -1 && x < 10; }
        private static bool TryParseDateTime1(string s, out DateTime o)
        {
            s = s.Trim();
            int[] colon_dot_dot_Position = new int[] { -1, -1, -1 };
            int dot_nth = 1;
            int i = 0;
            int[] num = new int[2];
            while (i < s.Length)
            {
                if (s[i] == ':')
                {
                    colon_dot_dot_Position[0] = i;
                    i++;
                    int num_nth = 0;
                    string a = "";
                    while (i < s.Length)
                    {
                        if (IsNumber(s[i]))
                        {
                            if (num_nth > 1) { o = new DateTime(); return false; }
                            a += s[i];
                        }
                        else if (s[i] == ' ')
                        {
                            if (a != "")
                            {
                                num[num_nth] = int.Parse(a);
                                a = "";
                                num_nth++;
                            }
                        }
                        else if (s[i] == '/')
                        {
                            if (a != "")
                            {
                                num[num_nth] = int.Parse(a);
                                a = "";
                                num_nth++;
                            }
                            if (num_nth < 2) { o = new DateTime(); return false; }
                            break;
                        }
                        else { o = new DateTime(); return false; }
                        i++;
                    }
                    if (i == s.Length)
                    {
                        if (a != "")
                        {
                            num[num_nth] = int.Parse(a);
                            a = "";
                            num_nth++;
                        }
                        if (num_nth > 1) { o = new DateTime(); return false; }
                        int x;
                        if (int.TryParse(s.Substring(0, colon_dot_dot_Position[0]).Trim(), out x))
                        {
                            if (x > 23 || x < 0) { o = new DateTime(); return false; }
                            if (num[0] > 59 || num[0] < 0) { o = new DateTime(); return false; }
                            o = DateTime.Now.Date.AddHours(x).AddMinutes(num[0]);
                            return true;
                        }
                    }
                }
                else if (s[i] == '/')
                {
                    if (dot_nth > 2) { o = new DateTime(); return false; }
                    colon_dot_dot_Position[dot_nth] = i;
                    dot_nth++;
                    i++;
                }
                else i++;
            }
            if (colon_dot_dot_Position[0] == -1)
            {
                int day, month, year;
                if (!int.TryParse(s.Substring(0, colon_dot_dot_Position[1]).Trim(), out day)) { o = new DateTime(); return false; }
                if (!int.TryParse(s.Substring(colon_dot_dot_Position[1] + 1, colon_dot_dot_Position[2] - colon_dot_dot_Position[1] - 1).Trim(), out month))
                { o = new DateTime(); return false; }
                if (!int.TryParse(s.Substring(colon_dot_dot_Position[2] + 1, s.Length - colon_dot_dot_Position[2] - 1).Trim(), out year))
                { o = new DateTime(); return false; }
                try
                {
                    o = new DateTime(year, month, day);
                }
                catch (Exception)
                {
                    o = new DateTime();
                    return false;
                }
                return true;
            }
            else
            {
                int month, year, hour;
                if (!int.TryParse(s.Substring(0, colon_dot_dot_Position[0]).Trim(), out hour)) { o = new DateTime(); return false; }
                if (!int.TryParse(s.Substring(colon_dot_dot_Position[1] + 1, colon_dot_dot_Position[2] - colon_dot_dot_Position[1] - 1).Trim(), out month))
                { o = new DateTime(); return false; }
                if (!int.TryParse(s.Substring(colon_dot_dot_Position[2] + 1, s.Length - colon_dot_dot_Position[2] - 1).Trim(), out year))
                { o = new DateTime(); return false; }
                try
                {
                    o = new DateTime(year, month, num[1], hour, num[0], 0);
                }
                catch (Exception)
                {
                    o = new DateTime();
                    return false;
                }
                return true;
            }
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
            DateTime expenseTime;
            var time = NewExpense_Time.Text;
            if (time == "") expenseTime = DateTime.Now;
            else
            {
                if (TryParseDateTime1(NewExpense_Time.Text, out expenseTime))
                {
                    
                }
                else if (DateTime.TryParse(NewExpense_Time.Text, out expenseTime))
                {
                    
                }
                else
                {
                    MessageBox.Show("Time Of New Expense is invalid.");
                    return;
                }
            }
            if(ExpenseNeedUpdating == null)
            {
                var eX = new Expense() { Item = NewExpense_Item.Text, Id = 0, ExpenseListId = ExpenseLists[ListOfExpenseListViewer.SelectedIndex].Id, Price = price, Time = expenseTime };
                myContext.Expenses.Add(eX);
                myContext.SaveChangesAsync();
                Expenses.Insert(0, eX);
                totalExpense += eX.Price;
                ExpenseList_TotalExpense.Text = currencyConverter.Convert(totalExpense, null, null, null).ToString();
            }
            else
            {
                var x = myContext.Expenses.Where(e => e.Id == ExpenseNeedUpdating.Id);
                var oldPrice = ExpenseNeedUpdating.Price;
                if(x.Count() != 0)
                {
                    var eX = x.First();
                    eX.Item = NewExpense_Item.Text;
                    eX.Price = price;
                    eX.Time = expenseTime;
                    myContext.SaveChangesAsync();
                    myContext.Entry(eX).State = EntityState.Detached;
                    totalExpense += eX.Price - oldPrice;
                    ExpenseNeedUpdating.Item = NewExpense_Item.Text;
                    ExpenseNeedUpdating.Price = price;
                    ExpenseNeedUpdating.Time = expenseTime;
                    ExpenseList_TotalExpense.Text = currencyConverter.Convert(totalExpense, null, null, null).ToString();
                }
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
            if(result == MessageBoxResult.OK)
            {
                Button button = sender as Button;
                var x = button.DataContext as Expense;
                myContext.Expenses.Remove(x);
                myContext.SaveChangesAsync();
                totalExpense -= x.Price;
                ExpenseList_TotalExpense.Text = currencyConverter.Convert(totalExpense, null, null, null).ToString();
                Expenses.Remove(x);
            }
        }
        private void EditExpense(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ExpenseNeedUpdating = button.DataContext as Expense;
            NewExpense_Item.Text = ExpenseNeedUpdating.Item;
            NewExpense_Price.Text = ExpenseNeedUpdating.Price.ToString();
            NewExpense_Time.Text = dateTimeDisplay.Convert(ExpenseNeedUpdating.Time, null, null, null).ToString();
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
}
