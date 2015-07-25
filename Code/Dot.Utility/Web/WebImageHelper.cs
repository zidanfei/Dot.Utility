using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dot.Utility.Web
{
    public class WebImageHelper
    {
        public static Array MatchHtml(string html, string com)
        {
            List<string> urls = new List<string>();

            html = html.ToLower();

            //获取SRC标签中的URL
            Regex regexSrc = new Regex("src=\"[^\"]*[(.jpg)(.png)(.gif)(.bmp)(.ico)]\"");

            foreach (Match m in regexSrc.Matches(html))
            {
                string src = m.Value;

                src = src.Replace("src=", "").Replace("\"", "");
                if (!src.Contains("http"))
                    src = com + src;
                if (!urls.Contains(src))
                    urls.Add(src);
            }

            //获取HREF标签中URL
            Regex regexHref = new Regex("href=\"[^\"]*[(.jpg)(.png)(.gif)(.bmp)(.ico)]\"");
            foreach (Match m in regexHref.Matches(html))
            {
                string href = m.Value;
                href = href.Replace("href=", "").Replace("\"", "");
                if (!href.Contains("http"))
                    href = com + href;
                if (!urls.Contains(href))
                    urls.Add(href);
            }

            return urls.ToArray();
        }

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int mode);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int handle);
        const int STD_INPUT_HANDLE = -10;
        const int ENABLE_QUICK_EDIT_MODE = 0x40 | 0x80;

        public static void EnableQuickEditMode()
        {
            int mode;
            IntPtr handle = GetStdHandle(STD_INPUT_HANDLE);
            GetConsoleMode(handle, out mode);
            mode |= ENABLE_QUICK_EDIT_MODE;
            SetConsoleMode(handle, mode);
        }

        public static void DownLoadImage(string rootPath,string url)
        {
            EnableQuickEditMode();
            int oldCount = 0;
            Console.Title = "TakeImageFromInternet";
            string path = string.Format("{0}:\\Download\\loading\\", rootPath);
            while (true)
            {
                Console.Clear();
                string countFile = string.Format("{0}:\\CountFile.txt", rootPath);//用来计数的文本，以至于文件名不重复
                int cursor = 0;
                if (File.Exists(countFile))
                {
                    string text = File.ReadAllText(countFile);
                    try
                    {
                        cursor = oldCount = Convert.ToInt32(text);//次数多了建议使用long
                    }
                    catch
                    {
                    }
                }
                //Console.Write("please input a url:");
                //string temp = Console.ReadLine();
                //if (!string.IsNullOrEmpty(temp))
                //    url = temp;
                Match mcom = new Regex(@"^(?i)http://(\w+\.){2,3}(com(\.cn)?|cn|net)\b").Match(url);//获取域名
                string com = mcom.Value;
                //Console.WriteLine(mcom.Value);
                //Console.Write("please input a save path:");
                //temp = Console.ReadLine();
                //if (Directory.Exists(temp))
                //    path = temp;
                //Console.WriteLine();
                WebClient client = new WebClient();
                byte[] htmlData = null;
                htmlData = client.DownloadData(url);
                MemoryStream mstream = new MemoryStream(htmlData);
                string html = "";
                using (StreamReader sr = new StreamReader(mstream))
                {
                    html = sr.ReadToEnd();
                }
                Array urls = MatchHtml(html, com);

                foreach (string imageurl in urls)
                {
                    Console.WriteLine(imageurl);
                    byte[] imageData = null;
                    try
                    {
                        imageData = client.DownloadData(imageurl);
                    }
                    catch
                    {
                    }
                    if (imageData != null && imageData.Length > 0)
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            try
                            {

                                string ext = System.IO.Path.GetExtension(imageurl);
                                ImageFormat format = ImageFormat.Jpeg;
                                switch (ext)
                                {
                                    case ".jpg":
                                        format = ImageFormat.Jpeg;
                                        break;
                                    case ".bmp":
                                        format = ImageFormat.Bmp;
                                        break;
                                    case ".png":
                                        format = ImageFormat.Png;
                                        break;
                                    case ".gif":
                                        format = ImageFormat.Gif;
                                        break;
                                    case ".ico":
                                        format = ImageFormat.Icon;
                                        break;
                                    default:
                                        continue;
                                }
                                Image image = new Bitmap(ms);
                                if (Directory.Exists(path))
                                    image.Save(path + "\\" + cursor + ext, format);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    cursor++;
                }
                mstream.Close();
                File.WriteAllText(countFile, cursor.ToString(), Encoding.UTF8);
                Console.WriteLine("take done...image count:" + (cursor - oldCount).ToString());
            }
        }
    }
}
