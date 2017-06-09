using Microsoft.Kinect.Tools;
using System;
using System.Collections.Generic;
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

namespace KinectStudio
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] fileNames = null;
        KStudioPlayback playback;
        Thread playThread;

        public MainWindow()
        {
            
            InitializeComponent();          
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {

            playThread = new Thread(new ThreadStart(Play));
            playThread.Start();
            
        }

        private void UpdateTextBox(string text) {
            this.statusBox.Text = text;
        }

        private void Play() {

            using (KStudioClient client = KStudio.CreateClient())
            {


                client.ConnectToService();


                for (int f = 0; f < fileNames.Length; f++)
                {

                    string[] nameAndLoop = fileNames[f].Split(',');
                    Dispatcher.InvokeAsync(new Action(() => this.statusBox.Text = "Playing" + nameAndLoop[0]));

                    double loopCount = 0, timeSleept = 0;

                    try
                    {
                        playback = client.CreatePlayback(nameAndLoop[0]);
                        if (nameAndLoop.Length > 1)
                        {
                            loopCount = Double.Parse(nameAndLoop[1]);
                        }
                        else
                            loopCount = 1;

                        lock (playback)
                        {
                            playback.LoopCount = (uint)loopCount;
                            playback.EndBehavior = KStudioPlaybackEndBehavior.Stop;
                            playback.Start();
                        }

                            while (playback != null && playback.State == KStudioPlaybackState.Playing)
                            {
                                Thread.Sleep(100);
                                timeSleept += 100;
                            }
                            if ( playback != null && timeSleept < playback.Duration.TotalMilliseconds)
                                MessageBox.Show("Due to an unknown failure the playback has stopped.");
                        

                    }
                    catch (ThreadAbortException t) {
                        Console.WriteLine(t.ToString());
                    }

                }


            }

        }

        private void stateChagedHandler(object sender, EventArgs e)
        {
            if (playback.State != KStudioPlaybackState.Playing) {
                Console.Write(playback.State.ToString());
            }
        }

        private void loadClick(object sender, RoutedEventArgs e)
        {
            try
            {
                StreamReader reader = new StreamReader(@"./files.txt", Encoding.UTF8);
                string file = reader.ReadToEnd();
                fileNames = file.Split('\n');
                Console.WriteLine("Load Complete");
                fileBox.Text = file;
                this.statusBox.Text = "Playlist loaded. Press play to start";
            }
            catch (FileNotFoundException f) {
                this.statusBox.Text = "File Not Found. Make Sure its under this application folder and its named files.txt";
            }
            
        }

        private void OnWindowsClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (playThread != null) {
                playThread.Abort();
                playThread = null;
            }
            
            Application.Current.Shutdown();
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (playback != null) {

                playback.Stop();
                playback = null;
            }

            if (playThread != null) {
                playThread.Abort();
                playThread = null;
            }
        }
    }
}
