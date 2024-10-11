using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ADCProcessorTrinity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string? filePath;
        private bool SaveTxtOnly;
        private bool SaveBinOnly;

        public MainWindow()
        {
            InitializeComponent();
            radbtn_SaveTxtOnly.Checked += radbtn_SaveTxtOnly_Checked;
            radbtn_SaveBinaryOnly.Checked += radbtn_SaveBinaryOnly_Checked;
            btn_Process.IsEnabled = false;
        }

        private void btn_FileSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                tb_FilePath.Text = openFileDialog.FileName;
                filePath = openFileDialog.FileName;
                btn_Process.IsEnabled = true;
                tb_FilePath.IsEnabled = false;
            }
        }

        private void btn_Process_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = File.ReadAllLines(filePath);
            short[] adcCodes = lines[0].Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(short.Parse)
                                       .ToArray();

            // Параметры АЦП
            const double adcRange = 1.0; // Амплитудный диапазон ±1 В
            const int adcBits = 14;      // Разрядность 14 бит
            double lsb = 2 * adcRange / (Math.Pow(2, adcBits) - 1); // Шаг квантования

            //Преобразование кодов АЦП в напряжение (в мВ)
            double[] voltages = adcCodes.Select(code => code * lsb * 1000).ToArray(); // В мВ

            if (SaveTxtOnly)
            {
                TxtFileSave(voltages);
            }

            else if (SaveBinOnly)
            {
                BinaryFileSave(voltages);
            }

            else
            {
                TxtFileSave(voltages);
                BinaryFileSave(voltages);
            }
            
            MessageBox.Show("Сигнал обработан", "Сигнал обработан", MessageBoxButton.OK, MessageBoxImage.Information);

            SinGraphDrawerWindow sinGraphDrawerWindow = new SinGraphDrawerWindow(adcCodes, voltages);
            sinGraphDrawerWindow.Show();

            WindowClear();  
        }

        private void radbtn_SaveTxtOnly_Checked(object sender, RoutedEventArgs e)
        {
            SaveTxtOnly = true;
            SaveBinOnly = false;
        }

        private void radbtn_SaveBinaryOnly_Checked(object sender, RoutedEventArgs e)
        {
            SaveBinOnly = true;
            SaveTxtOnly = false;
        }

        private void TxtFileSave(double[] voltages)
        {
            //Сохранение в текстовый файл
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.Title = "Сохранить текстовый файл";
            if (saveFileDialog.ShowDialog() == true)
            {
                string outputTextPath = saveFileDialog.FileName;
                using (StreamWriter writer = new StreamWriter(outputTextPath))
                {
                    foreach (var voltage in voltages)
                    {
                        writer.WriteLine(voltage.ToString("F3")); // Запись с 3 знаками после запятой
                    }
                }
            }
        }

        private void BinaryFileSave(double[] voltages)
        {
            //Сохранение в бинарный файл
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            saveFileDialog.Title = "Сохранить бинарный файл";
            if (saveFileDialog.ShowDialog() == true)
            {
                string outputBinaryPath = saveFileDialog.FileName;
                using (BinaryWriter writer = new BinaryWriter(File.Open(outputBinaryPath, FileMode.Create)))
                {
                    foreach (var voltage in voltages)
                    {
                        writer.Write(BitConverter.GetBytes(voltage));
                    }
                }
            }
        }

        private void WindowClear()
        {
            tb_FilePath.Text = null;
            tb_FilePath.IsEnabled = true;
            filePath = null;
            btn_Process.IsEnabled = false;

            // Сброс выбора RadioButton
            radbtn_SaveTxtOnly.IsChecked = false;
            radbtn_SaveBinaryOnly.IsChecked = false;
        }
    }
}