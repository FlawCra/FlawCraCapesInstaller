using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shell;
//LOOK NO MORE 32894729739287298174908 USING STATEMENTS! WOW!

namespace Cloaks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public static readonly string version = "1.0";
        public static readonly string versionLink = "https://capes.flawcra.cc/installer/vers";
        public static string installerDownload = new WebClient()
        { Proxy = ((IWebProxy)null) }.DownloadString("https://capes.flawcra.cc/installer/download");
        


        Storyboard StoryBoard = new Storyboard();
        TimeSpan duration = TimeSpan.FromMilliseconds(500);
        TimeSpan duration2 = TimeSpan.FromMilliseconds(1000);
        
        private IEasingFunction Smooth
        {
            get;
            set;
        }
        = new QuarticEase
        {
            EasingMode = EasingMode.EaseInOut
        };

        public void Fade(DependencyObject Object)
        {
            DoubleAnimation FadeIn = new DoubleAnimation()
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(duration),
            };
            Storyboard.SetTarget(FadeIn, Object);
            Storyboard.SetTargetProperty(FadeIn, new PropertyPath("Opacity", 1));
            StoryBoard.Children.Add(FadeIn);
            StoryBoard.Begin();
        }

        public void FadeOut(DependencyObject Object)
        {
            DoubleAnimation Fade = new DoubleAnimation()
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(duration),
            };
            Storyboard.SetTarget(Fade, Object);
            Storyboard.SetTargetProperty(Fade, new PropertyPath("Opacity", 1));
            StoryBoard.Children.Add(Fade);
            StoryBoard.Begin();
        }

        public void ObjectShift(DependencyObject Object, Thickness Get, Thickness Set)
        {
            ThicknessAnimation Animation = new ThicknessAnimation()
            {
                From = Get,
                To = Set,
                Duration = duration2,
                EasingFunction = Smooth,
            };
            Storyboard.SetTarget(Animation, Object);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(MarginProperty));
            StoryBoard.Children.Add(Animation);
            StoryBoard.Begin();
        }

        private bool IsRunAsAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            return wp.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public MainWindow()
        {
            if(!IsRunAsAdministrator())
            {
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                // The following properties run the new process as administrator
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";

                // Start the new process
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception)
                {
                    // The user did not allow the application to run as administrator
                    MessageBox.Show("Sorry, this application must be run as Administrator.", "FC Capes", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Shut down the current process
                Application.Current.Shutdown();
            }
            try
            {
                string getVersion = new WebClient().DownloadString(versionLink);
                if (version != getVersion.Trim())
                {
                    int num = (int)MessageBox.Show("This version of FC Capes is outdated. Please press OK to update.", "FC Capes | Update avaliable", MessageBoxButton.OK, MessageBoxImage.Error);
                    string ok = Path.GetDirectoryName(Directory.GetCurrentDirectory());
                    if (System.IO.File.Exists(ok + "\\FlawCraCapes.exe"))
                        System.IO.File.Delete(ok + "\\FlawCraCapes.exe");
                    new WebClient() { Proxy = ((IWebProxy)null) }.DownloadFile(MainWindow.installerDownload, ok + "\\FlawCraCapes.exe");
                    ProcessStartInfo startInfo = new ProcessStartInfo(ok + "\\FlawCraCapes.exe");
                    startInfo.Verb = "runas";
                    System.Diagnostics.Process.Start(startInfo);
                    this.Close();
                    Environment.Exit(0);
                }
            }
            catch(Exception ohShitWhatNow)
            {
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Error;
                MessageBox.Show("FC Capes has encountered an error trying to update. Please send the error message below to the Discord server.\n\n" + ohShitWhatNow.Message + "\nError source: " + ohShitWhatNow.Source, "FC Capes Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
            }
            //i'm dumb as shit and there's a \n at the end
            InitializeComponent();
            this.Activate();
        }

        private void Cloaks_Loaded(object sender, RoutedEventArgs e)
        {
            Fade(MainBorder);
            Fade(TopBorder);
            Fade(SelectFrame);

            ObjectShift(MainBorder, MainBorder.Margin, new Thickness(0, 0, 0, 0));
            ObjectShift(TopBorder, TopBorder.Margin, new Thickness(-2, -2, -2, 0));
            ObjectShift(SelectFrame, TopBorder.Margin, new Thickness(28, 30.5, 0, 0));
            ObjectShift(HomeFrame, HomeFrame.Margin, new Thickness(124, 63, 19, 35));

        }

        private void TopBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //what is this drag code  h u h
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void HomeFrame_Click(object sender, RoutedEventArgs e)
        {
            HomeFrame.Opacity = 100;
            HomeFrame.Visibility = Visibility.Visible;
            InstallFrame.Opacity = 0;
            InstallFrame.Visibility = Visibility.Collapsed;
            CreditsFrame.Visibility = Visibility.Collapsed;
            CreditsFrame.Opacity = 0;
        }

        private void InstallFrame_Click(object sender, RoutedEventArgs e)
        {
            HomeFrame.Visibility = Visibility.Collapsed;
            HomeFrame.Opacity = 0;
            InstallFrame.Opacity = 100;
            InstallFrame.Visibility = Visibility.Visible;
            CreditsFrame.Visibility = Visibility.Collapsed;
            CreditsFrame.Opacity = 0;
        }

        private void CreditsFrame_Click(object sender, RoutedEventArgs e)
        {
            HomeFrame.Visibility = Visibility.Collapsed;
            HomeFrame.Opacity = 0;
            InstallFrame.Visibility = Visibility.Collapsed;
            InstallFrame.Opacity = 0;
            CreditsFrame.Visibility = Visibility.Visible;
            CreditsFrame.Opacity = 100;
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            if(chequeBox.IsChecked == true)
            {
                try
                {
                    string oldIP = File.ReadAllText("C:\\Windows\\System32\\drivers\\etc\\hosts");
                    if (oldIP.Contains("161.35.130.99 s.optifine.net"))
                    {
                        this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Indeterminate;
                        var hosts = "C:\\Windows\\System32\\drivers\\etc\\hosts";
                        File.WriteAllLines(hosts, File.ReadLines(hosts).Where(l => l != "161.35.130.99 s.optifine.net").ToList());
                        string aaaaaa = File.ReadAllText("C:\\Windows\\System32\\drivers\\etc\\hosts");
                        if (aaaaaa.Contains("178.18.243.41 s.optifine.net"))
                        {
                            this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Error;
                            MessageBox.Show("You already have FC Capes", "FC Capes", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                        else
                        {
                            using (StreamWriter bruh = File.AppendText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts")))
                            {
                                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Indeterminate;
                                bruh.WriteLine("\n178.18.243.41 s.optifine.net\n# THE LINE ABOVE WAS INSERTED BY FLAWCRA CAPES");
                                this.Activate();
                                MessageBox.Show("FC Capes successfully installed!", "FC Capes", MessageBoxButton.OK, MessageBoxImage.Information);
                                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
                            }
                        }
                    }
                    else
                    {
                        string contents = File.ReadAllText("C:\\Windows\\System32\\drivers\\etc\\hosts");
                        if (contents.Contains("178.18.243.41 s.optifine.net"))
                        {
                            this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Error;
                            MessageBox.Show("You already have FC Capes", "FC Capes", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                        else
                        {
                            using (StreamWriter hosts = File.AppendText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts")))
                            {
                                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Indeterminate;
                                hosts.WriteLine("\n178.18.243.41 s.optifine.net\n# THE LINE ABOVE WAS INSERTED BY FLAWCRA CAPES");
                                this.Activate();
                                MessageBox.Show("FC Capes successfully installed!", "FC Capes", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
                            }
                        }
                    }
                }
                catch (IOException shittyVariableName)
                {
                    this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Error;
                    MessageBox.Show("FC Capes has encountered an error. Please send the error message below to the Discord server.\n\n" + shittyVariableName.Message + "\nError source: " + shittyVariableName.Source, "FC Capes Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
                }
                catch (Exception bruvIdkHowToSpellExecption)
                {
                    this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Error;
                    MessageBox.Show("FC Capes has encountered an error. Please send the error message below to the Discord server.\n\n" + bruvIdkHowToSpellExecption.Message + "\nError source: " + bruvIdkHowToSpellExecption.Source, "FC Capes Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
                }
            }
            else
            {
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Error;
                MessageBox.Show("Please agree to the EULA!", "FC Capes", MessageBoxButton.OK, MessageBoxImage.Error);
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
            }
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string hmm = File.ReadAllText("C:\\Windows\\System32\\drivers\\etc\\hosts");
                if (hmm.Contains("178.18.243.41 s.optifine.net"))
                {
                    var removeOld = "C:\\Windows\\System32\\drivers\\etc\\hosts";
                    File.WriteAllLines(removeOld, File.ReadLines(removeOld).Where(l => l != "161.35.130.99 s.optifine.net").ToList());
                    string contents = File.ReadAllText("C:\\Windows\\System32\\drivers\\etc\\hosts");
                    if (contents.Contains("178.18.243.41 s.optifine.net"))
                    {
                        this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Indeterminate;
                        var hosts = "C:\\Windows\\System32\\drivers\\etc\\hosts";
                        File.WriteAllLines(hosts, File.ReadLines(hosts).Where(l => l != "178.18.243.41 s.optifine.net").ToList());
                        string secondCheckThingy = File.ReadAllText("C:\\Windows\\System32\\drivers\\etc\\hosts");
                        if (contents.Contains("# THE LINE ABOVE WAS INSERTED BY FLAWCRA CAPES"))
                        {
                            var removeComment = "C:\\Windows\\System32\\drivers\\etc\\hosts";
                            File.WriteAllLines(removeComment, File.ReadLines(removeComment).Where(l => l != "# THE LINE ABOVE WAS INSERTED BY FLAWCRA CAPES").ToList());
                            MessageBox.Show("FC Capes successfully uninstalled!", "FC Capes", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
                        }
                        else
                        {
                            MessageBox.Show("FC Capes successfully uninstalled!", "FC Capes", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
                        }

                    }
                }
                else
                {
                    MessageBox.Show("FC Capes not detected!", "FC Capes Uninstaller", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (IOException IOError)
            {
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Error;
                MessageBox.Show("FC Capes has encountered an error. Please send the error message below to the Discord server.\n\n" + IOError.Message + "\nError source: " + IOError.Source, "FC Capes Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
            }
            catch (Exception exeption) //idk how to spell lmao
            {
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Error;
                MessageBox.Show("FC Capes has encountered an error. Please send the error message below to the Discord server.\n\n" + exeption.Message + "\nError source: " + exeption.Source, "FC Capes Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
            }
        }

        private void EulaButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://cloaksplus.com/terms.txt");
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FadeOut(MainBorder);
            FadeOut(TopBorder);
            FadeOut(SelectFrame);
            //AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ObjectShift(MainBorder, MainBorder.Margin, new Thickness(49, 70, 49, 26));
            ObjectShift(TopBorder, TopBorder.Margin, new Thickness(0, -28, 0, 0));
            ObjectShift(SelectFrame, SelectFrame.Margin, new Thickness(-90, 79, 0, 0));
            ObjectShift(HomeFrame, HomeFrame.Margin, new Thickness(101, 0, 199, 230));
            await Task.Delay(1000);
            Application.Current.Shutdown();
            await Task.Delay(1000);
            System.Windows.Forms.Application.Exit();
            await Task.Delay(1000); //tried usin thread.sleep but it just didn't work lmao 
            System.Windows.Forms.Application.ExitThread(); //zuhn the fuck is this code LMAO
            Environment.Exit(0);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
