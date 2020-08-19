using Desktop.Commands;
using System;

namespace Desktop.ViewModels
{
    public class GenerationViewModel : ViewModelBase
    {
        private int _generationProgressValue = 0;
        private int _maxGenerationProgressValue = 577;
        private string _generationInfo =
            "* Во время генерации нельзя пользоваться операциями копировать/вставить";
        private bool _isGenerationEnabled = true;
        private string _generationButtonText = "Начать";

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
        public string GenerationInfo
        {
            get { return _generationInfo; }
            set
            {
                _generationInfo = value;
                OnPropertyChanged("GenerationInfo");
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

        private RelayCommand _generationCommand;
        public RelayCommand GenerationCommand
        {
            get
            {
                return _generationCommand ??
                  (_generationCommand = new RelayCommand(async obj =>
                  {
                      if (!GenerationFlyers.Processing)
                      {
                          try
                          {
                              GenerationButtonText = "Отменить";
                              GenerationInfo = "Подготовка к созданию листовок ...";
                              IsGenerationEnabled = false;
                              GenerationFlyers.UpdateProgress += UpdateProgress;
                              bool isSuccessful = await GenerationFlyers.StartGenerationAsync();
                              GenerationCompleted(isSuccessful ? "Генерация листовок завершена"
                                  : "Генерация листовок отменена");
                          }
                          catch (Exception e)
                          {
                              GenerationCompleted("Произошла ошибка, повторите операцию! " + e.Message);
                          }
                          finally
                          {
                              RelayCommand.RaiseCanExecuteChanged();
                          }
                      }
                      else
                      {
                          GenerationInfo = "Отмена генерации ...";
                          IsGenerationEnabled = false;
                          GenerationFlyers.CancelGeneration();
                      }
                  },
                  obj => (!TransitionToNewMonth.Processing &&
                          !PrintPayments.Processing) || GenerationFlyers.Processing
                  ));
            }
        }

        private void UpdateProgress(int value)
        {
            if (!GenerationFlyers.IsCancelled)
            {
                if (!IsGenerationEnabled && value == 1)
                {
                    IsGenerationEnabled = true;
                }
                GenerationProgressValue = value;
                GenerationInfo = $"Создание листовок ... {value} / {MaxGenerationProgressValue}";
            }
        }
        private void GenerationCompleted(string message)
        {
            IsGenerationEnabled = true;
            GenerationButtonText = "Начать";
            GenerationProgressValue = 0;
            GenerationInfo = message;
            GenerationFlyers.UpdateProgress -= UpdateProgress;
        }
    }
}
