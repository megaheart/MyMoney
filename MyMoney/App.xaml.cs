using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMoney
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            MainWindow window = new MainWindow();
            window.Show();
            //MyContext context = new MyContext();
            //Console.WriteLine("start");
            //var eX = new Expense() { ExpenseListId = 1, Id = 0, Item = "hello0", Price = 500, Time = DateTime.Now };
            //context.Expenses.Add(eX);
            ////var x = context.Expenses
            ////    .Where(e => true).ForEachAsync(e => Console.WriteLine(e.Price));
            //Console.WriteLine(eX.Id);
            //context.SaveChanges();
            //Console.WriteLine(eX.Id);
            //Console.WriteLine("end");
        }
    }
}
