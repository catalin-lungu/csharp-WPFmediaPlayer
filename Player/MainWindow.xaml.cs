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
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;
using System.Data;
using Microsoft.Win32;
using System.IO;

namespace Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //bool wasCollapsed = false;
        bool isPlaying = false;
        DispatcherTimer timer;
        DispatcherTimer timer1;
        public delegate void timerTick();
        public delegate void timerTick1();
        timerTick tick;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);
            tick = new timerTick(changeStatus);

            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromSeconds(1);
            timer1.Tick += new EventHandler(timer_Tick1);

            sliderBar1.AddHandler(Slider.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(sliderBar1_MouseLeftButtonDown), true); 

        }

        void timer_Tick(object sender, EventArgs e)
        { Dispatcher.Invoke(tick); }

        string sec, min;
        void changeStatus()
        {
            if (isPlaying)
            {
                sec = mediaElement1.Position.Seconds.ToString();
                min = mediaElement1.Position.Minutes.ToString();

                sliderBar1.Value = mediaElement1.Position.TotalMilliseconds;
                if (sec.Length < 2) sec = "0" + sec;
                textBlock2.Text = min + ":" + sec;
            }
        }

        double valueProgresBar = 0;
        void timer_Tick1(object sender, EventArgs e)
        {
            valueProgresBar++;
            sliderBar1.Value = valueProgresBar;
            textBlock1.Text = sliderBar1.Value.ToString();
            if (sliderBar1.Value >= sliderBar1.Maximum)
            {
                timer1.Stop(); 
            }
        }
        
        //
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        private int getSeconds(string time)
        {
            int min = Convert.ToInt32(time.Substring(0, time.IndexOf(':')));
            return min * 60;
        }
        //play
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Melodie m = (Melodie)listView1.SelectedItem;
            if (m == null && listView1.Items.Count > 0)
            {
                m = (Melodie)listView1.Items[0];
            }
            if (m != null)
            {
                isPlaying = true;
                PauseButton.IsEnabled = true;
                StopButton.IsEnabled = true;
                NextButton.IsEnabled = true;
                PrevButton.IsEnabled = true;

                if (m.URI != null)
                {
                    try
                    {
                        mediaElement1.Source = m.URI;
                        mediaElement1.Play();
                        timer.Start();
                        titluMelodie.Text = m.NumeMelodie;
                        if (m.Coperta != null)
                        {
                            if (m.Coperta == "No image to display")
                            {
                                Image i = new Image();
                                BitmapImage src = new BitmapImage();
                                src.BeginInit();
                                src.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"/img/images.jpg");
                                src.CacheOption = BitmapCacheOption.OnLoad;
                                src.EndInit();
                                i.Source = src;
                                i.Stretch = Stretch.Uniform;
                            }
                            else
                            {
                                Image i = new Image();
                                BitmapImage src = new BitmapImage();
                                src.BeginInit();
                                src.UriSource = new Uri(m.Coperta);
                                src.CacheOption = BitmapCacheOption.OnLoad;
                                src.EndInit();
                                i.Source = src;
                                i.Stretch = Stretch.Uniform;
                            }
                        }
                        else
                        {
                            Image i = new Image();
                            BitmapImage src = new BitmapImage();
                            src.BeginInit();
                            src.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory +@"/img/images.jpg");
                            src.CacheOption = BitmapCacheOption.OnLoad;
                            src.EndInit();
                            i.Source = src;
                            i.Stretch = Stretch.Uniform;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Playing error" + ex.StackTrace);
                    }
                }
            }
            else
            {
                MessageBox.Show("No song in list!");
            }
        }
        public void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            timer.Start();
            isPlaying = true;
            openMedia();
        }
        public void openMedia()
        {
            mediaElement1.Volume = (double)slider1.Value;
            try
            {
                if (mediaElement1.NaturalDuration.TimeSpan.Seconds < 10)
                    sec = "0" + mediaElement1.Position.Seconds.ToString();
                else
                    sec = mediaElement1.NaturalDuration.TimeSpan.Seconds.ToString();

                if (mediaElement1.NaturalDuration.TimeSpan.Minutes < 10)
                    min = "0" + mediaElement1.NaturalDuration.TimeSpan.Minutes.ToString();
                else
                    min = mediaElement1.NaturalDuration.TimeSpan.Minutes.ToString();
                
                textBlock1.Text = min + ":" + sec;
                
            }
            catch {
                MessageBox.Show("Error!!!");
            }
            string path = mediaElement1.Source.LocalPath.ToString();

            double duration = mediaElement1.NaturalDuration.TimeSpan.TotalMilliseconds;
            sliderBar1.Maximum = duration;

            mediaElement1.Volume = slider1.Value;
            mediaElement1.ScrubbingEnabled = true;

            slider1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(volumeSliderValueChanged);
            
        }
        public void mediaElement_MediaEnd(object sender, RoutedEventArgs e)
        {
           // mediaElement1.Stop();
            mediaElement1.Position = TimeSpan.Zero;
            if (checkBox1.IsChecked == true)
                mediaElement1.Play();
            else if (checkBox2.IsChecked == true)
                Play_Next_Click(null, null);
            else
            {
                Stop_Click(null, null);
            }
        }
      
        //pause
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            isPlaying = false;
            mediaElement1.Pause();
            timer.Stop();
            timer1.Stop();
        }
      

        private void openFileButton(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "All Supported File Types(*.mp3,*.wav,*.mpeg,*.wmv,*.avi)|*.mp3;*.wav;*.mpeg;*.wmv;*.avi";
            // Show open file dialog box 
            if ((bool)dlg.ShowDialog())
            {
                try
                {
                    foreach (String file in dlg.FileNames)
                    {
                        if (File.Exists(file))
                        {
                            Melodie m = new Melodie();
                            m.URI = new Uri(file);
                            m.NumeMelodie = (file.Substring(dlg.FileName.LastIndexOf('\\') + 1));
                            m.NumeMelodie = m.NumeMelodie.Substring(0, m.NumeMelodie.LastIndexOf('.'));
                            listView1.Items.Add(m);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            isPlaying = false;
            timer.Stop();
            timer1.Stop(); valueProgresBar = 0;
            mediaElement1.Stop();
            sliderBar1.Value = 0;
            textBlock1.Text = "00:00";
            textBlock2.Text = "00:00";
            titluMelodie.Text = "";
            PauseButton.IsEnabled = false;
            StopButton.IsEnabled = false;
            NextButton.IsEnabled = false;
            PrevButton.IsEnabled = false;
        }
        private void Play_Next_Click(object sender, RoutedEventArgs e)
        {
            if (listView1.HasItems)
            {
                if (listView1.SelectedIndex < listView1.Items.Count-1)
                    listView1.SelectedIndex += 1;
                else
                    listView1.SelectedIndex = 0;
                Button_Click_1(null,null);
            }
        }
        private void Play_Prev_Click(object sender, RoutedEventArgs e)
        {
            if (listView1.HasItems)
            {
                if (listView1.SelectedIndex == 0)
                    listView1.SelectedIndex = listView1.Items.Count - 1;
                else
                    listView1.SelectedIndex -= 1;
                Button_Click_1(null, null);
            }
        }

        //remove 1
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (listView1.HasItems)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    Melodie aux = (Melodie)listView1.SelectedItem;
                    if (aux.NumeMelodie == titluMelodie.Text)
                        Stop_Click(null, null);
                    listView1.Items.Remove(listView1.SelectedItem);
                }
                else
                    MessageBox.Show("no item selected to be deleted");
            }
            else
            {
                MessageBox.Show("list is empty");
            }
        }
        //remove all
        private void RemoveAll_Click(object sender, RoutedEventArgs e)
        { 
            if (listView1.HasItems)
            {
                Stop_Click(null, null);
                while (!listView1.Items.IsEmpty)
                {
                    listView1.Items.RemoveAt(0);
                }
            }
            else
            {
                MessageBox.Show("List is empty!");
            }
        }

        private void Check1(object sender, RoutedEventArgs e)
        {
            if (checkBox2.IsChecked==true) checkBox2.IsChecked = false;
        }
        private void Check2(object sender, RoutedEventArgs e)
        {
            if (checkBox1.IsChecked == true) checkBox1.IsChecked = false;
        }

        private void slider1_MouseMove(object sender, MouseEventArgs e)
        {
            double currentWidth = slider1.ActualWidth;
            Point point = e.GetPosition(slider1);

        }

        private void slider1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double currentWidth = slider1.ActualWidth;
            if (e.Delta > 0)
            {
                if (slider1.Value < 100)
                {
                    slider1.Value += 5;
                }
            }
            else
            {
                if(slider1.Value > 0)
                {
                    slider1.Value -= 5;
                }
            }
        }

        #region volume
        // vol 
        private void Button_VolumePlus(object sender, RoutedEventArgs e)
        {
           
                if (slider1.Value < 100)
                {
                    slider1.Value += 5;
                }
        }

        // vol -
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (slider1.Value > 0)
            {
                slider1.Value -= 5;
            }
        }

          //volum
        private void volumeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaElement1 != null)
            {
                mediaElement1.Volume = slider1.Value/100;
            }
        }


        # endregion

        // slider progress
        private void sliderBar1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isPlaying)
            {
                if (sliderBar1.Value > 0)
                {
                    Point p = e.GetPosition((IInputElement)sender);
                    double val = p.X * 100 / ((Slider)sender).ActualWidth;
                    long newPoz = (long)((double)mediaElement1.NaturalDuration.TimeSpan.Ticks * val / 100);

                    mediaElement1.Position = TimeSpan.FromTicks(newPoz) <= mediaElement1.NaturalDuration.TimeSpan ? TimeSpan.FromTicks(newPoz) : mediaElement1.Position;
                }
            }
        }
    }
}
