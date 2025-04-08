using CommunityToolkit.Mvvm.ComponentModel;
using DevMeter.Core.Models;
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

        public void Update(int linesOfCode, int linesOfWhitespace)
        {
            TotalLines = $"{String.Format($"{linesOfCode + linesOfWhitespace:n0}")}";
            TotalLinesExcludingWhitespace = $"Excluding Whitespace: {String.Format($"{linesOfCode:n0}")}";
        }

    }
}
