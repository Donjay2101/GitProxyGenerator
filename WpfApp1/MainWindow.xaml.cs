using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Core;
using WpfApp1.Models;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var request = new ProxyRequest
                {
                    Password = Password.Password,
                    Proxy_Network = PNetwork.Text,
                    Proxy_Url = PUrl.Text,
                    Username = Username.Text
                };
                ProxyController.Instance.TaskScheduler(request);
                MessageBox.Show("Configuration has been done.");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("application doesnt have sufficient permission, please run application as administrator.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
