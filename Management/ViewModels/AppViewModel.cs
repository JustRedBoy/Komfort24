using Management.Commands;
using Management.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Management.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        #region Fileds and Properties

        private string _accountId = "";
        private string _ownerInfo = "";
        private bool _isSearchEnabled = true;
        private bool _isFindPartEnabled = true;
        private bool _isPrintEnabled = true;
        //Collapsed	2	
        //Hidden	1	
        //Visible	0
        private int _foundPaymentsVisibility = 2;

        private int _generationProgressValue = 0;
        private int _maxGenerationProgressValue = 577;
        private string _generationMessage = 
            "* Во время генерации нельзя пользоваться операциями копировать/вставить";
        private bool _isGenerationEnabled = true;
        private string _generationButtonText = "Начать";
        private bool _isGenerationPartEnabled = true;

        private int _maxTransitionProgressValue = 3; 
        private int _transitionProgressValue = 0;
        private bool _isTransitionEnabled = true;
        private string _transitionMessage = "";
        private bool _isTransitionPartEnabled = true;

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
        public bool IsSearchEnabled
        {
            get { return _isSearchEnabled; }
            set
            {
                _isSearchEnabled = value;
                OnPropertyChanged("IsSearchEnabled");
            }
        }
        public bool IsFindPartEnabled
        {
            get { return _isFindPartEnabled; }
            set
            {
                _isFindPartEnabled = value;
                OnPropertyChanged("IsFindPartEnabled");
            }
        }
        public bool IsPrintEnabled
        {
            get { return _isPrintEnabled; }
            set
            {
                _isPrintEnabled = value;
                OnPropertyChanged("IsPrintEnabled");
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
        public ObservableCollection<Payment> Payments { get; set; } 
            = new ObservableCollection<Payment>();

        public int GenerationProgressValue
        {
            get { return _generationProgressValue; }
            set
            {
                _generationProgressValue = value;
                OnPropertyChanged("GenerationProgressValue");
            }
        }
        public int MaxGenerationProgressValue
        {
            get { return _maxGenerationProgressValue; }
            set
            {
                _maxGenerationProgressValue = value;
                OnPropertyChanged("MaxGenerationProgressValue");
            }
        }
        public string GenerationMessage
        {
            get { return _generationMessage; }
            set
            {
                _generationMessage = value;
                OnPropertyChanged("GenerationMessage");
            }
        }
        public bool IsGenerationEnabled
        {
            get { return _isGenerationEnabled; }
            set
            {
                _isGenerationEnabled = value;
                OnPropertyChanged("IsGenerationEnabled");
            }
        }
        public string GenerationButtonText
        {
            get { return _generationButtonText; }
            set
            {
                _generationButtonText = value;
                OnPropertyChanged("GenerationButtonText");
            }
        }
        public bool IsGenerationPartEnabled
        {
            get { return _isGenerationPartEnabled; }
            set
            {
                _isGenerationPartEnabled = value;
                OnPropertyChanged("IsGenerationPartEnabled");
            }
        }

        public int TransitionProgressValue
        {
            get { return _transitionProgressValue; }
            set
            {
                _transitionProgressValue = value;
                OnPropertyChanged("TransitionProgressValue");
            }
        }
        public int MaxTransitionProgressValue
        {
            get { return _maxTransitionProgressValue; }
            set
            {
                _maxTransitionProgressValue = value;
                OnPropertyChanged("MaxTransitionProgressValue");
            }
        }
        public bool IsTransitionEnabled
        {
            get { return _isTransitionEnabled; }
            set
            {
                _isTransitionEnabled = value;
                OnPropertyChanged("IsTransitionEnabled");
            }
        }
        public string TransitionMessage
        {
            get { return _transitionMessage; }
            set
            {
                _transitionMessage = value;
                OnPropertyChanged("TransitionMessage");
            }
        }
        public bool IsTransitionPartEnabled
        {
            get { return _isTransitionPartEnabled; }
            set
            {
                _isTransitionPartEnabled = value;
                OnPropertyChanged("IsTransitionPartEnabled");
            }
        }

        #endregion

        #region Commands

        private RalayCommand _searchCommand;
        public RalayCommand SearchCommand
        {
            get
            {
                return _searchCommand ??
                  (_searchCommand = new RalayCommand(async obj =>
                  {
                      if (TransitionToNewMonth.Processing ||
                          PrintPayments.Processing || 
                          SearchPayments.Processing) return;
                      IsSearchEnabled = false;
                      IsPrintEnabled = false;

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
                              if (!GenerationFlyers.Processing)
                              {
                                  IsPrintEnabled = true;
                              }
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
                      IsSearchEnabled = true;
                  }));
            }
        }

        private RalayCommand _printCommand;
        public RalayCommand PrintCommand
        {
            get
            {
                return _printCommand ??
                  (_printCommand = new RalayCommand(async obj =>
                  {
                      if (IsAnyProcessing()) return;
                      IsGenerationPartEnabled = false;
                      IsPrintEnabled = false;
                      IsSearchEnabled = false;
                      await Task.Run(() => PrintPayments.Print(Payments.ToList()));
                      IsGenerationPartEnabled = true;
                      IsPrintEnabled = true;
                      IsSearchEnabled = true;
                  }));
            }
        }

        private RalayCommand _generationCommand;
        public RalayCommand GenerationCommand
        {
            get
            {
                return _generationCommand ??
                  (_generationCommand = new RalayCommand(async obj =>
                  {
                      if (TransitionToNewMonth.Processing || PrintPayments.Processing) return;
                      if (GenerationFlyers.Processing)
                      {
                          GenerationMessage = "Отмена генерации ...";
                          IsGenerationEnabled = false;
                          GenerationFlyers.CancelGeneration();
                      }
                      else
                      {
                          GenerationButtonText = "Отменить";
                          GenerationMessage = "Подготовка к созданию листовок ...";
                          IsGenerationEnabled = false;
                          IsTransitionPartEnabled = false;
                          IsPrintEnabled = false;
                          GenerationFlyers.UpdateProgress += GenerateFlyers_UpdateProgress;
                          GenerationFlyers.CompletedGeneration += GenerateFlyers_CompletedGeneration;
                          await GenerationFlyers.StartGenerationAsync();
                      }
                  }));
            }
        }

        private RalayCommand _transitionCommand;
        public RalayCommand TransitionCommand
        {
            get
            {
                return _transitionCommand ??
                  (_transitionCommand = new RalayCommand(async obj =>
                  {
                      if (IsAnyProcessing()) return;
                      TransitionMessage = "Подготовка к переходу на новый месяц ...";
                      IsTransitionEnabled = false;
                      IsGenerationPartEnabled = false;
                      IsFindPartEnabled = false;
                      TransitionToNewMonth.UpdateProgress += TransitionToNewMonth_UpdateProgress;
                      TransitionToNewMonth.CompletedTransition += TransitionToNewMonth_CompletedTransition;
                      await TransitionToNewMonth.StartTransitionAsync();
                  }));
            }
        }

        #endregion

        #region Methods

        public bool CanExit() => !IsAnyProcessing();

        private bool IsAnyProcessing() => 
            GenerationFlyers.Processing ||
            TransitionToNewMonth.Processing ||
            SearchPayments.Processing ||
            PrintPayments.Processing;

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

        private void GenerateFlyers_CompletedGeneration(int result)
        {
            if (result == 0)
            {
                GenerationCompleted("Генерация листовок завершена");
            }
            else
            {
                GenerationCompleted("Генерация листовок отменена");
            }
        }
        private void GenerateFlyers_UpdateProgress(int value)
        {
            if (!GenerationFlyers.IsCancelled)
            {
                if (!IsGenerationEnabled && value == 1)
                {
                    IsGenerationEnabled = true;
                }
                GenerationProgressValue = value;
                GenerationMessage = $"Создание листовок ... {value} / {MaxGenerationProgressValue}";
            }
        }
        private void GenerationCompleted(string message)
        {
            IsGenerationEnabled = true;
            GenerationButtonText = "Начать";
            GenerationProgressValue = 0;
            GenerationMessage = message;
            IsTransitionPartEnabled = true;
            if (!SearchPayments.Processing && Payments.Count > 0)
            {
                IsPrintEnabled = true;
            }
            GenerationFlyers.UpdateProgress -= GenerateFlyers_UpdateProgress;
            GenerationFlyers.CompletedGeneration -= GenerateFlyers_CompletedGeneration;
        }

        private void TransitionToNewMonth_CompletedTransition(int value, string message)
        {
            IsTransitionEnabled = true;
            TransitionProgressValue = 0;
            TransitionMessage = message;
            IsGenerationPartEnabled = true;
            IsFindPartEnabled = true;
            TransitionToNewMonth.UpdateProgress -= TransitionToNewMonth_UpdateProgress;
            TransitionToNewMonth.CompletedTransition -= TransitionToNewMonth_CompletedTransition;
        }
        private void TransitionToNewMonth_UpdateProgress(int value, string message)
        {
            TransitionProgressValue = value;
            TransitionMessage = message;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
