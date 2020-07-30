using Management.Operations;
using Management.ViewModels;
using System.Windows;

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
            if (GenerationFlyers.Processing || TransitionToNewMonth.Processing)
            {
                e.Cancel = true;
            }
        }
    }
}
