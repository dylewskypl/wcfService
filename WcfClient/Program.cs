﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace WcfClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Task> tasks = new List<Task>();
            int time = 0;
            Task.Factory.StartNew(() => 
            {
                while (true)
                {
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    Console.WriteLine("Count " + ThreadPool.ThreadCount);
                    Console.WriteLine("Pending work " + ThreadPool.PendingWorkItemCount);
                    ThreadPool.GetAvailableThreads(out int worker, out int io);
                    Console.WriteLine("work " + worker + " io " + io);
                    time++;
                }
            });

            ThreadPool.SetMinThreads(117, 1);

            GC.TryStartNoGCRegion(10000, true);
            for (int i = 0; i < 100; i++)
            {
                string t = i.ToString();
                /// wołania synchroniczne klasyczne
                Task task = Task.Factory.StartNew(() =>
                {
                    var client = new TestClient.ContractClient(TestClient.ContractClient.EndpointConfiguration.BasicHttpBinding_Contract);
                    var result = client.GetData(new TestClient.GetDataRequest(t));
                    Console.WriteLine(result.GetDataResult);
                });
                tasks.Add(task);

                /// wołanie do soap async
                //tasks.Add(client.GetDataAsync(new TestClient.GetDataRequest(t)).ContinueWith(x =>
                //{
                //    Console.WriteLine(x.GetAwaiter().GetResult().GetDataResult);
                //}));
            }
            if (GCSettings.LatencyMode == GCLatencyMode.NoGCRegion)
                GC.EndNoGCRegion();

            Console.Write("all task started");
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("all task ended");
            Console.WriteLine("execution time " + time);
        }
    }
}
