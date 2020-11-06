using MyMoney.Controllers;
using MyMoney.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ExpenseListViewer.xaml
    /// </summary>
    public partial class ExpenseListViewer : UserControl
    {
        IntToVNDCurrency currencyConverter = new IntToVNDCurrency();
        DateTimeToString dateTimeDisplay = new DateTimeToString();
        ObservableCollection<Expense> Expenses { get => ExpensesManager.Expenses; }
        public MyContext myContext { set; get; }
        public ExpensesManager ExpensesManager { set; get; }
        public static readonly DependencyProperty ExpenseListIdProperty = DependencyProperty.Register("ExpenseListIdProperty", typeof(ExpenseList), typeof(ExpenseListViewer),
                                                                                                        new PropertyMetadata(null, ExpenseListIdPropertyChanged));
        private static void ExpenseListIdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExpenseListViewer ctrl = d as ExpenseListViewer;
            ExpenseList expenseList = e.NewValue as ExpenseList;
            if (expenseList == null)
            {
                ctrl.ExpenseList_Name.Text = "";
                ctrl.ExpenseList_TotalExpense.Text = "";
                ctrl.ExpensesViewer.ItemsSource = null;
                //ExpenseManager.ClearCache();
            }
            else
            {

                ctrl.ExpenseList_Name.Text = expenseList.Name;
                ctrl.ExpensesManager.Initialize(expenseList.Id);
                ctrl.ExpensesViewer.ItemsSource = ctrl.Expenses;
                ctrl.ExpenseList_TotalExpense.Text = ctrl.currencyConverter.Convert(ctrl.ExpensesManager.TotalExpense, null, null, null).ToString();

            }
        }
        public ExpenseList ExpenseList
        {
            set => SetValue(ExpenseListIdProperty, value);
            get => (ExpenseList)GetValue(ExpenseListIdProperty);
        }
        public ExpenseListViewer()
        {
            InitializeComponent();
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
        }
        private void CreatingExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExpenseList == null) return;
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
                Exception exception = ExpensesManager.Add(NewExpense_Item.Text, ExpenseList.Id, price, NewExpense_Time.Text, expenseTypeIndex);
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
}
