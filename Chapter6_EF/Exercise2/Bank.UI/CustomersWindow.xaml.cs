using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bank.AppLogic.Contracts.DataAccess;
using Bank.Domain;

namespace Bank.UI
{
    public partial class CustomersWindow : Window
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IWindowDialogService _windowDialogService;
        private Customer _newCustomer;
        private readonly IReadOnlyList<City> _allCities;
        private readonly ObservableCollection<Customer> _allCustomers;
        
        public CustomersWindow(ICustomerRepository customerRepository,
            ICityRepository cityRepository,
            IWindowDialogService windowDialogService)
        {
            InitializeComponent();

            _customerRepository = customerRepository;
            _windowDialogService = windowDialogService;

            _allCustomers = new ObservableCollection<Customer>(_customerRepository.GetAllWithAccounts());
            CustomersListView.ItemsSource = _allCustomers;

            _allCities = cityRepository.GetAllOrderedByZipCode();
            CityComboBox.ItemsSource = _allCities;

            _newCustomer = new Customer
            {
                ZipCode = _allCities.First().ZipCode
            };
            NewCustomerGroupBox.DataContext = _newCustomer;
        }

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            ClearError();

            Result validationResult = _newCustomer.Validate(_allCities);
            if (!validationResult.IsSuccess)
            {
                ShowError($"Cannot add new customer. {validationResult.Message}");
            }
            else
            {
                _customerRepository.Add(_newCustomer);
                _allCustomers.Add(_newCustomer);

                _newCustomer = new Customer
                {
                    ZipCode = _allCities.First().ZipCode
                };
                NewCustomerGroupBox.DataContext = _newCustomer;
            }
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

        private void ShowAccountsButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)e.Source;
            Customer selectedCustomer = (Customer)clickedButton.Tag;
            _windowDialogService.ShowAccountDialogForCustomer(selectedCustomer);
        }
    }
}
