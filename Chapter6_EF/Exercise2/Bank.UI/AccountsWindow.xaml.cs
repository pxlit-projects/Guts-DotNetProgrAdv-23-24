using System.Windows;
using System.Windows.Controls;
using Bank.AppLogic;
using Bank.AppLogic.Contracts;
using Bank.Domain;

namespace Bank.UI
{
    public partial class AccountsWindow : Window
    {
        private readonly Customer _customer;
        private readonly IAccountService _accountService;
        private readonly IWindowDialogService _windowDialogService;

        public AccountsWindow(Customer customer,
            IAccountService accountService,
            IWindowDialogService windowDialogService)
        {
            InitializeComponent();

            _customer = customer;
            _accountService = accountService;
            _windowDialogService = windowDialogService;

            Title = $"Accounts of {customer.FirstName} {customer.Name}";

            AccountsListView.ItemsSource = _customer.Accounts;

            TypeComboBox.SelectedIndex = 0;
        }

        private void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            ClearError();

            string accountNumber = AccountNumberTextBox.Text;
            AccountType type = (AccountType)TypeComboBox.SelectedValue;
            Result result = _accountService.AddNewAccountForCustomer(_customer, accountNumber, type);
            if (result.IsSuccess)
            {
                AccountsListView.Items.Refresh();
                AccountNumberTextBox.Text = string.Empty;
            }
            else
            {
                ShowError(result.Message);
            }
        }

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)e.Source;
            Account selectedAccount = (Account)clickedButton.Tag;
            _windowDialogService.ShowTransferDialog(selectedAccount, _customer.Accounts);
        }

        private void ClearError()
        {
            ErrorTextBlock.Visibility = Visibility.Hidden;
            ErrorTextBlock.Text = "";
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Visibility = Visibility.Visible;
            ErrorTextBlock.Text = message;
        }
    }
}
