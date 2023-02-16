using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib
{
    internal interface ICustomer
    {
        uint? OpenAccount(string firstName, string lastName, float debtLimit);
        float Withdraw(uint account, float amount);
        void Deposit(uint account, float amount);
        string GetHistory(uint account);
        bool CloseAccount(uint account);
         
    }
}
