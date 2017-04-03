using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HttpClientApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            string result1 = Helper.PostJsonData("http://127.0.0.1:8080", "{" + "'FirstName':'Kenan','LastName':'Hancer'" + "}\r\n\r\n").Result;

            string result2 = Helper.RequestPage("http://127.0.0.1:8080").Result;

            string result3 = Helper.RequestPage("http://127.0.0.1:8080/Hello").Result;

            string result4 = Helper.RequestPage("http://127.0.0.1:8080/Account/Hasan/40").Result;

            Console.WriteLine(result1);

            Console.WriteLine(result2);

            Console.WriteLine(result3);

            Console.WriteLine(result4);

            int count = 10000;

            ParallelQuery<Task> taskList = ParallelEnumerable.Range(0, count)
                                                 .WithDegreeOfParallelism(Environment.ProcessorCount)
                                                 .Select(async index =>
                                                 {
                                                     string result = await Helper.RequestPage("http://127.0.0.1:8080/Account/Enes/4").ConfigureAwait(false);
                                                 });

            Stopwatch sw = Stopwatch.StartNew();

            Task.WhenAll(taskList.ToArray()).Wait();

            sw.Stop();

            Console.WriteLine($"{count} requests are responded in {sw.Elapsed}");

            Console.ReadKey();
        }
    }
}