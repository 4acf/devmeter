using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmeter.ui.ViewModels
{
    public partial class TotalContributorsViewModel : ViewModelBase
    {

        [ObservableProperty]
        private string _totalContributors;

        [ObservableProperty]
        private string _bottomRowContent;

        public TotalContributorsViewModel()
        {
            _totalContributors = "-";
            _bottomRowContent = string.Empty;
        }

    }
}
