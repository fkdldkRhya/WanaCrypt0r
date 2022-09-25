using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace _WanaDecryptor_.Utils.Crypto
{
    class AESManager
    {
        // 암호화 IV 값
        private byte[] encryptIV = new byte[] { 1, 0, 7, 3, 4, 2, 0, 3, 9, 6, 0, 6, 2, 5, 6, 8 };




        /// <summary>
        /// AES 문자열 복호화
        /// </summary>
        /// <param name="str">복호화 문자열</param>
        /// <returns>복호화 성공 문자열</returns>
        public string TextAESDecrypt(string str)
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
                var decrypt = aes.CreateDecryptor();
                byte[] xBuff = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Convert.FromBase64String(str);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }
                return Encoding.UTF8.GetString(xBuff);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// AES 문자열 암호화
        /// </summary>
        /// <param name="str">암호화 문자열</param>
        /// <returns>암호화 성공 문자열</returns>
        public string TextAESEncrypt(string str)
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



        /// <summary>
        /// 파일 AES 복호화 함수
        /// </summary>
        /// <param name="inputFile">파일 경로 (입력)</param>
        /// <param name="outputFile">파일 경로 (출력)</param>
        /// <param name="key">복호화 키</param>
        public void FileAESDecrypt(string inputFile, string outputFile, string key)
        {
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] salt = new byte[32];
            FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
            fsCrypt.Read(salt, 0, salt.Length);
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Key = Encoding.UTF8.GetBytes(key);
            AES.IV = encryptIV;
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CFB;
            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);
            FileStream fsOut = new FileStream(outputFile, FileMode.Create);
            int read;
            byte[] buffer = new byte[1048576];
            try
            {
                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                    fsOut.Write(buffer, 0, read);
            }
            catch (CryptographicException) { }
            catch (Exception) { }

            try
            {
                cs.Close();
            }
            catch (Exception) { }
            finally
            {
                fsOut.Close();
                fsCrypt.Close();
            }
        }
    }
}
