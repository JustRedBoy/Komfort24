using Management.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Management.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AppViewModel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!((AppViewModel)DataContext).CanExit())
            {
                e.Cancel = true;
            }
        }

        private void OnKeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var viewModel = (AppViewModel)DataContext;
                if (viewModel.SearchCommand.CanExecute(null))
                {
                    viewModel.SearchCommand.Execute(null);
                }
            }
        }
    }
}
