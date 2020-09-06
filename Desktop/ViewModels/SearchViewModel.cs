using Desktop.Commands;
using Desktop.Models;
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
        private int _foundReportsVisibility = 2;

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
        public int FoundReportsVisibility
        {
            get { return _foundReportsVisibility; }
            set
            {
                _foundReportsVisibility = value;
                OnPropertyChanged("FoundReportsVisibility");
            }
        }
        public ObservableCollection<Report> Reports { get; set; } = new ObservableCollection<Report>();

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
                          var reports = await SearchReportsCommand.SearchAsync(AccountId);
                          SearchCompleted(reports, "Не правильный формат");
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
                  obj => !TransitionToNewMonthCommand.Processing &&
                         !SearchReportsCommand.Processing &&
                         !PrintReportsCommand.Processing
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
                          await Task.Run(() => PrintReportsCommand.Print(Reports));
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
                  obj => !AppViewModel.IsAnyProcessing() && Reports.Count > 0));
            }
        }

        private void SearchCompleted(List<Report> reports, string errorMessage = "")
        {
            if (reports != null)
            {
                if (reports.Count > 0)
                {
                    string name = string.IsNullOrEmpty(reports[0].Owner) ?
                       "\"Имя владельца\"" : reports[0].Owner;
                    SearchInfo = $"{name} (лицевой счет: {AccountId})";
                    FoundReportsVisibility = 0;
                    UpdateReports(reports);
                }
                else
                {
                    SearchInfo = "Записи не найдены";
                    UpdateReports(null);
                    FoundReportsVisibility = 2;
                }
            }
            else
            {
                SearchInfo = errorMessage;
                UpdateReports(null);
                FoundReportsVisibility = 2;
            }
        }
        private void UpdateReports(List<Report> reports)
        {
            Reports.Clear();
            if (reports != null && reports.Count > 0)
            {
                foreach (Report report in reports)
                {
                    Reports.Add(report);
                }
            }
        }
    }
}
