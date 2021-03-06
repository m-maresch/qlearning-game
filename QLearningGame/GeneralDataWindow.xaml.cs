﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace QLearningGame
{
    /// <summary>
    /// Interaction logic for GeneralDataWindow.xaml
    /// </summary>
    public partial class GeneralDataWindow : Window
    {
        public int TotalActions { get; set; }
        public SeriesCollection DataSeries { get; set; }

        /// <summary>
        /// Visualizes the given data
        /// </summary>
        /// <param name="dataToVisualize">Data to be visualized</param>
        public GeneralDataWindow(IEnumerable<int> dataToVisualize)
        {
            TotalActions = dataToVisualize.Sum();
            var seriesCollection = new SeriesCollection();
            var lineSeries = new LineSeries
            {
                AreaLimit = -10,
                Values = new ChartValues<ObservableValue>()
            };
            lineSeries.Values.AddRange(dataToVisualize.Select(x => new ObservableValue(x)));
            seriesCollection.Add(lineSeries);
            DataSeries = seriesCollection;
            InitializeComponent();
            DataContext = this;
        }
    }
}
