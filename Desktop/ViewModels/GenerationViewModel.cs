using Desktop.Commands;

namespace Desktop.ViewModels
{
    public class GenerationViewModel : ViewModelBase
    {
        private int _generationProgressValue = 0;
        private int _maxGenerationProgressValue = 577;
        private string _generationMessage =
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

        private RelayCommand _generationCommand;
        public RelayCommand GenerationCommand
        {
            get
            {
                return _generationCommand ??
                  (_generationCommand = new RelayCommand(async obj =>
                  {
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
                          GenerationFlyers.UpdateProgress += UpdateProgress;
                          GenerationFlyers.GenerationCompleted += GenerationCompleted;
                          await GenerationFlyers.StartGenerationAsync();
                      }
                  },
                  obj => (!TransitionToNewMonth.Processing &&
                          !PrintPayments.Processing) || GenerationFlyers.Processing
                  ));
            }
        }

        private void GenerationCompleted(int result)
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
        private void UpdateProgress(int value)
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
            GenerationFlyers.UpdateProgress -= UpdateProgress;
            GenerationFlyers.GenerationCompleted -= GenerationCompleted;
            RelayCommand.RaiseCanExecuteChanged();
        }
    }
}
