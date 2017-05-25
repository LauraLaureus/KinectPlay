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

        public MainWindow()
        {
            InitializeComponent();          
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
                      

            using (KStudioClient client = KStudio.CreateClient()) {

                client.ConnectToService();

                for (int f = 0; f < fileNames.Length; f++) {


                    string[] nameAndLoop = fileNames[f].Split(',');
                    KStudioPlayback playback = client.CreatePlayback(nameAndLoop[0]);
                    if (nameAndLoop.Length > 1) {
                        playback.LoopCount = (uint)Double.Parse(nameAndLoop[1]);
                    }

                    this.statusBox.Text = "Playing" + nameAndLoop[0];

                    playback.EndBehavior = KStudioPlaybackEndBehavior.Stop;
                    playback.Start();

                    while (playback.State == KStudioPlaybackState.Playing)
                    {
                        Thread.Sleep(500);
                    }
                    Console.WriteLine("End" + fileNames[f]);
            
                }
               
                
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
            Application.Current.Shutdown();
        }
    }
}
