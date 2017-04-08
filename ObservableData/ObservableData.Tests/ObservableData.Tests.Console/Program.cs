using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using ObservableDataQuerying.Utils;

namespace ObservableDataQuerying.Tests.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Do();
            while (true)
            {
                System.Console.ReadLine();
            }
        }

        public static async void Do()
        {
            var subject = new Subject<DateTime>();

            var select = subject.Select(x =>
            {
                Debug.WriteLine("select");
                return x.ToString();
            });
            var sub = select
                .WeakSubscribe(
                    x =>
                    {
                        Debug.WriteLine(x.ToString());
                    }
                );
            var sub2 = select.WeakSubscribe(
                x =>
                {
                    Debug.WriteLine("empty");
                }
            );

            while (true)
            {
                subject.OnNext(DateTime.Now);
                //sub?.Dispose();
                //sub = null;
                GC.Collect();
                await Task.Delay(1000);
            }
        }
    }
}
