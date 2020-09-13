using Desktop.Commands;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<ArchiveReport> Reports { get; set; } = new ObservableCollection<ArchiveReport>();

        private RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        {
            get
            {
                return _searchCommand ??= new RelayCommand(async obj =>
                {
                    try
                    {
                        if (!SearchReportsCommand.HaveReportsInfo())
                        {
                            SearchInfo = "Загрузка информации ...";
                        }
                        var reports = await SearchReportsCommand.SearchAsync(AccountId);
                        SearchCompleted(reports);
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
                       !PrintReportsCommand.Processing);
            }
        }

        private RelayCommand _printCommand;
        public RelayCommand PrintCommand
        {
            get
            {
                return _printCommand ??= new RelayCommand(async obj =>
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
                obj => !AppViewModel.IsAnyProcessing() && Reports.Count > 0);
            }
        }

        private void SearchCompleted(List<ArchiveReport> reports, string errorMessage = null)
        {
            if (reports != null)
            {
                if (reports.Count > 0)
                {
                    string owner = string.IsNullOrEmpty(reports[0].Owner) ?
                       "\"Имя владельца\"" : reports[0].Owner;
                    SearchInfo = $"{owner} (лицевой счет: {AccountId})";
                    FoundReportsVisibility = 0;
                    UpdateReports(reports);
                    return;
                }
                SearchInfo = "Записи не найдены";
            }
            else
            {
                SearchInfo = errorMessage ?? "Не правильный формат";
            }
            UpdateReports(null);
            FoundReportsVisibility = 2;
        }

        private void UpdateReports(List<ArchiveReport> reports)
        {
            Reports.Clear();
            if (reports != null && reports.Count > 0)
            {
                foreach (ArchiveReport report in reports)
                {
                    Reports.Add(report);
                }
            }
        }
    }
}
