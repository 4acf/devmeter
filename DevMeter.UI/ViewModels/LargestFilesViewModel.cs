using CommunityToolkit.Mvvm.ComponentModel;
using DevMeter.Core.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.UI.ViewModels
{
    internal partial class LargestFilesViewModel : ViewModelBase
    {

        [ObservableProperty]
        private ObservableCollection<ISeries> _series;

        [ObservableProperty]
        private List<Axis> _xAxes;

        [ObservableProperty]
        private List<Axis> _yAxes;

        [ObservableProperty]
        private bool _isVisible;

        [ObservableProperty]
        private FindingStrategy _toolTipFindingStrategy;

        public LargestFilesViewModel()
        {

            IsVisible = false;

            Series = [
                new ColumnSeries<ObservableValue> {
                    IsVisible = false
                },
            ];

            XAxes = new List<Axis>
            {
                new Axis
                {
                    IsVisible = false
                }
            };

            YAxes = new List<Axis>
            {
                new Axis
                {
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("2a2b2d"))
                }
            };

            ToolTipFindingStrategy = FindingStrategy.ExactMatch;

        }

        public void Update(PriorityQueue<File, int> priorityQueue)
        {

            var newSeries = new ObservableCollection<ISeries>();
            var color = new SolidColorPaint(SKColor.Parse("#02b075"));
            while (priorityQueue.Count > 0)
            {
                var file = priorityQueue.Dequeue();
                newSeries.Add(
                    new ColumnSeries<ObservableValue>
                    {
                        Name = file.Name,
                        Values = [new ObservableValue(file.LinesOfCode)],
                        Fill = color,
                        MaxBarWidth = int.MaxValue,
                        XToolTipLabelFormatter = null,
                        Padding = 15
                    }
                );
            }

            Series.Clear();
            IsVisible = true;
            Series = newSeries;

        }

    }
}
