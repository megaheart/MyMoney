using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MyMoney.Models;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace MyMoney.Controllers
{
    public class ExpensesManager
    {
        public ObservableCollection<Expense> Expenses;
        //private int totalExpense;
        //public int TotalExpense { get => totalExpense; }
        private static ExpensesManager ExpensesManagerForMainThread = new ExpensesManager();
        public static ExpensesManager GetExpensesManagerForMainThread()
        {
            return ExpensesManagerForMainThread;
        }
        public static ExpensesManager GetExpensesManagerForOtherThread()
        {
            return new ExpensesManager();
        }
        private ExpensesManager()
        {
        }
        //class ExpenseComparer : IComparer<Expense>
        //{
        //    public int Compare([AllowNull] Expense x, [AllowNull] Expense y)
        //    {
        //        if (x == null || y == null) throw new Exception();
        //        return y.Time.CompareTo(x.Time);
        //    }
        //}
        //public void Initialize(int expenseListId)
        //{
        //    if (expenseListId == -1) throw new Exception("expenseListId of ExpensesManager.Initialize(int expenseListId) must be natural number.");
        //    if (expenseListId == _expenseListId) return;
        //    _expenseListId = expenseListId;
        //    var _expenses = myContext.Expenses.Where(e => e.ExpenseListId == expenseListId).AsNoTracking().ToList();
        //    _expenses.Sort(new ExpenseComparer());
        //    Expenses = new ObservableCollection<Expense>(_expenses);
        //    totalExpense = 0;
        //    for (var i = 0; i < totalExpenseOfAllTypes.Length; i++)
        //    {
        //        totalExpenseOfAllTypes[i].Value = 0;
        //    }
        //    for (var i = 0; i < Expenses.Count; i++)
        //    {
        //        totalExpense += Expenses[i].Price;
        //        totalExpenseOfAllTypes[(int)Expenses[i].ExpenseType].Value += Expenses[i].Price;
        //    }
        //}
        public Exception Add(string itemName, int expenseListId, int price, string time, int expenseTypeIndex)
        {
            DateTime expenseTime;
            if(!TryParseDateTime(time, out expenseTime))
                return new Exception("Time Of New Expense is invalid.");
            var eX = new Expense() { Item = itemName, Id = 0, ExpenseListId = expenseListId, Price = price, Time = expenseTime, ExpenseType=(ExpenseType)expenseTypeIndex };
            myContext.Expenses.Add(eX);
            myContext.SaveChangesAsync();
            int i = 0;
            while(i < Expenses.Count && eX.Time < Expenses[i].Time)
            {
                i++;
            }
            Expenses.Insert(i, eX);
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
                int oldI = Expenses.IndexOf(expenseNeedUpdating);
                if (oldI > 0 && expenseNeedUpdating.Time == Expenses[oldI - 1].Time) return null;
                int i = 0;
                bool isBehindOldIndex = false;
                while (expenseNeedUpdating.Time < Expenses[i].Time || expenseNeedUpdating == Expenses[i])
                {
                    if (i >= Expenses.Count) break;
                    if (expenseNeedUpdating == Expenses[i]) isBehindOldIndex = true;
                    i++;
                }
                if (i > 0 && expenseNeedUpdating == Expenses[i - 1]) return null;
                if(isBehindOldIndex) Expenses.Move(oldI, i - 1); 
                else Expenses.Move(oldI, i);
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
