using System.Windows;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace ADCProcessorTrinity
{
    /// <summary>
    /// Логика взаимодействия для SinGraphDrawerWindow.xaml
    /// </summary>
    public partial class SinGraphDrawerWindow : Window
    {
        private double[] voltages;
        private short[] adcCodes;

        public SinGraphDrawerWindow(short[] adcCodes, double[] voltages)
        {
            InitializeComponent();
            this.adcCodes = adcCodes;
            this.voltages = voltages;
        }

        // Показ графика кодов АЦП
        private void ShowAdcCodesGraph()
        {
            var plotModel = new PlotModel { Title = "График кодов АЦП" };

            var series = new LineSeries();
            for (int i = 0; i < adcCodes.Length; i++)
            {
                series.Points.Add(new DataPoint(i, adcCodes[i]));
            }

            plotModel.Series.Add(series);
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Отсчеты" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Коды АЦП" });

            plotView.Model = plotModel;
        }

        // Показ графика напряжений
        private void ShowVoltagesGraph()
        {
            var plotModel = new PlotModel { Title = "График напряжения" };

            var series = new LineSeries();
            for (int i = 0; i < voltages.Length; i++)
            {
                series.Points.Add(new DataPoint(i, voltages[i]));
            }

            plotModel.Series.Add(series);
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Отсчеты" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Напряжение (мВ)" });

            plotView.Model = plotModel;
        }

        private void btn_ShowADC_Click(object sender, RoutedEventArgs e)
        {
            ShowAdcCodesGraph();
        }

        private void btn_ShowVoltage_Click(object sender, RoutedEventArgs e)
        {
            ShowVoltagesGraph();
        }
    }
}
