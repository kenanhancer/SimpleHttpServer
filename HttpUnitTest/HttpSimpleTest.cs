using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HttpUnitTest
{
    public class HttpSimpleTest
    {
        [Fact]
        public void TestMethod1()
        {
            Task<string> t1 = Helper.PostJsonData("http://127.0.0.1:8080");

            string result = t1.Result;
        }

        [Fact]
        public void TestMethod2()
        {
            Task<string> t1 = Helper.RequestPage("http://127.0.0.1:8080");

            string result = t1.Result;
        }

        [Fact]
        public void TestMethod3()
        {
            Task<string> t1 = Helper.RequestPage("http://127.0.0.1:8080/Hello");

            string result = t1.Result;
        }

        [Fact]
        public void TestMethod4()
        {
            Task<string> t1 = Helper.RequestPage("http://127.0.0.1:8080/Account");

            string result = t1.Result;
        }

        [Fact]
        public void TestMethod5()
        {
            int count = 10000;

            ParallelQuery<Task> taskList = ParallelEnumerable.Range(0, count)
                                                 .WithDegreeOfParallelism(Environment.ProcessorCount)
                                                 .Select(async index =>
                                                 {
                                                     string result = await Helper.RequestPage("http://127.0.0.1:8080").ConfigureAwait(false);
                                                 });

            Task.WhenAll(taskList.ToArray()).Wait();
        }

        [Fact]
        public void TestMethod6()
        {
            int count = 10000;

            ParallelQuery<Task> taskList = ParallelEnumerable.Range(0, count)
                                                 .WithDegreeOfParallelism(Environment.ProcessorCount)
                                                 .Select(async index =>
                                                 {
                                                     string result = await Helper.RequestPage("http://127.0.0.1:8080/Account").ConfigureAwait(false);


                                                 });

            Task.WhenAll(taskList.ToArray()).Wait();
        }
    }
}