using System;
using System.Collections.Generic;
using System.Linq;
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

using System.Threading;
using System.Diagnostics;

namespace ReactionTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int _TESTS_COUNT = 5;
        int testsDone = 0;

        Random rnd = new Random();
        Stopwatch stopWatch;

        //int bestResult = int.MaxValue;
        //int worstResult = int.MinValue;
        List<long> scores = new List<long>();
        int falses = 0;

        Thread testingThread;

        bool isColorChanged = false;
        bool isTestingStarted = false;

        public MainWindow()
        {
            InitializeComponent();
                        
            testingThread = new Thread(new ThreadStart(DoTest));
            testingThread.IsBackground = true;

            SetUpLabel();
            //Test();
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            resultsLabel.Content = "";

            testsDone = 0;
            scores.Clear();
            falses = 0;

            isTestingStarted = true;

            Start_Button.Visibility = Visibility.Hidden;

            //testingThread.Start();
            CheckIfTestingEnded();
        }

        void DoTest()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                grid.Background = Brushes.White;
            }));

            isColorChanged = false;

            var timeBeforeColorChange = rnd.Next(800, 4000);

            Thread.Sleep(timeBeforeColorChange);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                grid.Background = Brushes.Red;
            }));

            isColorChanged = true;

            stopWatch = Stopwatch.StartNew();
        }

        void Test()
        {
            Label resultsLabel = new Label();

            var resultsText = "";

            //if (falses != 0)
            resultsText += String.Format("Ложных нажатий: {0}", 123) + Environment.NewLine;

            resultsText += String.Format("Среднее время: {0} мс", 333) + Environment.NewLine;
            resultsText += String.Format("Минимальное время: {0} мс", 12) + Environment.NewLine;
            resultsText += String.Format("Максимальное время: {0} мс", 123) + Environment.NewLine;

            resultsLabel.Height = 114;
            resultsLabel.FontSize = 20;
            resultsLabel.Content = resultsText;
            resultsLabel.HorizontalAlignment = HorizontalAlignment.Center;
            resultsLabel.Margin = new Thickness(0, -200, 0, 0);

            Start_Button.Visibility = Visibility.Visible;

            grid.Children.Add(resultsLabel);
        }

        Label resultsLabel = new Label();
        void SetUpLabel()
        {
            resultsLabel.Height = 114;
            resultsLabel.FontSize = 20;
            //resultsLabel.Content = "123";
            resultsLabel.HorizontalAlignment = HorizontalAlignment.Center;
            resultsLabel.Margin = new Thickness(0, -200, 0, 0);

            grid.Children.Add(resultsLabel);
        }


        
        void CheckIfTestingEnded()
        {           
            if (_TESTS_COUNT == testsDone)
            {               
                grid.Background = Brushes.White;

                //resultsLabel = new Label();

                var resultsText = "";

                if (falses != 0)
                    resultsText += String.Format("Ложных нажатий: {0}", falses) + Environment.NewLine;

                if(scores.Count != 0)
                {
                    resultsText += String.Format("Среднее время: {0} мс", (int)scores.Average()) + Environment.NewLine;
                    resultsText += String.Format("Минимальное время: {0} мс", scores.Min()) + Environment.NewLine;
                    resultsText += String.Format("Максимальное время: {0} мс", scores.Max()) + Environment.NewLine;
                }


                //resultsLabel.Height = 114;
                //resultsLabel.FontSize = 20;
                resultsLabel.Content = resultsText;
                //resultsLabel.HorizontalAlignment = HorizontalAlignment.Center;
                //resultsLabel.Margin = new Thickness(0, -200, 0, 0);

                Start_Button.Visibility = Visibility.Visible;

                //grid.Children.Add(resultsLabel);

                isColorChanged = false;
                isTestingStarted = false;
            }
            else
            {
                //testingThread.Resume();
                

                testingThread = new Thread(new ThreadStart(DoTest));
                testingThread.IsBackground = true;
                testingThread.Start();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Space)
                return;

            if (!isTestingStarted)
                return;

            testingThread.Abort();

            if (!isColorChanged)
            {
                falses++;
                testsDone++;

                CheckIfTestingEnded();
            }
            else
            {
                scores.Add(stopWatch.ElapsedMilliseconds);

                testsDone++;

                CheckIfTestingEnded();
            }    
        }
    }
}
