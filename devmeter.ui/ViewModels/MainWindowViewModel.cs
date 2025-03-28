using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using devmeter.core.Github;
using devmeter.core.Processing;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml;

namespace devmeter.ui.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {

        [ObservableProperty]
        private string? _searchString;

        [RelayCommand]
        private void Search()
        {
            
            if(InputParser.TryParse(SearchString, out var path) == false)
            {
                return;
            }

            
            

        }

    }
}
