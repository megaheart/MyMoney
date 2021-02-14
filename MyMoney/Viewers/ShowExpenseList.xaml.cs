using MyMoney.Controllers;
using MyMoney.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ShowExpenseList.xaml
    /// </summary>
    public partial class ShowExpenseList : UserControl
    {
        IntToVNDCurrency currencyConverter = new IntToVNDCurrency();
        DateTimeToString dateTimeDisplay = new DateTimeToString();
        ObservableCollection<Expense> Expenses { get => ExpensesManager.Expenses; }
        public static readonly DependencyProperty ExpenseListIdProperty = DependencyProperty.Register("ExpenseListIdProperty", typeof(ExpenseList), typeof(ShowExpenseList),
                                                                                                        new PropertyMetadata(null, ExpenseListIdPropertyChanged));
        private static void ExpenseListIdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShowExpenseList ctrl = d as ShowExpenseList;
            ExpenseList expenseList = e.NewValue as ExpenseList;
            if (expenseList == null)
            {
                ctrl.ExpensesViewer.ItemsSource = null;
                //ExpenseManager.ClearCache();
            }
            else
            {
                ctrl.ExpensesManager.Initialize(expenseList.Id);
                ctrl.ExpensesViewer.ItemsSource = ctrl.Expenses;
            }
        }
        public ExpenseList ExpenseList
        {
            set => SetValue(ExpenseListIdProperty, value);
            get => (ExpenseList)GetValue(ExpenseListIdProperty);
        }
        public ExpensesManager ExpensesManager { set; get; }
        public ShowExpenseList()
        {
            InitializeComponent();
            var list = new TotalExpenseOfAType[ExpensesManager.totalExpenseOfAllTypes.Length];
            list[list.Length - 1] = ExpensesManager.totalExpenseOfAllTypes[0];
            Array.Copy(ExpensesManager.totalExpenseOfAllTypes, 1, list, 0, list.Length - 1);
            TotalExpenseOfAllTypeViewer.ItemsSource = list;
        }
        private void CreatingExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExpenseList == null) return;
            
        }
        private void RemoveExpense(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("This expense will be deleted. Are you sure!", "Deleting Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                Button button = sender as Button;
                ExpensesManager.Remove(button.DataContext as Expense);
                
            }
        }
        private void EditExpense(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            
            
        }
    }
}
