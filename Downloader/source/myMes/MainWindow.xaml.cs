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
using System.Data;
using System.Xml;
using System.Web;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json;
namespace myMes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        public int x= 5;
        public string token;
        public List<int> number2;
        public string t = "123";
        int i = 0;

        public string mesCoun;
       LoginWindow loginWin = new LoginWindow();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
      
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            this.WindowState = WindowState.Minimized;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loginWin.Owner = this;
            loginWin.ShowDialog();
        }

        public void Friendlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StackPanel st = new StackPanel();
            st = (StackPanel)Friendlist.SelectedItem; // обратно получаем стекпанель из листбокса
            TextBlock tx = new TextBlock(); // получаем текстблок из стекпанели
            tx = (TextBlock)st.Children[1];
            x = Convert.ToInt32(Friendlist.SelectedIndex);
            number2 = loginWin.numbers;
            mesCoun = number2.Count.ToString();
            token = loginWin.token; // дает токен каждый раз при двойном нажатии. Возможно получать токен один раз при загрузке окна
            Download dw = new Download();
            dw.Owner = this;
            dw.Show();
        }
    }
}
