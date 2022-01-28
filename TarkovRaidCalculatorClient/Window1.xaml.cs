using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TarkovRaidCalculatorClient
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private HubConnection Connection { get; set; }
        private string ConnId { get; set; }
        private string Url { get; set; }
        private Auth Auth { get; set; }
        private string Raid { get; set; }
        private Key LitsenKey { get; set; }
        private bool Active { get; set; }
        private DirectoryInfo ScreenShotDirectory { get; set; }

        private LowLevelKeyboardListener _listener;
        private readonly INet _net;
        public Window1(Auth auth, MainWindow mainWindow)
        {
            mainWindow.Close();
            Active = true;
            LitsenKey = Key.PrintScreen;
            Raid = "pre";
            //label1.Text = GetRaidText();
            Auth = auth;
            Url = "http://192.168.2.37:9000/screenshotHub";
            _net = new NetHttps();
            ScreenShotDirectory = new DirectoryInfo("C:\\Users\\anthe\\Documents\\Escape from Tarkov\\Screenshots");
            Connection = GetConnection(Url);
            Connection.On<string>("ReceiveConId", (conId) =>
            {

            });
            
            _listener = new LowLevelKeyboardListener();
            _listener.OnKeyPressed += _listener_OnKeyPressed;

            _listener.HookKeyboard();
            InitializeComponent();
            Task.Run(() => {
                Connection.StartAsync();
            });
            PathLabel.Content = ScreenShotDirectory.FullName;
            KeyLabel.Content = keyToString(LitsenKey);

        }
        public string keyToString(Key key) {
            switch (key)
            {
                case Key.PrintScreen:
                    return "PRINT SCREEN";
                default:
                    return key.ToString();
            }
        }
        void _listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            if (e.KeyPressed == LitsenKey && !Active) 
            {
                Task.Run(() => {
                    Task.Delay(1000);
                    PrintScreenPress();
                });
            }
        }
        private string GetRaidText()
        {
            switch (Raid)
            {
                case "pre":
                    return "Before";
                case "post":
                    return "After";
                default:
                    return "";
            }
        }
        private void SetPreraid() => Raid = "Pre";
        private void SetPostraid() => Raid = "Post";
        private void PrintScreenPress()
        {
            FileInfo imageFilePath = GetLastImage();
            if (imageFilePath.Exists) {
                byte[] body = ReadImage(imageFilePath.FullName);
                _net.SendImage(body, Auth.Token, imageFilePath.Name, Raid);
            }
        }
        private FileInfo GetLastImage() => ScreenShotDirectory.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
        private HubConnection GetConnection(string url) => new HubConnectionBuilder().WithUrl(url).WithAutomaticReconnect().Build();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(this);
            mainWindow.ShowDialog();
        }
        //after
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            SetPostraid();
        }
        //before
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SetPreraid();
            
        }

        private void ActiveCB_Checked(object sender, RoutedEventArgs e)
        {
            Active = !Active;
        }
        private byte[] ReadImage(string path) => System.IO.File.ReadAllBytes(path);
    }
}
