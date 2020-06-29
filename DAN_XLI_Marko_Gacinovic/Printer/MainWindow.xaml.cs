using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Printer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // properties for textboxes
        string copies { get; set; }
        string text { get; set; }

        static BackgroundWorker bw = new BackgroundWorker();

        // constructor with method events
        public MainWindow()
        {
            InitializeComponent();
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// method for calculation for the progress bar and creating files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            // validation for copies input
            bool isValid = int.TryParse(copies, out int numOfCopies);

            // variables for calculating progress bar percentage
            int sum = 0;
            int percentage;

            // loop for calculating progres bar percentage and printing files
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

                // name for the files
                string path = "../../" + i + "." + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".txt";

                // creating files
                File.WriteAllText(path, text);
            }

            e.Result = sum;
        }

        /// <summary>
        /// method for displaying progress bar work
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
            Percentage.Content = e.ProgressPercentage.ToString() + "%";
        }

        /// <summary>
        /// method for displaying result message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// method for getting text from the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            text = Text.Text;
        }

        /// <summary>
        /// method for getting copies from the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            copies = Copies.Text;
        }

        /// <summary>
        /// method for Print button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!bw.IsBusy)            {
                
                bw.RunWorkerAsync();
            }
            else
            {
                Message.Content = "Printer is busy, please wait.";
            }
        }

        /// <summary>
        /// method for Cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (bw.IsBusy)
            {                
                bw.CancelAsync();
            }
        }
    }
}
