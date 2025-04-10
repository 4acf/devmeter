using CommunityToolkit.Mvvm.ComponentModel;
using DevMeter.Core.Processing.Formatting;

namespace DevMeter.UI.ViewModels
{
    internal partial class TotalLinesViewModel : ViewModelBase
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
