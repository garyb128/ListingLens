using ListingLens.ViewModels;

namespace ListingLens
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
