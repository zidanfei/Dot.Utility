using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Log
{
    public class LogMessage
    {
        public LogMessage()
        {
            ExtendPropety = new Dictionary<string, string>();
        }

        [Display(Name = "�ͻ���Ip")]
        public string ClientIP { get; set; }
        [Display(Name = "������Ip")]
        public string ServerIP { get; set; }

        [Display(Name = "�����ַ")]
        public string RequestUrl { get; set; }
        [Display(Name = "�û�ID")]
        public string UserId { get; set; }
        [Display(Name = "�û���")]
        public string UserName { get; set; }
        [Display(Name = "��ʾ��")]
        public string DisplayName { get; set; }

        [Display(Name = "��־��¼")]
        public object Message { get; set; }

        [Display(Name = "Area")]
        public string AreaName { get; set; }

        [Display(Name = "Controller")]
        public string ControllerName { get; set; }

        [Display(Name = "Action")]
        public string ActionName { get; set; }

        public Dictionary<string,string> ExtendPropety { get; set; }
        
    }
}
