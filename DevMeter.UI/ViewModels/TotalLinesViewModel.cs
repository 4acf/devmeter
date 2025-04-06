using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.UI.ViewModels
{
    public partial class TotalLinesViewModel : ViewModelBase
    {

        [ObservableProperty]
        private string _totalLines;

        [ObservableProperty]
        private string _totalLinesExcludingWhitespace;

        public TotalLinesViewModel()
        {
            TotalLines = "-";
            TotalLinesExcludingWhitespace = "-";
        }

    }
}
