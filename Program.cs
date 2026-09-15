
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static readonly object balanceLock = new object();

    static int balance = 10000;

    static void Main()
    {
        Console.WriteLine("===== C# MULTITHREADING PROJECT =====");

        ThreadDemo();

        ThreadPoolDemo();

        LockDemo();

        TaskDemo();

        ParallelDemo();

        AsyncAwaitDemo().GetAwaiter().GetResult();

        Console.WriteLine("\n===== PROJECT COMPLETED =====");
    }

    
    // 1. THREAD + START + JOIN + SLEEP
    

    static void ThreadDemo()
    {
        Console.WriteLine("\n===== 1. THREAD DEMO =====");

        Thread workerThread = new Thread(ProcessOrders);

        Console.WriteLine("Main thread starting worker thread...");

        workerThread.Start();

        workerThread.Join();

        Console.WriteLine("Worker thread completed.");
        Console.WriteLine("Main thread continues.");
    }

    static void ProcessOrders()
    {
        for (int i = 1; i <= 5; i++)
        {
            Console.WriteLine(
                $"Thread {Thread.CurrentThread.ManagedThreadId} processing Order {i}");

            Thread.Sleep(500);
        }
    }


    
    // 2. THREAD POOL
   

    static void ThreadPoolDemo()
    {
        Console.WriteLine("\n===== 2. THREAD POOL DEMO =====");

        for (int i = 1; i <= 3; i++)
        {
            int orderId = i;

            ThreadPool.QueueUserWorkItem(state =>
            {
                Console.WriteLine(
                    $"ThreadPool processing Order {orderId} " +
                    $"on Thread {Thread.CurrentThread.ManagedThreadId}");

                Thread.Sleep(500);

                Console.WriteLine(
                    $"Order {orderId} completed");
            });
        }

        Thread.Sleep(2000);
    }


    
    // 3. LOCK / THREAD SYNCHRONIZATION
    

    static void LockDemo()
    {
        Console.WriteLine("\n===== 3. LOCK DEMO =====");

        balance = 10000;

        Thread customer1 = new Thread(() => Withdraw(7000));
        Thread customer2 = new Thread(() => Withdraw(7000));

        customer1.Start();
        customer2.Start();

        customer1.Join();
        customer2.Join();

        Console.WriteLine($"Final Balance: {balance}");
    }

    static void Withdraw(int amount)
    {
        lock (balanceLock)
        {
            Console.WriteLine(
                $"Thread {Thread.CurrentThread.ManagedThreadId} " +
                $"checking balance...");

            Thread.Sleep(500);

            if (balance >= amount)
            {
                balance -= amount;

                Console.WriteLine(
                    $"Thread {Thread.CurrentThread.ManagedThreadId} " +
                    $"withdrawn ₹{amount}");

                Console.WriteLine(
                    $"Remaining Balance: ₹{balance}");
            }
            else
            {
                Console.WriteLine(
                    $"Thread {Thread.CurrentThread.ManagedThreadId} " +
                    $"failed. Insufficient balance.");
            }
        }
    }


    
    // 4. TASK
    

    static void TaskDemo()
    {
        Console.WriteLine("\n===== 4. TASK DEMO =====");

        Task task1 = Task.Run(() =>
        {
            Console.WriteLine(
                $"Task 1 running on Thread " +
                $"{Thread.CurrentThread.ManagedThreadId}");

            Thread.Sleep(1000);

            Console.WriteLine("Task 1 completed.");
        });

        Task task2 = Task.Run(() =>
        {
            Console.WriteLine(
                $"Task 2 running on Thread " +
                $"{Thread.CurrentThread.ManagedThreadId}");

            Thread.Sleep(1000);

            Console.WriteLine("Task 2 completed.");
        });

        Task.WaitAll(task1, task2);

        Console.WriteLine("Both tasks completed.");
    }


    
    // 5. PARALLEL PROGRAMMING / TPL
    

    static void ParallelDemo()
    {
        Console.WriteLine("\n===== 5. PARALLEL PROGRAMMING =====");

        List<int> orders = new List<int>
        {
            101, 102, 103, 104, 105
        };

        Parallel.ForEach(orders, orderId =>
        {
            Console.WriteLine(
                $"Processing Order {orderId} " +
                $"on Thread {Thread.CurrentThread.ManagedThreadId}");

            Thread.Sleep(500);

            Console.WriteLine(
                $"Order {orderId} completed.");
        });

        Console.WriteLine("All orders completed.");
    }


    
    // 6. ASYNC / AWAIT
    

    static async Task AsyncAwaitDemo()
    {
        Console.WriteLine("\n===== 6. ASYNC / AWAIT =====");

        Console.WriteLine("Sending request to server...");

        string result = await GetOrderFromServer();

        Console.WriteLine($"Server Response: {result}");
    }

    static async Task<string> GetOrderFromServer()
    {
        await Task.Delay(2000);

        return "Order details received successfully";
    }


    
    // 7. DEADLOCK EXAMPLE
    

    static void DeadlockExample()
    {
        object lock1 = new object();
        object lock2 = new object();

        Thread thread1 = new Thread(() =>
        {
            lock (lock1)
            {
                Thread.Sleep(100);

                lock (lock2)
                {
                    Console.WriteLine("Thread 1 completed");
                }
            }
        });

        Thread thread2 = new Thread(() =>
        {
            lock (lock2)
            {
                Thread.Sleep(100);

                lock (lock1)
                {
                    Console.WriteLine("Thread 2 completed");
                }
            }
        });

        thread1.Start();
        thread2.Start();

        thread1.Join();
        thread2.Join();
    }
}
