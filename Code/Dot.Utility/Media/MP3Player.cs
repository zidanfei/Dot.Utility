using Dot.Utility.Net;
using System.IO;
using System.Runtime.InteropServices;

namespace Dot.Utility.Media
{
    /// <summary>
    /// MP3音乐播放
    /// </summary>
    public class MP3Player
    {

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="filePath"></param>
        public static void Play(string filePath)
        {
            mciSendString("close all", "", 0, 0);
            mciSendString("open " + GetFilePath(filePath) + " alias media", "", 0, 0);
            mciSendString("play media", "", 0, 0);
        }
        /// <summary>
        /// 暂停
        /// </summary>
        public static void Pause()
        {
            mciSendString("pause media", "", 0, 0);
        }
        /// <summary>
        /// 停止
        /// </summary>
        public static void Stop()
        {
            mciSendString("close media", "", 0, 0);
        }
        /// <summary>
        /// API函数
        /// </summary>
        /// <param name="lpstrCommand"></param>
        /// <param name="lpstrReturnString"></param>
        /// <param name="uReturnLength"></param>
        /// <param name="hwndCallback"></param>
        /// <returns></returns>
        [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Auto)]
        private static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);


        static string GetFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;
            string localFilePath = Path.GetFileName(filePath);
            var dir = System.AppDomain.CurrentDomain.BaseDirectory + "\\Voice\\";
            localFilePath = "Voice\\" + localFilePath;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(localFilePath))
            {
                if (filePath.StartsWith("http"))
                {
                    HttpHelper httpHelper = new HttpHelper();

                    httpHelper.Download(filePath, localFilePath);
                    return localFilePath;
                }
            }

            return filePath;
        }
    }


}