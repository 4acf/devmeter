using CommunityToolkit.Mvvm.ComponentModel;
using DevMeter.Core.Models;
using DevMeter.Core.Processing.Formatting;
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
            TotalLines = string.Empty;
            TotalLinesExcludingWhitespace = string.Empty;
        }

        public void Update(int linesOfCode, int linesOfWhitespace)
        {
            TotalLines = StringFormatting.CommaString(linesOfCode + linesOfWhitespace);
            TotalLinesExcludingWhitespace = $"Excluding Whitespace: {StringFormatting.CommaString(linesOfCode)}";
        }

    }
}
