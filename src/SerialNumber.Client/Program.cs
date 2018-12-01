using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SerialNumber.Client
{
    class Program
    {
        const int initializeAttemptsBeforeFailing = 5;
        private static int attempt = 0;

        static async Task Main(string[] args)
        {
            Console.Title = "Client";
            try
            {
                Console.WriteLine("Press any key to begin.");
                Console.ReadKey();

                for (int i = 0; i < 1; i++)
                {
                    // 并发启动多线程
                    var thread = new Thread(new ThreadStart(Test1));
                    thread.Start();
                    // 并发启动多线程
                    var thread1 = new Thread(new ThreadStart(Test1));
                    thread1.Start();
                    // 并发启动多线程
                    var thread2 = new Thread(new ThreadStart(Test2));
                    thread2.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
            Console.Read();
        }

        private static void Test1()
        {
            RunTest().ConfigureAwait(false);
        }

        private static async Task RunTest()
        {
            var num = 10_000;
            var ts = new Stopwatch();
            ts.Start();
            using (var client = await StartClientWithRetries())
            {
                for (int i = 0; i < num; i++)
                {
                    await DoClientWork(client, "测试示例-test");
                }
            }
            ts.Stop();
            Console.WriteLine($"生成{num}个序号，耗时：{ts.ElapsedMilliseconds}");
        }


        private static void Test2()
        {
            RunTest2().ConfigureAwait(false);
        }

        private static async Task RunTest2()
        {
            var num = 10_000;
            var ts = new Stopwatch();
            ts.Start();
            using (var client = await StartClientWithRetries())
            {
                for (int i = 0; i < num; i++)
                {
                    await DoClientWork(client, "测试示例-test2");
                }
            }
            ts.Stop();
            Console.WriteLine($"test2生成{num}个序号，耗时：{ts.ElapsedMilliseconds}");
        }

        private static async Task<IClusterClient> StartClientWithRetries()
        {
            attempt = 0;
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "SerialNumberApp";
                })
                //.ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect(RetryFilter);
            Console.WriteLine("Client successfully connect to silo host");
            return client;
        }

        private static async Task<bool> RetryFilter(Exception exception)
        {
            if (exception.GetType() != typeof(SiloUnavailableException))
            {
                Console.WriteLine($"Cluster client failed to connect to cluster with unexpected error.  Exception: {exception}");
                return false;
            }
            attempt++;
            Console.WriteLine($"Cluster client attempt {attempt} of {initializeAttemptsBeforeFailing} failed to connect to cluster.  Exception: {exception}");
            if (attempt > initializeAttemptsBeforeFailing)
            {
                return false;
            }
            await Task.Delay(TimeSpan.FromSeconds(4));
            return true;
        }

        private static async Task DoClientWork(IClusterClient client, string primaryKey)
        {
            // example of calling grains from the initialized client
            var serialNumberService = client.GetGrain<SerialNumber.Interfaces.ISerialNumberService>(primaryKey);
            var response = await serialNumberService.GetSerialNumber();
            Console.WriteLine(response);
        }
    }
}
