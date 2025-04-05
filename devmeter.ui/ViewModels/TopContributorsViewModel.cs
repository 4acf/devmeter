using CommunityToolkit.Mvvm.ComponentModel;
using DevMeter.Core.Models;
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
            Row0Col0 = "-";
            Row0Col1 = "-";
            Row1Col0 = "-";
            Row1Col1 = "-";
            Row2Col0 = "-";
            Row2Col1 = "-";
            Row3Col0 = "-";
            Row3Col1 = "-";
            Row4Col0 = "-";
            Row4Col1 = "-";
            Row5Col0 = "-";
            Row5Col1 = "-";
            Row6Col0 = "-";
            Row6Col1 = "-";
        }

        public void Update(List<Contributor> topContributors)
        {
            int n = topContributors.Count;
            if(n > 0)
            {
                Row0Col0 = topContributors[0].Name;
                Row0Col1 = $"{String.Format($"{topContributors[0].Contributions:n0}")}";
            }
            if(n > 1)
            {
                Row1Col0 = topContributors[1].Name;
                Row1Col1 = $"{String.Format($"{topContributors[1].Contributions:n0}")}";
            }
            if (n > 2)
            {
                Row2Col0 = topContributors[2].Name;
                Row2Col1 = $"{String.Format($"{topContributors[2].Contributions:n0}")}";
            }
            if (n > 3)
            {
                Row3Col0 = topContributors[3].Name;
                Row3Col1 = $"{String.Format($"{topContributors[3].Contributions:n0}")}";
            }
            if (n > 4)
            {
                Row4Col0 = topContributors[4].Name;
                Row4Col1 = $"{String.Format($"{topContributors[4].Contributions:n0}")}";
            }
            if (n > 5)
            {
                Row5Col0 = topContributors[5].Name;
                Row5Col1 = $"{String.Format($"{topContributors[5].Contributions:n0}")}";
            }
            if (n > 6)
            {
                Row6Col0 = topContributors[6].Name;
                Row6Col1 = $"{String.Format($"{topContributors[6].Contributions:n0}")}";
            }
        }

    }
}
