using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMoney
{
    public class Cookie
    {
        public string Key { set; get; }
        public string Value { set; get; }
        public static string GetCookie(MyContext context, string key)
        {
            var output = context.Cookies.Where(c => c.Key == key);
            if (output.Count() == 0) return "";
            return output.First().Value;
        }
        public static void SetCookie(MyContext context, string key, string value)
        {
            var x = context.Cookies.Where(c => c.Key == key);
            if (x.Count() == 0) {
                context.Cookies.Add(new Cookie() { Key = key, Value = value });
                
            }
            else
            {
                x.First().Value = value;
            }
            context.SaveChangesAsync();
        }
    }
}
