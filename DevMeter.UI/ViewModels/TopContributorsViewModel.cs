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
    public partial class TopContributorsViewModel : ViewModelBase
    {

        //TODO: REFACTOR TO AN ARRAY
        [ObservableProperty]
        private string _row0Col0;
        [ObservableProperty]
        private string _row0Col1;
        [ObservableProperty]
        private string _row1Col0;
        [ObservableProperty]
        private string _row1Col1;
        [ObservableProperty]
        private string _row2Col0;
        [ObservableProperty]
        private string _row2Col1;
        [ObservableProperty]
        private string _row3Col0;
        [ObservableProperty]
        private string _row3Col1;
        [ObservableProperty]
        private string _row4Col0;
        [ObservableProperty]
        private string _row4Col1;
        [ObservableProperty]
        private string _row5Col0;
        [ObservableProperty]
        private string _row5Col1;
        [ObservableProperty]
        private string _row6Col0;
        [ObservableProperty]
        private string _row6Col1;

        public TopContributorsViewModel()
        {
            Row0Col0 = string.Empty;
            Row0Col1 = string.Empty;
            Row1Col0 = string.Empty;
            Row1Col1 = string.Empty;
            Row2Col0 = string.Empty;
            Row2Col1 = string.Empty;
            Row3Col0 = string.Empty;
            Row3Col1 = string.Empty;
            Row4Col0 = string.Empty;
            Row4Col1 = string.Empty;
            Row5Col0 = string.Empty;
            Row5Col1 = string.Empty;
            Row6Col0 = string.Empty;
            Row6Col1 = string.Empty;
        }

        public void Update(List<Contributor> topContributors)
        {
            int n = topContributors.Count;
            if(n > 0)
            {
                Row0Col0 = topContributors[0].Name;
                Row0Col1 = $"{StringFormatting.CommaString(topContributors[0].Contributions)}";
            }
            if(n > 1)
            {
                Row1Col0 = topContributors[1].Name;
                Row1Col1 = $"{StringFormatting.CommaString(topContributors[1].Contributions)}";
            }
            if (n > 2)
            {
                Row2Col0 = topContributors[2].Name;
                Row2Col1 = $"{StringFormatting.CommaString(topContributors[2].Contributions)}";
            }
            if (n > 3)
            {
                Row3Col0 = topContributors[3].Name;
                Row3Col1 = $"{StringFormatting.CommaString(topContributors[3].Contributions)}";
            }
            if (n > 4)
            {
                Row4Col0 = topContributors[4].Name;
                Row4Col1 = $"{StringFormatting.CommaString(topContributors[4].Contributions)}";
            }
            if (n > 5)
            {
                Row5Col0 = topContributors[5].Name;
                Row5Col1 = $"{StringFormatting.CommaString(topContributors[5].Contributions)}";
            }
            if (n > 6)
            {
                Row6Col0 = topContributors[6].Name;
                Row6Col1 = $"{StringFormatting.CommaString(topContributors[6].Contributions)}";
            }
        }

    }
}
