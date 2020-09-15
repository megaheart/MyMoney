using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MyMoney
{
    public class MyContext: Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<ExpenseList> ListOfExpenseList { set; get; }
        public DbSet<Expense> Expenses { set; get; }
        public DbSet<Cookie> Cookies { set; get; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source = D:\\Develop_Programs\\MyMoney.db; ");
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExpenseList>().HasKey(el => el.Id);
            modelBuilder.Entity<Cookie>().HasKey(c => c.Key);
            modelBuilder.Entity<Expense>().HasKey(e => e.Id);
        }
    }
}
