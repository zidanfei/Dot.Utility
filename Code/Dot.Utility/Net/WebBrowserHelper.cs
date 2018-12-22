using mshtml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dot.Utility.Net
{
    //引用com类库 Microsoft HTML Object Library

    public class WebBrowserHelper
    {
        /// <summary>  
        /// 获取WebBrowser指定的图片  
        /// </summary>  
        /// <param name="webBrowser">需要获取图片的WebBrowser</param>  
        /// <param name="imgID">指定的图片的id(优先查找指定id)</param>  
        /// <param name="imgSrc">指定的图片的src，支持模糊查询</param>  
        /// <param name="imgAlt">指定的图片的src， 支持模糊查询</param>  
        /// <returns></returns>  
        public static Image GetRegCodePic(ref WebBrowser webBrowser, String imgID, String imgSrc, String imgAlt)
        {

            HTMLDocument doc = (HTMLDocument)webBrowser.Document.DomDocument;
            HTMLBody body = (HTMLBody)doc.body;
            IHTMLControlRange rang = (IHTMLControlRange)body.createControlRange();
            IHTMLControlElement img;

            // 如果没有图片的ID,通过Src或Alt中的关键字来取  
            if (imgID.Length == 0)
            {
                Int32 ImgNum = GetPicIndex(ref webBrowser, ref imgSrc, ref imgAlt);

                if (ImgNum == -1)
                    return null;

                img = (IHTMLControlElement)webBrowser.Document.Images[ImgNum].DomElement;
            }
            else
                img = (IHTMLControlElement)webBrowser.Document.All[imgID].DomElement;

            rang.add(img);
            rang.execCommand("Copy", false, null);
            Image regImg = Clipboard.GetImage();
            Clipboard.Clear();
            return regImg;
        }

        /// <summary>
        /// 返回指定WebBrowser中图片<IMG></IMG>中的图内容
        /// </summary>
        /// <param name="WebCtl">WebBrowser控件</param>
        /// <param name="ImgeTag">IMG元素</param>
        /// <returns>IMG对象</returns>
        private Image GetWebImage(WebBrowser WebCtl, HtmlElement ImgeTag)
        {

            HTMLDocument doc = (HTMLDocument)WebCtl.Document.DomDocument;
            HTMLBody body = (HTMLBody)doc.body;
            IHTMLControlRange rang = (IHTMLControlRange)body.createControlRange();
            IHTMLControlElement Img = (IHTMLControlElement)ImgeTag.DomElement; //图片地址

            Image oldImage = Clipboard.GetImage();
            rang.add(Img);
            rang.execCommand("Copy", false, null);  //拷贝到内存
            Image numImage = Clipboard.GetImage();
            Clipboard.SetImage(oldImage);
            return numImage;
        }
        /// <summary>  
        /// 获取WebBrowser指定图片的索引  
        /// </summary>  
        /// <param name="webBrowser">指定的WebBrowser</param>  
        /// <param name="imgSrc">指定的图片src，支持模糊查询</param>  
        /// <param name="imgAlt">指定的图片alt，支持模糊查询</param>  
        /// <returns></returns>  
        public static Int32 GetPicIndex(ref WebBrowser webBrowser, ref String imgSrc, ref String imgAlt)
        {
            IHTMLImgElement img;

            // 获取所有的Image元素  
            for (Int32 i = 0; i < webBrowser.Document.Images.Count; i++)
            {
                img = (IHTMLImgElement)webBrowser.Document.Images[i].DomElement;

                if (imgAlt.Length == 0)
                {
                    if (img.src.IndexOf(imgSrc) >= 0)
                        return i;
                }
                else
                {
                    if (imgSrc.Length == 0)
                    {
                        // 当imgSrc为空时，只匹配imgAlt  
                        if (img.alt.IndexOf(imgAlt) >= 0)
                            return i;
                    }
                    else
                    {
                        // 当imgSrc不为空时，匹配imgAlt和imgSrc任意一个  
                        if (img.alt.IndexOf(imgAlt) >= 0 || img.src.IndexOf(imgSrc) >= 0)
                            return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 根据Name获取元素
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public HtmlElement GetElement_Name(WebBrowser wb, string Name)
        {
            HtmlElement e = wb.Document.All[Name];
            return e;
        }
        /// <summary>
        /// 根据Id获取元素
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public HtmlElementCollection GetElement_Tag(WebBrowser wb, string tag)
        {
            HtmlElementCollection e = wb.Document.GetElementsByTagName(tag);
            return e;
        }
        /// <summary>
        /// 根据Id获取元素
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public HtmlElement GetElement_Id(WebBrowser wb, string id)
        {
            HtmlElement e = wb.Document.GetElementById(id);
            return e;
        }

        /// <summary>
        /// 根据Index获取元素
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public HtmlElement GetElement_Index(WebBrowser wb, int index)
        {
            HtmlElement e = wb.Document.All[index];
            return e;
        }

        /// <summary>
        /// 获取form表单名name,返回表单
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="form_name"></param>
        /// <returns></returns>
        public HtmlElement GetElement_Form(WebBrowser wb, string form_name)
        {
            HtmlElement e = wb.Document.Forms[form_name];
            return e;
        }


        /// <summary>
        /// 设置元素value属性的值
        /// </summary>
        /// <param name="e"></param>
        /// <param name="value"></param>
        public void Write_value(HtmlElement e, string value)
        {
            e.SetAttribute("value", value);
        }
        /// <summary>
        /// 执行元素的方法，如：click，submit(需Form表单名)等
        /// </summary>
        /// <param name="e"></param>
        /// <param name="s"></param>
        public void Btn_click(HtmlElement e, string s)
        {

            e.InvokeMember(s);
        }
    }
}
