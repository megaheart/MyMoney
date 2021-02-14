using MyMoney.Controllers;
using MyMoney.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyMoney.Viewers
{
    /// <summary>
    /// Interaction logic for AddingExpenseControl.xaml
    /// </summary>
    public partial class AddingExpenseControl : UserControl
    {
       
        public ExpensesManager ExpensesManager;
        DateTimeToString dateTimeDisplay = new DateTimeToString();
        public AddingExpenseControl()
        {
            ExpensesManager = ExpensesManager.GetExpensesManagerForMainThread();
            InitializeComponent();
            var list = new TotalExpenseOfAType[ExpensesManager.totalExpenseOfAllTypes.Length];
            list[list.Length - 1] = ExpensesManager.totalExpenseOfAllTypes[0];
            Array.Copy(ExpensesManager.totalExpenseOfAllTypes, 1, list, 0, list.Length - 1);
            NewExpense_Type.ItemsSource = list.Select(t =>
            {
                var index = t.Name.IndexOf('(');
                if (index > -1) return t.Name.Remove(index).Trim();
                return t.Name.Trim();
            });
        }
        public static readonly RoutedEvent FinishedEvent = EventManager.RegisterRoutedEvent("Finished", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddingExpenseControl));
        public event RoutedEventHandler Finished
        {
            add { AddHandler(FinishedEvent, value); }
            remove { RemoveHandler(FinishedEvent, value); }
        }
        private ExpenseList _expenseList;
        public void AddNewExpenseTo(ExpenseList list)
        {
            _expenseList = list;
            NewExpense_Item.Text = "";
            NewExpense_Price.Text = "";
            NewExpense_Time.Text = "";
            NewExpense_Type.SelectedIndex = NewExpense_Type.Items.Count - 1;
        }
        private Expense ExpenseNeedUpdating = null;
        public void EditExpense(Expense expense)
        {
            ExpenseNeedUpdating = expense;
            NewExpense_Item.Text = ExpenseNeedUpdating.Item;
            NewExpense_Price.Text = ExpenseNeedUpdating.Price.ToString();
            NewExpense_Time.Text = dateTimeDisplay.Convert(ExpenseNeedUpdating.Time, null, null, null).ToString();
            int index = (int)ExpenseNeedUpdating.ExpenseType;
            if (index == 0) NewExpense_Type.SelectedIndex = NewExpense_Type.Items.Count - 1;
            else NewExpense_Type.SelectedIndex = index - 1;
        }
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
                if (_expenseList == null) throw new Exception("Please use AddNewExpenseTo(ExpenseList list) method before open.");
                Exception exception = ExpensesManager.Add(NewExpense_Item.Text, _expenseList.Id, price, NewExpense_Time.Text, expenseTypeIndex);
                if (exception != null) MessageBox.Show(exception.Message);
            }
            else
            {
                Exception exception = ExpensesManager.Update(ExpenseNeedUpdating, NewExpense_Item.Text, price, NewExpense_Time.Text, expenseTypeIndex);
                if (exception != null) MessageBox.Show(exception.Message);
                ExpenseNeedUpdating = null;
            }

        }
        private void CancelAddingNewExpense(object sender, RoutedEventArgs e)
        {
            CreatingNewExpensePanel2.Visibility = Visibility.Collapsed;
        }
    }
}
