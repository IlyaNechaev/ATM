using ATMApplication.Models;
using ATMApplication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Services
{
    public class MyBankService : IBankService
    {
        IRepositoryFactory RepositoryFactory { get; init; }
        public MyBankService(IRepositoryFactory repositoryFactory)
        {
            RepositoryFactory = repositoryFactory;
        }

        public void TransferMoney(Card source, Card dest)
        {

            throw new NotImplementedException();
        }
    }
}
