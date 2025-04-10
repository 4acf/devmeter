using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using DevMeter.Core.Github.Models.Json;

namespace DevMeter.UI.ViewModels
{
    internal partial class RecentActivityViewModel : ViewModelBase
    {

        [ObservableProperty]
        private ObservableCollection<ISeries> _series;

        [ObservableProperty]
        private List<Axis> _yAxes;

        [ObservableProperty]
        private bool _isVisible;

        [ObservableProperty]
        private FindingStrategy _toolTipFindingStrategy;

        public RecentActivityViewModel()
        {
            IsVisible = false;

            Series = [
                new ColumnSeries<ObservableValue>
                {
                    IsVisible = false
                },
            ];

            YAxes = new List<Axis>
            {
                new Axis
                {
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("2a2b2d"))
                }
            };

            ToolTipFindingStrategy = FindingStrategy.ExactMatch;
        }

        public void Update(List<GitHubCommit> recentCommits)
        {
            var now = DateTime.UtcNow;
            var values = new int[31];
            foreach (var commit in recentCommits)
            {
                int difference = (now - commit.GetDate()).Days;
                values[difference] += 1;
            }
            var reversed = values.Reverse().ToArray();
            var observableValues = new ObservableValue[reversed.Length];
            for (int i = 0; i < reversed.Length; i++)
            {
                observableValues[i] = new ObservableValue(reversed[i]);
            }

            var newSeries = new ObservableCollection<ISeries>();
            var color = SKColor.Parse("#02b075");
            newSeries.Add(
                new LineSeries<ObservableValue>
                {
                    Values = observableValues,
                    Stroke = new SolidColorPaint(color) { StrokeThickness = 4 },
                    GeometryStroke = null,
                    GeometrySize = 0,
                    LineSmoothness = 0,
                    EnableNullSplitting = false,
                    Fill = new LinearGradientPaint(
                        [color.WithAlpha(255), color.WithAlpha(0)],
                        new SKPoint(0.5f, 0),
                        new SKPoint(0.5f, 0.3f)
                    )
                }
            );

            Series.Clear();
            IsVisible = true;
            Series = newSeries;

        }

    }
}
