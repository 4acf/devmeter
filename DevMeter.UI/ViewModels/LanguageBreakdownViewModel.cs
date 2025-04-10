using CommunityToolkit.Mvvm.ComponentModel;
using DevMeter.Core.Utils;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DevMeter.UI.ViewModels
{
    internal partial class LanguageBreakdownViewModel : ViewModelBase
    {

        [ObservableProperty]
        private ObservableCollection<ISeries> _series;

        public LanguageBreakdownViewModel()
        {
            Series = [
                new PieSeries<ObservableValue>
                {
                    IsVisible = false
                },
            ];
        }

        public void Update(Dictionary<string, long> languages)
        {

            long total = 0;
            foreach (var kvp in languages)
            {
                total += kvp.Value;
            }

            var newSeries = new ObservableCollection<ISeries>();
            foreach (var kvp in languages)
            {
                if (!Filetypes.Colors.TryGetValue(kvp.Key, out var languageColor))
                {
                    languageColor = "#fdfdfd";
                }

                var color = SKColor.Parse(languageColor);

                newSeries.Add(
                    new PieSeries<ObservableValue>
                    {
                        Name = kvp.Key,
                        Values = [new ObservableValue(((float)kvp.Value / (float)total) * 100)],
                        Fill = new SolidColorPaint(color),
                        MaxRadialColumnWidth = 50,
                        ToolTipLabelFormatter = (point) => $"{$"{(point?.StackedValue?.Share * 100):F2}%"}"
                    }
                );
            }

            Series.Clear();
            Series = newSeries;

        }

    }
}
