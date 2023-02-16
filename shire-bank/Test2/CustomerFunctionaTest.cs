using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
namespace Test2
{
    internal class CustomerFunctionaTest
    {
        private readonly ILogger<CustomerFunctionaTest> _logger;
        


        public CustomerFunctionaTest(ILogger<CustomerFunctionaTest> logger)
        {
            _logger = logger;
            
        } 

        public void DoAction(string name)
        {
            // The port number must match the port of the gRPC server.
            
            //using var channel = GrpcChannel.ForAddress("https://localhost:7212");
            //

                var channel = GrpcChannel.ForAddress("https://localhost:7212");
            
                var client = new Greeter.GreeterClient(channel);

                //ManualResetEvent[] endOfWorkEvents = Enumerable.Repeat<ManualResetEvent>(new ManualResetEvent(false), 2).ToArray();
                ManualResetEvent[] endOfWorkEvents =
                                   { new ManualResetEvent(false), new ManualResetEvent(false),new ManualResetEvent(false) };
                var historyPrintLock = new object();

                // Customer 1
                new Thread(async () =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    var accountId = await client.OpenAccountAsync(new OpenAccountRequest { FirstName = "Henrietta", LastName = "Baggins", DebtLimit = 100.0f });

                    if (accountId != null)
                    {
                        await client.DepositAsync(new DepositRequest { Account = accountId.Value, Ammount = 500.0f });

                        Thread.Sleep(TimeSpan.FromSeconds(2));

                        await client.DepositAsync(new DepositRequest { Account = accountId.Value, Ammount = 500.0f });
                        await client.DepositAsync(new DepositRequest { Account = accountId.Value, Ammount = 1000.0f });

                        var withDrawResult = await client.WithdrawAsync(new WithdrawRequest { Account = accountId.Value, Ammount = 2000.0f });
                        if (2000.0f != withDrawResult.Value)
                        {
                            Console.WriteLine("=== Customer 1 === Can't withdraw a valid amount");
                        }

                        lock (historyPrintLock)
                        {
                            Console.WriteLine("=== Customer 1 ===");
                            Console.WriteLine(client.GetHistory(accountId));
                        }

                        var closeAccountResult = await client.CloseAccountAsync(accountId);
                        if (!closeAccountResult.Value)
                        {
                            Console.WriteLine("=== Customer 1 === Failed to close account");
                        }
                    }
                    else
                    {
                        Console.WriteLine("=== Customer 1 === Failed to open account");
                    }
                    endOfWorkEvents[0].Set();
                }).Start();

                // Customer 2
                new Thread(async () =>
                {

                    var accountId = await client.OpenAccountAsync(new OpenAccountRequest { FirstName = "Barbara", LastName = "Tuk", DebtLimit = 50.0f });
                    if (accountId == null)
                    {

                        Console.WriteLine("=== Customer 2 === Failed to open account");
                        //throw new Exception("Failed to open account");
                    }
                    else
                    {

                        if (await client.OpenAccountAsync(new OpenAccountRequest { FirstName = "Barbara", LastName = "Tuk", DebtLimit = 500.0f }) != null)
                        {
                            Console.WriteLine("=== Customer 2 === Opened account for the same name twice");
                            //throw new Exception("Opened account for the same name twice!");
                        }

                        var withdrawResult = await client.WithdrawAsync(new WithdrawRequest { Account = accountId.Value, Ammount = 2000.0f });
                        if (50.0f != withdrawResult.Value)
                        {
                            Console.WriteLine("=== Customer 2 === Can only borrow up to debit limit only");
                            //throw new Exception("Can only borrow up to debit limit only");
                        }

                        Thread.Sleep(TimeSpan.FromSeconds(10));

                        if ((await client.CloseAccountAsync(accountId)).Value)
                        {
                            Console.WriteLine("=== Customer 2 === Can't close the account with outstanding debt");
                            //throw new Exception("Can't close the account with outstanding debt");
                        }

                        await client.DepositAsync(new DepositRequest { Account = accountId.Value, Ammount = 100.0f });
                        if ((await client.CloseAccountAsync(accountId)).Value)
                        {
                            Console.WriteLine("=== Customer 2 === Can't close the account before clearing all funds");
                            //throw new Exception("Can't close the account before clearing all funds");
                        }

                        if (50.0f != (await client.WithdrawAsync(new WithdrawRequest { Account = accountId.Value, Ammount = 50.0f })).Value)
                        {
                            Console.WriteLine("=== Customer 2 === Can't withdraw a valid amount");
                            //throw new Exception("Can't withdraw a valid amount");
                        }

                        lock (historyPrintLock)
                        {
                            Console.WriteLine("=== Customer 2 ===");
                            Console.WriteLine(client.GetHistory(accountId));
                        }

                        if (!(await client.CloseAccountAsync(accountId)).Value)
                        {
                            Console.WriteLine("=== Customer 2 === Failed to close account");
                            //throw new Exception("Failed to close account");
                        }
                    }
                    endOfWorkEvents[1].Set();
                }).Start();


                // Customer 3
                new Thread(async () =>
                {

                    var accountId = await client.OpenAccountAsync(new OpenAccountRequest { FirstName = "Gandalf", LastName = "Grey", DebtLimit = 10000.0f });
                    if (accountId == null)
                    {
                        Console.WriteLine("=== Customer 2 === Failed to open account");
                    }
                    else
                    {
                        var toProcess = 200;
                        var resetEvent = new ManualResetEvent(false);

                        for (var i = 0; i < 100; i++)
                        {
                            ThreadPool.QueueUserWorkItem(async stateInfo =>
                            {
                                if ((await client.WithdrawAsync(new WithdrawRequest { Account = accountId.Value, Ammount = 10.0f })).Value != 10.0f)
                                {
                                    Console.WriteLine("=== Customer 2 === Can't withdraw a valid amount!");
                                }

                                if (Interlocked.Decrement(ref toProcess) == 0)
                                {
                                    resetEvent.Set();
                                }
                            });
                        }

                        for (var i = 0; i < 100; i++)
                        {
                            ThreadPool.QueueUserWorkItem(async stateInfo =>
                            {
                                await client.DepositAsync(new DepositRequest { Account = accountId.Value, Ammount = 10.0f });
                                if (Interlocked.Decrement(ref toProcess) == 0)
                                {
                                    resetEvent.Set();
                                }
                            });
                        }

                        Thread.Sleep(TimeSpan.FromSeconds(10));

                        resetEvent.WaitOne();

                        lock (historyPrintLock)
                        {
                            Console.WriteLine("=== Customer 3 ===");
                            Console.WriteLine(client.GetHistory(accountId));
                        }

                        if (!(await client.CloseAccountAsync(accountId)).Value)
                        {
                            Console.WriteLine("=== Customer 3 === Failed to close account");
                            //throw new Exception("Failed to close account");
                        }
                    }
                    endOfWorkEvents[2].Set();
                }).Start();
                 WaitHandle.WaitAll(endOfWorkEvents);
                _logger.LogDebug(20, "Doing hard work! {Action}", name);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
        }
        }
    
}
