using Desktop.Commands;
using Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Desktop.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private string _accountId = "";
        private string _ownerInfo = "";
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
        public string OwnerInfo
        {
            get { return _ownerInfo; }
            set
            {
                _ownerInfo = value;
                OnPropertyChanged("OwnerInfo");
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
                      var payments = await SearchPayments.SearchAsync(AccountId);
                      if (payments != null)
                      {
                          FoundPaymentsVisibility = 0;
                          UpdatePayments(payments);
                          if (payments.Count > 0)
                          {
                              string name = string.IsNullOrEmpty(payments[0].FlatOwner) ?
                                 "\"Имя владельца\"" : payments[0].FlatOwner;
                              OwnerInfo = $"{name} (лицевой счет: {AccountId})";
                          }
                          else
                          {
                              OwnerInfo = "Платежи не найдены";
                          }
                      }
                      else
                      {
                          OwnerInfo = "";
                          UpdatePayments(null);
                          FoundPaymentsVisibility = 2;
                      }
                      RelayCommand.RaiseCanExecuteChanged();
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
                      await Task.Run(() => PrintPayments.Print(Payments.ToList()));
                      RelayCommand.RaiseCanExecuteChanged();
                  },
                  obj => !AppViewModel.IsAnyProcessing() && Payments.Count > 0));
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
