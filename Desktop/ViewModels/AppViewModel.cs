using Desktop.Commands;

namespace Desktop.ViewModels
{
    public class AppViewModel
    {
        public GenerationViewModel GenerationViewModel { get; set; }
        public TransitionViewModel TransitionViewModel { get; set; }
        public SearchViewModel SearchViewModel { get; set; }

        public AppViewModel()
        {
            GenerationViewModel = new GenerationViewModel();
            TransitionViewModel = new TransitionViewModel();
            SearchViewModel = new SearchViewModel();
        }

        public static bool IsAnyProcessing() => 
            GenerationFlyers.Processing ||
            TransitionToNewMonth.Processing ||
            SearchReports.Processing ||
            PrintPayments.Processing;
    }
}
