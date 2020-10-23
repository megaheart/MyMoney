using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MyMoney
{
    public class ExpenseListManager
    {
        private MyContext myContext;
        public ObservableCollection<ExpenseList> ExpenseLists { get; private set; }
        public ExpenseListManager(MyContext context)
        {
            myContext = context;
        }
        public void Initialize()
        {
            var lists = myContext.ListOfExpenseList.AsNoTracking().ToList();
            lists.Reverse();
            ExpenseLists = new ObservableCollection<ExpenseList>(lists);
        }
        public Exception Add(string name)
        {
            if (string.IsNullOrEmpty(name)) return new Exception("Title Of New Expense List cannot be empty.");
            var eX = new ExpenseList() { CreatedDate = DateTime.Now, Id = 0, Name = name };
            myContext.ListOfExpenseList.Add(eX);
            myContext.SaveChangesAsync();
            ExpenseLists.Insert(0, eX);
            return null;
        }
    }
}
