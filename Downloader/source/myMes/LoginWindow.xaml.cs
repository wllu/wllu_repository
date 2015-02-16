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
using System.Windows.Shapes;
using System.Xml;

using System.IO;
using System.Net;
namespace myMes
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
       public string token;
       public int i=0;
       public string pollKey, pollServer, pollTs;
       public List<int> numbers = new List<int>();
        public LoginWindow()
        {

            InitializeComponent();
            MainWindow main = this.Owner as MainWindow;
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = this.Owner as MainWindow;  // устанавливаем родителя окна
            string username = "";
            XmlDocument xml = new XmlDocument();// Сюда получаем ответ          
            auth avtor = new auth();
            token= avtor.Auth(loginText.Text, passText.Password); // получаем токен
           // Clipboard.SetText(token); // Копировать в буфер токен
            xml.Load(String.Format("https://api.vkontakte.ru/method/users.get.xml?fields=photo_50&access_token={0}", token));
            username = (xml.SelectSingleNode("response/user/first_name").InnerText) + " " + (xml.SelectSingleNode("response/user/last_name").InnerText);
            mw.UserName.Content = username; // устанавливаем имя и фамилию в окошке
            mw.Avatar.Source= GetPhoto(xml.SelectSingleNode("response/user"));
            xml.Load(String.Format("https://api.vkontakte.ru/method/friends.get.xml?v=5.27&access_token={0}", token));
            string countF = xml.SelectSingleNode("response/count").InnerText;
            xml.Load(String.Format("https://api.vkontakte.ru/method/friends.get.xml?fields=photo_50&order=hints&access_token={0}", token));
            foreach(XmlNode x in xml.SelectNodes("response/user"))
            {
                numbers.Add( Convert.ToInt32(x.SelectSingleNode("uid").InnerText)); // массив в айди друзей
                username= "  " +x.SelectSingleNode("first_name").InnerText + " " + x.SelectSingleNode("last_name").InnerText ;
                StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
                TextBlock tx = new TextBlock() { Text = username };
                Image image = new Image();
                image.Source = GetPhoto(x);
                image.Width = 37;
                image.Height = 37;
                sp.Children.Add(image);
               
                sp.Children.Add(tx);
                mw.Friendlist.Items.Add(sp);

            }
            this.Close();

        }

        private BitmapImage GetPhoto(XmlNode xm)   //Загрузка фото
      {
          WebClient wb = new WebClient();
          byte[] b1 = wb.DownloadData(xm.SelectSingleNode("photo_50").InnerText);
          MemoryStream m1 = new MemoryStream(b1);
          BitmapImage Picture1 = new BitmapImage();
          Picture1.BeginInit();
          Picture1.StreamSource = m1;
          Picture1.EndInit();
          return Picture1;
      }

    }
}