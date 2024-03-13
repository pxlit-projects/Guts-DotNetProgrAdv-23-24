using System.Windows;
using Bank.AppLogic;
using Bank.AppLogic.Contracts.DataAccess;
using Bank.Infrastructure;

namespace Bank.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var context = new BankContext();
            context.CreateOrUpdateDatabase();

            ICityRepository cityRepository = new CityRepository(context);
            ICustomerRepository customerRepository = new CustomerRepository(context);
            IAccountRepository accountRepository = new AccountRepository(context);

            var accountService = new AccountService(accountRepository);

            var dialogService = new WindowDialogService(accountRepository, accountService);

            var customersWindow = new CustomersWindow(customerRepository, cityRepository, dialogService);
            customersWindow.Show();
        }
    }
}
