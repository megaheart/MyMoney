using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMoney
{
    public class Cookie
    {
        private static Cookie cookieForMainThread = new Cookie();
        public static Cookie ForMainThread() => cookieForMainThread;
        public static Cookie ForOtherThread() => new Cookie();
        private Cookie() {}
        public class KeyValuePair
        {
            public string Key { set; get; }
            public string Value { set; get; }
        }
        MyContext context = new MyContext();
        public string GetCookie(string key)
        {
            var output = context.Cookies.Where(c => c.Key == key);
            if (output.Count() == 0) return "";
            return output.First().Value;
        }
        public void SetCookie(string key, string value)
        {
            var x = context.Cookies.Where(c => c.Key == key);
            if (x.Count() == 0)
            {
                context.Cookies.Add(new KeyValuePair() { Key = key, Value = value });

            }
            else
            {
                x.First().Value = value;
            }
            context.SaveChangesAsync();
        }
    }

}
