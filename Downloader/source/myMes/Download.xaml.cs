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
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Xml;

namespace myMes
{
    /// <summary>
    /// Interaction logic for Download.xaml
    /// </summary>
    /// 

    public partial class Download : Window
    {
        XmlDocument xml = new XmlDocument();
        string path = "";
        int countMess = 0;
        int cm=0;
        string url;
        int tm = 500;
        public Download()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowDialog();
            path = dlg.SelectedPath;
            pathbox.Text = path;
            }

        private void Download1_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            
            xml.Load(String.Format("https://api.vkontakte.ru/method/messages.getHistory.xml?user_id={0}&rev=0&access_token={1}", main.number2[main.x].ToString(), main.token));
            countMess = Convert.ToInt32(xml.SelectSingleNode("response/count").InnerText);
            frId.Content =  main.number2[main.x].ToString();
            mscCo.Content = countMess;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            if (timeout.Text == null || Convert.ToInt32(timeout.Text) > 10000 || Convert.ToInt32(timeout.Text) < 0)
            {
                System.Windows.MessageBox.Show("Неверное значение timeout");
            }
            else
            {
                tm = Convert.ToInt32(timeout.Text);
                if (Directory.Exists(path))
                {
                    path = path + "\\";
                    for (int i = 0; i <= countMess; i = i + 200)
                    {
                        xml.Load(String.Format("https://api.vkontakte.ru/method/messages.getHistory.xml?user_id={0}&count=200&offset={1}&rev=1&access_token={2}", main.number2[main.x].ToString(), i, main.token));// получили первые 200 

                        foreach (XmlNode x in xml.SelectNodes("response/message")) // проходим по всем узлам message
                        {
                            if (x.SelectSingleNode("attachments") != null)
                            {
                                foreach (XmlNode xn in x.SelectNodes("attachments/attachment")) // проходим по всем узлам с материалами
                                {
                                    if (xn.SelectSingleNode("type").InnerText == "photo")
                                    {
                                        if (xn.SelectSingleNode("photo/src_xxbig") != null)
                                        {
                                            url = xn.SelectSingleNode("photo/src_xxbig").InnerText;
                                        }
                                        else if (xn.SelectSingleNode("photo/src_xbig") != null)
                                            url = xn.SelectSingleNode("photo/src_xbig").InnerText;
                                        else url = xn.SelectSingleNode("photo/src_big").InnerText;
                                        all.Items.Add(xn.SelectSingleNode("photo/src").InnerText); // выводим ссылки в лист бокс
                                        cm++;
                                        saveImage(url, path);
                                        Thread.Sleep(500);
                                    }


                                }

                            }

                        }


                    }
                    System.Windows.MessageBox.Show("Всего загруженно: " + cm.ToString());

                }

                else System.Windows.MessageBox.Show("Ошибка, путь не наден", "Error");
            }
        }

      public void saveImage(string url, string path)
        {
            WebClient wc = new WebClient();
            wc.DownloadFileAsync(new Uri(url),path + System.IO.Path.GetFileName(url));
            
        }
        
        
    }

}
