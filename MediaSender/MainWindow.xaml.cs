using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MediaSender.Helpers;
using MediaSender.Properties;
using MediaSender.Senders;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;


namespace MediaSender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<string> FilesList;
        private ListViewItem[] filesListViewItems;
        private event RoutedEventHandler LScanningStart;
        private List<string> attachments = new List<string>();
        private List<string> showedFileList = new List<string>();
        private CancellationTokenSource ScanPause; 
 
        public static bool IsChannelSet { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
            CheckFolder();
            SearchFilesAsync();
            ScrollViewer.MaxHeight = SystemParameters.PrimaryScreenHeight - 100;
            this.WindowStartupLocation = Settings.Default.WindowPosition;
            

        }

        private void CheckFolder()
        {
            if (!Directory.Exists(Settings.Default.DefaultFolder))
            {
                Settings.Default.DefaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) ;
            }
        }

        private async void ShowScan()
        {
            Scanning.IsEnabled = true;
            await Task.Delay(3000 );
            Scanning.IsEnabled = false;
        }


        private async void SearchFilesAsync()
        {
            MediaPanel.Children.Clear();
            int LLastFiles = 0;
            List<string> extensions = new List<string>(Settings.Default.FileFormats.Split(","));
            SearchOption scansub = SearchOption.TopDirectoryOnly;

            ScanPause = new CancellationTokenSource();
            

            while (true)
            {
                ShowScan();

                if (Settings.Default.SubfoldersScan == true)
                {
                    scansub = SearchOption.AllDirectories;
                }
                else
                {
                    scansub = SearchOption.TopDirectoryOnly;
                }


                string[] LAllFiles = Directory.GetFiles(Settings.Default.DefaultFolder, "*", scansub);


                

                if (LAllFiles.Length != LLastFiles)
                {

                    foreach (string ext in extensions)
                    {

                        foreach (string Lfile in LAllFiles)
                        {
                            if (Lfile.Contains(ext))
                            {
                                ShellFile ShFile = ShellFile.FromFilePath(Lfile);
                                Bitmap LBitmap = ShFile.Thumbnail.Bitmap;
                                

                                MyMediaElement LMediaElement = new MyMediaElement()
                                {
                                    Width = 80,
                                    Height = 50,
                                    Source = Imaging.CreateBitmapSourceFromHBitmap(LBitmap.GetHbitmap(),
                                    IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(80, 50)),
                                    Margin = new Thickness(2, 2, 2, 2),
                                    Stretch = Stretch.UniformToFill,
                                    SourceUri = new Uri(Lfile),
                                    IsSelected = false,
                                    ToolTip = "Right click to preview"
                                };

                                LMediaElement.MouseLeftButtonDown += Media_MouseLeft;
                                LMediaElement.MouseRightButtonDown += Media_MouseRight;

                                Border LelementBorder = new Border()
                                {
                                    BorderBrush = new SolidColorBrush(Colors.Black),
                                    Margin = new Thickness(2, 2, 2, 2),
                                    BorderThickness = new Thickness(1, 1, 1, 1),
                                    Child = LMediaElement
                                };

                                if (!showedFileList.Contains(Lfile))
                                {
                                    MediaPanel.Children.Add(LelementBorder);
                                    showedFileList.Add(Lfile);
                                }



                            }

                        }
                    }
                }




                Initialized.Text = "New files: " + (LAllFiles.Length - LLastFiles).ToString();
                LLastFiles = LAllFiles.Length;

                try
                {
                    await Task.Delay(60_000 * Settings.Default.ScanPeriodMin, ScanPause.Token);
                   
                }
                catch (Exception e)
                {
                    return;
                }

               
                


            }
        }

        private void Media_MouseRight(object sender, MouseEventArgs e)
        {
            MyMediaElement LMediaElement = (MyMediaElement)sender;
            Preview LPrevWindow = new Preview(LMediaElement.SourceUri);
            LPrevWindow.Show();

        }

        private void Media_MouseLeft(object sender, MouseButtonEventArgs e)
        {

            MyMediaElement LMediaElement = (MyMediaElement)sender;

            if (LMediaElement.IsSelected == false)
            {
                LMediaElement.Effect = new BlurEffect() { Radius = 10 };
                LMediaElement.IsSelected = true;
                LMediaElement.OpacityMask = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/checked.png")));
            }
            else
            {
                LMediaElement.Effect = null;
                LMediaElement.IsSelected = false;
                LMediaElement.OpacityMask = null;
            }

        }

        private void ButtonSend_OnClick(object sender, RoutedEventArgs e)
        {
            AttachFiles(this.MediaPanel.Children);

            if (ToEMail.IsChecked == true)
            {
                MailSender eMailSender = new MailSender();
                eMailSender.MailSend(attachments);
            }

            if (ToTelegram.IsChecked == true)
            {
                TelegramSender telegramSender = new TelegramSender();
                telegramSender.TelegramSend(attachments);
            }



        }


        private void OnPicturesLoaded(object? sender, EventArgs e)
        {

            Loading.Visibility = Visibility.Collapsed;
        }


        private void MainWindow_OnContentRendered(object? sender, EventArgs e)
        {

            MediaPanel.SizeChanged += OnPicturesLoaded;
            TextBoxEmailPassw.Password = "Enter password";
            TextBoxEmailPassw.LostFocus += TextBoxEmailPassw_OnPasswordChanged;
            BoxesValidation.IsChecked = true;
        }


        private void MainWindow_OnClosed(object? sender, EventArgs e)
        {

            Settings.Default.WindowPosition = this.WindowStartupLocation;
            Settings.Default.Save();
        }

        private void SettingsBtn_OnClick(object sender, RoutedEventArgs e)
        {

            Storyboard sb = Application.Current.Resources["ShowSettingsPanel"] as Storyboard;
            sb?.Begin(AppSettings);
        }

        private void SettingsSaveBtn_OnClick(object sender, RoutedEventArgs e)
        {

            Storyboard sb = Application.Current.Resources["HideSettingsPanel"] as Storyboard;
            sb?.Begin(AppSettings);
            Settings.Default.Save();

        }

        private void TextBoxEmailPassw_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Settings.Default.Password = TextBoxEmailPassw.Password;
            //new NetworkCredential("", TextBoxEmailPassw.Password).SecurePassword;

        }

        private void ShowPasswordCharsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TextBoxShowPassw.Text = TextBoxEmailPassw.Password;
            TextBoxEmailPassw.Visibility = System.Windows.Visibility.Collapsed;
            TextBoxShowPassw.Visibility = System.Windows.Visibility.Visible;

            TextBoxShowPassw.Focus();
        }

        private void ShowPasswordCharsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TextBoxEmailPassw.Password = TextBoxShowPassw.Text;
            TextBoxEmailPassw.Visibility = System.Windows.Visibility.Visible;
            TextBoxShowPassw.Visibility = System.Windows.Visibility.Collapsed;

            TextBoxEmailPassw.Focus();


        }

        public void AttachFiles(UIElementCollection files)
        {
            attachments.Clear();

            foreach (Border file in files)
            {

                MyMediaElement InnerElement = file.Child as MyMediaElement;
                if (InnerElement?.IsSelected == true)
                    attachments.Add(InnerElement.SourceUri.AbsolutePath);
            }
        }


        private void ChannelSet(object sender, RoutedEventArgs e)
        {
            if (ToEMail.IsChecked == true || ToTelegram.IsChecked == true || ToInstagram.IsChecked == true)
            {
                BoxesValidation.IsChecked = false;
            }
            else
            {
                BoxesValidation.IsChecked = true;

            }
        }

        private void SSL_OnClick(object sender, RoutedEventArgs e)
        {
            if (SSL.IsChecked == true)
            {
                Settings.Default.SmtpPort = 465;
            }
            else
            {
                Settings.Default.SmtpPort = 25;
            }


        }

        private void ButtonFolder_OnClick(object sender, RoutedEventArgs e)
        {

            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                Settings.Default.DefaultFolder = dialog.SelectedPath;
                showedFileList.Clear();
                ScanPause.Cancel();
                SearchFilesAsync();
            }
        }

        private void RescanBtn_OnClick(object sender, RoutedEventArgs e)
        {
            showedFileList.Clear();
            ScanPause.Cancel();
            SearchFilesAsync();
        }
    }
}
