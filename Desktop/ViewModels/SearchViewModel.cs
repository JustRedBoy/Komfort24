using Desktop.Commands;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Desktop.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private string _accountId = "";
        private string _searchInfo = "";
        //Collapsed	2	
        //Hidden	1	
        //Visible	0
        private int _foundPaymentsVisibility = 2;

        public string AccountId
        {
            get { return _accountId; }
            set
            {
                _accountId = value;
                OnPropertyChanged("AccountId");
            }
        }
        public string SearchInfo
        {
            get { return _searchInfo; }
            set
            {
                _searchInfo = value;
                OnPropertyChanged("SearchInfo");
            }
        }
        public int FoundPaymentsVisibility
        {
            get { return _foundPaymentsVisibility; }
            set
            {
                _foundPaymentsVisibility = value;
                OnPropertyChanged("FoundPaymentsVisibility");
            }
        }
        public ObservableCollection<Payment> Payments { get; set; } = new ObservableCollection<Payment>();

        private RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        {
            get
            {
                return _searchCommand ??
                  (_searchCommand = new RelayCommand(async obj =>
                  {
                      try
                      {
                          var payments = await SearchPayments.SearchAsync(AccountId);
                          SearchCompleted(payments, "Не правильный формат");
                      }
                      catch (Exception e)
                      {
                          SearchCompleted(null, e.Message);
                      }
                      finally
                      {
                          RelayCommand.RaiseCanExecuteChanged();
                      }
                  },
                  obj => !TransitionToNewMonth.Processing &&
                         !SearchPayments.Processing &&
                         !PrintPayments.Processing
                  ));
            }
        }

        private RelayCommand _printCommand;
        public RelayCommand PrintCommand
        {
            get
            {
                return _printCommand ??
                  (_printCommand = new RelayCommand(async obj =>
                  {
                      try
                      {
                          await Task.Run(() => PrintPayments.Print(Payments.ToList()));
                      }
                      catch (Exception e)
                      {
                          SearchInfo = "Не удается распечатать файл. " + e.Message;
                      }
                      finally
                      {
                          RelayCommand.RaiseCanExecuteChanged();
                      }
                  },
                  obj => !AppViewModel.IsAnyProcessing() && Payments.Count > 0));
            }
        }

        private void SearchCompleted(List<Payment> payments, string errorMessage = "")
        {
            if (payments != null)
            {
                FoundPaymentsVisibility = 0;
                UpdatePayments(payments);
                if (payments.Count > 0)
                {
                    string name = string.IsNullOrEmpty(payments[0].FlatOwner) ?
                       "\"Имя владельца\"" : payments[0].FlatOwner;
                    SearchInfo = $"{name} (лицевой счет: {AccountId})";
                }
                else
                {
                    SearchInfo = "Платежи не найдены";
                }
            }
            else
            {
                SearchInfo = errorMessage;
                UpdatePayments(null);
                FoundPaymentsVisibility = 2;
            }
        }
        private void UpdatePayments(List<Payment> payments)
        {
            Payments.Clear();
            if (payments != null && payments.Count > 0)
            {
                foreach (Payment payment in payments)
                {
                    Payments.Add(payment);
                }
            }
        }
    }
}
