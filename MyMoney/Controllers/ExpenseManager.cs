using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MyMoney.Models;

namespace MyMoney.Controllers
{
    public class ExpensesManager
    {
        private MyContext myContext;
        public ObservableCollection<Expense> Expenses;
        private int totalExpense;
        public readonly TotalExpenseOfAType[] totalExpenseOfAllTypes = new TotalExpenseOfAType[]
        {
            new TotalExpenseOfAType()
            {
                Name = "Others",
                Value = 0
            },
            new TotalExpenseOfAType()
            {
                Name = "Book(s)",
                Value = 0
            },
            new TotalExpenseOfAType()
            {
                Name = "Cooking Material",
                Value = 0
            },
            new TotalExpenseOfAType()
            {
                Name = "Food Eaten Out",
                Value = 0
            },
            new TotalExpenseOfAType()
            {
                Name = "Appliance(s)",
                Value = 0
            },
        };
        public int TotalExpense { get => totalExpense; }
        public ExpensesManager(MyContext context)
        {
            myContext = context;
        }
        public void Initialize(int expenseListId)
        {
            var _expenses = myContext.Expenses.Where(e => e.ExpenseListId == expenseListId).AsNoTracking().ToList();
            _expenses.Reverse();
            Expenses = new ObservableCollection<Expense>(_expenses);
            totalExpense = 0;
            for (var i = 0; i < totalExpenseOfAllTypes.Length; i++)
            {
                totalExpenseOfAllTypes[i].Value = 0;
            }
            for (var i = 0; i < Expenses.Count; i++)
            {
                totalExpense += Expenses[i].Price;
                totalExpenseOfAllTypes[(int)Expenses[i].ExpenseType].Value += Expenses[i].Price;
            }
        }
        public void ClearCache()
        {
            Expenses = null;
            GC.Collect();
        }
        public Exception Add(string itemName, int expenseListId, int price, string time, int expenseTypeIndex)
        {
            DateTime expenseTime;
            if(!TryParseDateTime(time, out expenseTime))
                return new Exception("Time Of New Expense is invalid.");
            var eX = new Expense() { Item = itemName, Id = 0, ExpenseListId = expenseListId, Price = price, Time = expenseTime, ExpenseType=(ExpenseType)expenseTypeIndex };
            myContext.Expenses.Add(eX);
            myContext.SaveChangesAsync();
            Expenses.Insert(0, eX);
            totalExpense += eX.Price;
            totalExpenseOfAllTypes[(int)eX.ExpenseType].Value += eX.Price;
            return null;
        }
        public Exception Update(Expense expenseNeedUpdating, string itemName, int price, string time, int expenseTypeIndex)
        {
            DateTime expenseTime;
            if (!TryParseDateTime(time, out expenseTime))
                return new Exception("Time Of New Expense is invalid.");
            var x = myContext.Expenses.Where(e => e.Id == expenseNeedUpdating.Id);
            var oldPrice = expenseNeedUpdating.Price;
            var oldType = expenseNeedUpdating.ExpenseType;
            if (x.Count() != 0)
            {
                var eX = x.First();
                eX.Item = itemName;
                eX.Price = price;
                eX.Time = expenseTime;
                eX.ExpenseType = (ExpenseType)expenseTypeIndex;
                myContext.SaveChangesAsync();
                myContext.Entry(eX).State = EntityState.Detached;
                totalExpense += eX.Price - oldPrice;
                if(oldType == eX.ExpenseType)
                {
                    totalExpenseOfAllTypes[(int)eX.ExpenseType].Value += eX.Price - oldPrice;
                }
                else
                {
                    totalExpenseOfAllTypes[(int)oldType].Value -= oldPrice;
                    totalExpenseOfAllTypes[(int)eX.ExpenseType].Value += eX.Price;
                }
                expenseNeedUpdating.Item = itemName;
                expenseNeedUpdating.Price = price;
                expenseNeedUpdating.Time = expenseTime;
                expenseNeedUpdating.ExpenseType = eX.ExpenseType;

            }
            return null;
        }
        public void Remove(Expense x)
        {
            myContext.Expenses.Remove(x);
            myContext.SaveChangesAsync();
            totalExpense -= x.Price;
            totalExpenseOfAllTypes[(int)x.ExpenseType].Value -= x.Price;
            Expenses.Remove(x);
        }
        private static bool IsNumber(char c) { int x = c - '0'; return x > -1 && x < 10; }
        private static bool TryParseDateTime(string s, out DateTime o)
        {
            if (s == "")
            {
                o = DateTime.Now;
                return true;
            }
            else
            {
                if (TryParseDateTime_customize1(s, out o))
                {
                    return true;
                }
                else if (DateTime.TryParse(s, out o))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private static bool TryParseDateTime_customize1(string s, out DateTime o)
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
    }
}
