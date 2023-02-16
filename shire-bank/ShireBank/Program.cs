using System;
using System.ServiceModel;
using SharedInterface;

namespace ShireBank
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var serviceHost = new ServiceHost(new CustomerServiceHost(), new Uri(Constants.BankBaseAddress));
            serviceHost.AddServiceEndpoint(typeof(ICustomerInterface), new BasicHttpBinding(), Constants.ServiceName);
            serviceHost.Open();
            Console.ReadKey();
        }
    }
}