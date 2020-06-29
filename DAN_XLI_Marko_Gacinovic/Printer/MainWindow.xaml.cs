using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Printer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string copies { get; set; }
        string text { get; set; }

        static BackgroundWorker bw = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            bool isValid = int.TryParse(copies, out int numOfCopies);
            int sum = 0;
            int percentage;

            for (int i = 1; i <= numOfCopies; i++)
            {
                Thread.Sleep(1000);
                percentage = 100 / numOfCopies;
                sum = sum + percentage;
                bw.ReportProgress(sum);

                if (bw.CancellationPending)
                {
                    // Set Cancel property of DoWorkEventArgs object to true
                    e.Cancel = true;
                    // Reset progress percentage to ZERO and return
                    bw.ReportProgress(0);
                    return;
                }

                string path = "../../" + i + "." + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".txt";

                File.WriteAllText(path, text);
            }

            e.Result = sum;
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
            Percentage.Content = e.ProgressPercentage.ToString() + "%";
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Message.Content = "Processing cancelled";
            }
            else if (e.Error != null)
            {
                Message.Content = e.Error.Message;
            }
            else
            {
                Message.Content = e.Result.ToString();
            }
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            text = Text.Text;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            copies = Copies.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!bw.IsBusy)
            {
                // This method will start the execution asynchronously in the background
                bw.RunWorkerAsync();
            }
            else
            {
                Message.Content = "Printer is busy, please wait.";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (bw.IsBusy)
            {
                // Cancel the asynchronous operation if still in progress
                bw.CancelAsync();
            }
        }
    }
}
