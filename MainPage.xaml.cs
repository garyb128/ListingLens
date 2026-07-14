using eBaySoldItemAnalyser.ViewModels;

namespace eBaySoldItemAnalyser
{
    public partial class MainPage : ContentPage
    {
        public MainPage(SearchViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }

    }
}
