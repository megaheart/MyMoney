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
        ExpensesManager ExpensesManager = ExpensesManager.GetExpensesManagerForMainThread();
        ObservableCollection<Expense> Expenses { get => ExpensesManager.Expenses; }
        IntToVNDCurrency currencyConverter = new IntToVNDCurrency();

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
                //ctrl.ExpensesViewer.ItemsSource = null;
                //ExpenseManager.ClearCache();
            }
            else
            {

                ctrl.ExpenseList_Name.Text = expenseList.Name;
                ctrl.ExpensesManager.Initialize(expenseList.Id);
                //ctrl.ExpensesViewer.ItemsSource = ctrl.Expenses;
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
            
        }
        
    }
}
