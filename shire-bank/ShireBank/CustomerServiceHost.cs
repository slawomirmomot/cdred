using System;
using System.ServiceModel;
using SharedInterface;

namespace ShireBank
{
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults = true
    )]
    public class CustomerServiceHost : ICustomerInterface
    {
        public uint? OpenAccount(string firstName, string lastName, float debtLimit)
        {
            return null;
            throw new NotImplementedException();
        }

        public float Withdraw(uint account, float amount)
        {
            throw new NotImplementedException();
        }

        public void Deposit(uint account, float amount)
        {

            throw new NotImplementedException();
        }

        public string GetHistory(uint account)
        {
            
            throw new NotImplementedException();
        }

        public bool CloseAccount(uint account)
        {
            throw new NotImplementedException();
        }
    }
}