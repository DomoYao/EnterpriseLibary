using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Enterprises.Framework.Utility
{
    /// <summary>
    /// 会议日历邮件
    /// </summary>
    public class NetMeetingManager
    {   // 发送邮件地址         
        string credentialEmail = "lfyao@ppfbh.com";
        // 发送邮件密码          
        string credentialPassword = "123456768";
        //邮件服务器
        string smtpserver = "hzsmtp1.ppfbh.com";
        // 邮件实体类          
        private readonly NetMeetingModel _calendarModel;
        /// <summary>         
        /// 构造函数         
        /// </summary>          
        public NetMeetingManager(NetMeetingModel model)
        {
            this._calendarModel = model;
        }
        /// <summary>        
        /// 发送邮件         
        /// </summary>          
        public void SendEmail()
        {
            try
            {
                #region 设置邮件信息                  
                MailMessage mail = new MailMessage();
                // 解决邮件标头的编码问题                 
                Encoding czCoding = Encoding.UTF8;
                // 邮件标头 
                mail.Headers.Add("Content-class", "urn:content-classes:calendarmessage");
                // 邮件主题                 
                mail.Subject = this._calendarModel.Summary;
                mail.SubjectEncoding = czCoding;
                // 设置邮件的优先级                 
                mail.Priority = MailPriority.Normal;
                // 设置邮件的发送通知                 
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.None;
                // 发件人邮件                  
                mail.From = new MailAddress(this.credentialEmail, this._calendarModel.Location, czCoding);
                // 收件人邮件                  
                foreach (string requiredemail in this._calendarModel.RequiredRecipients)
                {
                    if (!string.IsNullOrEmpty(requiredemail))
                    {
                        mail.To.Add(new MailAddress(requiredemail, requiredemail, czCoding));
                    }
                }
                // 传抄收件人邮件                  
                foreach (string optionalemail in this._calendarModel.OptionalRecipients)
                {
                    if (!string.IsNullOrEmpty(optionalemail))
                    {
                        mail.CC.Add(new MailAddress(optionalemail, optionalemail, czCoding));
                    }
                }
                // 密传抄收件人邮件                  
                foreach (string resourceemail in this._calendarModel.ResourceRecipients)
                {
                    if (!string.IsNullOrEmpty(resourceemail))
                    {
                        mail.Bcc.Add(new MailAddress(resourceemail, resourceemail, czCoding));
                    }
                }
                //对组织者进行传抄一份                  
                if (!string.IsNullOrEmpty(this._calendarModel.OrganizerEmail))
                {
                    mail.CC.Add(new MailAddress(this._calendarModel.OrganizerEmail, this._calendarModel.OrganizerName, czCoding));
                }
                // 设置邮件正文   
                mail.Body = string.IsNullOrEmpty(this._calendarModel.Body) ? $"{this._calendarModel.OrganizerName}:{this._calendarModel.Description}" : this._calendarModel.Body;

                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;

                // 附件
                Attachments(mail,this._calendarModel.Paths);

                // 在邮件中设置不同的Mime类型                  
                // ContentType typeText = new ContentType("text/plain");
                ContentType typeHtml = new ContentType("text/html");
                ContentType typeCalendar = new ContentType("text/calendar");
                //向calendar header添加参数                  
                if (typeCalendar.Parameters != null)
                {
                    typeCalendar.Parameters.Add("method", "REQUEST");
                    typeCalendar.Parameters.Add("charset", "utf-8");
                    typeCalendar.Parameters.Add("name", "meeting.ics");
                }

                //使用文本类型创建邮件的body部分 
                //AlternateView viewText = AlternateView.CreateAlternateViewFromString(this.CalendarModel.Body, typeText); 
                //mail.AlternateViews.Add(viewText);
                // 使用html格式创建邮件的body部分 
                AlternateView viewHtml = AlternateView.CreateAlternateViewFromString(this._calendarModel.Body, typeHtml);
                mail.AlternateViews.Add(viewHtml);
                // 使用vcalendar格式创建邮件的body部分                 
                AlternateView viewCalendar = AlternateView.CreateAlternateViewFromString(this.CalendarGenerate(this._calendarModel.Body), typeCalendar);
                viewCalendar.TransferEncoding = TransferEncoding.SevenBit;
                mail.AlternateViews.Add(viewCalendar);
                SmtpClient smtp = new SmtpClient(this.smtpserver);
                // 是否使用默认证书                  
                smtp.UseDefaultCredentials = false;
                // 获取发件人验证证书                  
                smtp.Credentials = new NetworkCredential(this.credentialEmail, this.credentialPassword);
                // 通过网络处理待发电子邮件                 
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                // 使用套间字加密连接                 
                smtp.EnableSsl = false;
                // 发送邮件 
                smtp.Send(mail);
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>  
        /// 添加附件  
        /// </summary>  
        public void Attachments(MailMessage mailMessage, List<string> path)
        {
            if (path == null || path.Count < 1)
            {
                return;
            }

            foreach (var t in path)
            {
                var data = new Attachment(t, MediaTypeNames.Application.Octet);
                var disposition = data.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(t);// 获取附件的创建日期  
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(t);// 获取附件的修改日期  
                disposition.ReadDate = System.IO.File.GetLastAccessTime(t);// 获取附件的读取日期  
                mailMessage.Attachments.Add(data);// 添加到附件中  
            }
        }

        /// <summary>          
        /// 生成ics文件（为以Outlook主要参考）         
        /// </summary>          
        /// <returns></returns>         
        private string CalendarGenerate(string strBodyHtml)
        {
            string calDateFormat = "yyyyMMddTHHmmssZ";
            StringBuilder sb = new StringBuilder();
            sb.Append("BEGIN:VCALENDAR").Append("\r\n");
            sb.Append("PRODID:").Append("-//Microsoft Corporation//Outlook 12.0 MIMEDIR//EN").Append("\r\n");
            sb.Append("VERSION:2.0").Append("\r\n");
            sb.Append("METHOD:REQUEST").Append("\r\n");
            sb.Append("X-MS-OLK-FORCEINSPECTOROPEN:TRUE").Append("\r\n");
            sb.Append("BEGIN:VEVENT").Append("\r\n");
            if (!string.IsNullOrEmpty(this._calendarModel.OrganizerEmail))
            {
                sb.Append("ORGANIZER;CN=\"").Append(this._calendarModel.OrganizerName).Append("\":MAILTO" + ":").Append(this._calendarModel.OrganizerEmail).Append("\r\n");
            }
            foreach (string recipient in this._calendarModel.RequiredRecipients)
            {
                sb.Append("ATTENDEE;CN=\"" + recipient + "\";RSVP=TRUE:mailto:" + recipient).Append("\r\n");
            }
            foreach (string recipient in this._calendarModel.OptionalRecipients)
            {
                sb.Append("ATTENDEE;CN=\"" + recipient + "\";ROLE=OPT-PARTICIPANT;RSVP=TRUE:mailto:" + recipient).Append("\r\n");
            }
            foreach (string recipient in this._calendarModel.ResourceRecipients)
            {
                sb.Append("ATTENDEE;CN=\"" + recipient + "\";CUTYPE=RESOURCE;ROLE=NON-PARTICIPANT;RSVP=TRUE:mailto:" + recipient).Append("\r\n");
            }
            sb.Append("CLASS:PUBLIC").Append("\r\n");
            sb.Append("CREATED:").Append(DateTime.Now.ToUniversalTime().ToString(calDateFormat)).Append("\r\n");
            sb.Append("DESCRIPTION:").Append(this.NotNull(this._calendarModel.Description)).Append("\r\n");
            sb.Append("DTEND:").Append(this._calendarModel.End.ToUniversalTime().ToString(calDateFormat)).Append("\r\n");
            sb.Append("DTSTAMP:").Append(DateTime.Now.ToUniversalTime().ToString(calDateFormat)).Append("\r\n");
            sb.Append("DTSTART:").Append(this._calendarModel.Start.ToUniversalTime().ToString(calDateFormat)).Append("\r\n");
            sb.Append("LAST-MODIFIED:").Append(DateTime.Now.ToUniversalTime().ToString(calDateFormat)).Append("\r\n");
            sb.Append("LOCATION:").Append(this.NotNull(this._calendarModel.Location)).Append("\r\n");
            sb.Append("PRIORITY:5").Append("\r\n");
            sb.Append("SEQUENCE:0").Append("\r\n");
            sb.Append("SUMMARY:").Append(this.NotNull(this._calendarModel.Summary)).Append("\r\n");
            sb.Append("UID:").Append(Guid.NewGuid().ToString().Replace("-", "") + Guid.NewGuid().ToString().Replace("-", "")).Append("\r\n");
            sb.Append("X-ALT-DESC;FMTTYPE=text/html:").Append(strBodyHtml).Append("\r\n");
            sb.Append("STATUS:CONFIRMED").Append("\r\n");
            sb.Append("TRANSP:OPAQUE").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-BUSYSTATUS:BUSY").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-IMPORTANCE:1").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-INSTTYPE:0").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-INTENDEDSTATUS:BUSY").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-ALLDAYEVENT:FALSE").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-OWNERAPPTID:-611620904").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-APPT-SEQUENCE:0").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-ATTENDEE-CRITICAL-CHANGE:").Append(DateTime.Now.ToUniversalTime().ToString(calDateFormat)).Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-OWNER-CRITICAL-CHANGE:").Append(DateTime.Now.ToUniversalTime().ToString(calDateFormat)).Append("\r\n");
            sb.Append("BEGIN:VALARM").Append("\r\n");
            sb.Append("ACTION:DISPLAY").Append("\r\n");
            sb.Append("DESCRIPTION:REMINDER").Append("\r\n");
            sb.Append("TRIGGER;RELATED=START:-PT00H15M00S").Append("\r\n");
            sb.Append("END:VALARM").Append("\r\n"); sb.Append("END:VEVENT").Append("\r\n");
            sb.Append("END:VCALENDAR").Append("\r\n");
            return sb.ToString();
        }
        /// <summary>         
        /// 设定不为null值         
        /// </summary>         
        /// <param name="str"></param>         
        /// <returns></returns>          
        private string NotNull(string str)
        {
            return str ?? String.Empty;
        }

    }


    /// <summary>
    /// 使用net发送邮件实体类
    /// </summary>
    public class NetMeetingModel
    {
        private DateTime start;
        /// <summary>         
        /// 会议起始时间        
        /// </summary>       
        public DateTime Start
        {
            get { return this.start; }
            set { this.start = value; }
        }
        private DateTime end;
        /// <summary>         
        /// 会议结束时间         
        /// </summary>        
        public DateTime End
        {
            get { return this.end; }
            set { this.end = value; }
        }
        private string summary;
        /// <summary>         
        /// 会议主题         
        /// </summary>        
        public string Summary
        {
            get { return this.summary; }
            set { this.summary = value; }
        }
        private string location;
        /// <summary>         
        /// 会议地点         
        /// </summary>          
        public string Location
        {
            get { return this.location; }
            set { this.location = value; }
        }
        private string description;
        /// <summary>         
        /// 会议的详细说明         
        /// </summary>        
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }
        private string organizerName;
        /// <summary>         
        /// 发送人姓名        
        /// </summary>         
        public string OrganizerName
        {
            get { return this.organizerName; }
            set { this.organizerName = value; }
        }
        private string organizerEmail;
        /// <summary>         
        /// 发送人邮箱         
        /// </summary>          
        public string OrganizerEmail
        {
            get { return this.organizerEmail; }
            set { this.organizerEmail = value; }
        }
        private List<string> requiredRecipients = new List<string>();
        /// <summary>         
        /// 必选收件人邮件         
        /// </summary>          
        public List<string> RequiredRecipients
        {
            get { return this.requiredRecipients; }
            set { this.requiredRecipients = value; }
        }
        private List<string> optionalRecipients = new List<string>();
        /// <summary>        
        /// 传抄收件人邮件        
        /// </summary>         
        public List<string> OptionalRecipients
        {
            get { return this.optionalRecipients; }
            set { this.optionalRecipients = value; }
        }
        private List<string> resourceRecipients = new List<string>();
        /// <summary>        
        /// 密传抄收件人邮件         
        /// </summary>         
        public List<string> ResourceRecipients
        {
            get { return this.resourceRecipients; }
            set { this.resourceRecipients = value; }
        }

        private string body;
        /// <summary>         
        /// 邮件内容       
        /// </summary>          
        public string Body
        {
            get { return this.body; }
            set { this.body = value; }
        }


        /// <summary>         
        /// 邮件附件  
        /// </summary>          
        public List<string> Paths { get; set; }
    }
}

    
