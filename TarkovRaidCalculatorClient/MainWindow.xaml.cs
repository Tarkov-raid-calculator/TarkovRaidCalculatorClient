using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

namespace TarkovRaidCalculatorClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly INet _net;
        public string Email { get; set; }
        public Auth Auth { get; set; }

        public MainWindow()
        {
            _net = new NetHttps();
            InitializeComponent();
        }
        public MainWindow(Window1 window1)
        {
            window1.Close();
            _net = new NetHttps();
            InitializeComponent();
        }
        //email
        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                Email = textBox.Text;
            }
        }
  
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder builders = new StringBuilder();
            string hashStr;
            using (SHA512 shaM = new SHA512Managed())
            {
                byte[] hash = shaM.ComputeHash(Encoding.UTF8.GetBytes(passowrdField.Password + Email));
                for (int i = 0; i < hash.Length; i++)
                {
                    builders.Append(hash[i].ToString("x2"));
                }
                hashStr = builders.ToString();
            }
            Auth = await _net.AuthAsync(Email, hashStr);
            
            if (Auth == null)
            {
                WrongLoginLabel.Visibility = Visibility.Visible;
            }
            else
            {
                Window1 form2 = new Window1(Auth,this);
                form2.ShowDialog();
            }
        }
    }
}
