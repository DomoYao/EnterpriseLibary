using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Enterprises.Framework.Plugin.Domain.AdManager
{
    /// <summary>
    /// ����Ĭ�ϲ���
    /// ����Ӧ�������������ļ��У����û�����ã���Ӧ���ֶ����á�
    /// </summary>
    public class ParaMgr
    {
        private ParaMgr() { }

        private static object s_lock = new object();
        private static volatile ParaMgr s_value = null;

        private static ParaMgr Value
        {
            get 
            {
                if (s_value == null)
                {
                    lock (s_lock)
                    {
                        if (s_value == null)
                        {
                            s_value = new ParaMgr();
                            NameValueCollection setting =  ConfigurationManager.AppSettings;
                            // TODO:���������ã�AD_Admin��AD_Password�����м��ܣ���ʱӦ�����ܡ�
                            if (!String.IsNullOrEmpty(setting["AD_Admin"]))
                                s_value.userName = setting["AD_Admin"];
                            if (!String.IsNullOrEmpty(setting["AD_Domain"]))
                                s_value.domain = setting["AD_Domain"];
                            if (!String.IsNullOrEmpty(setting["AD_Domain2000"]))
                                s_value.domain2000 = setting["AD_Domain2000"];
                            if (!String.IsNullOrEmpty(setting["AD_Password"]))
                                s_value.password = setting["AD_Password"];
                            if (!String.IsNullOrEmpty(setting["AD_Path"]))
                                s_value.fullPath = setting["AD_Path"];
                        }
                    }
                }

                return s_value;
            }
        }

        /// <summary>
        /// ����ADsPathǰ׺��
        /// </summary>
        public const string LDAP_IDENTITY = "LDAP://";

        private string userName;        // AD �û�����eg:administrator
        private string domain;          // Domain Name��eg:maodou.com
        private string domain2000;      // Domain Name 2000��eg:maodou
        private string password;        // AD �û�����
        private string fullPath;        // Domain DN��eg:DC=maodou,DC=com�����Ժ�maodou.com����ת��


        /// <summary>
        /// ����ϵͳ����
        /// </summary>
        /// <param name="userName">AD �û���</param>
        /// <param name="domain">AD ������</param>
        /// <param name="password">AD �û�����</param>
        /// <param name="fullPath">AD ��·����DN��</param>
        public static void SetSysPara(string userName, string domain, string password, string fullPath)
        {
            if (!String.IsNullOrEmpty(userName))
            {
                Value.userName = userName;
            }
            if (!String.IsNullOrEmpty(domain))
            {
                Value.domain = domain;
            }
            if (!String.IsNullOrEmpty(password))
            {
                Value.password = password;
            }
            if (!String.IsNullOrEmpty(userName))
            {
                Value.fullPath = fullPath;
            }
        }

        /// <summary>
        /// ��ȡAD �û���
        /// </summary>
        public static string ADUserName
        {
            get { return Value.userName; }
        }

        /// <summary>
        /// ��ȡAD ������
        /// </summary>
        public static string ADDomain
        {
            get { return Value.domain; }
        }

        /// <summary>
        /// ��ȡAD ������2000
        /// </summary>
        public static string ADDomain2000
        {
            get { return Value.domain2000; }
        }

        /// <summary>
        /// ��ȡAD �û�����
        /// </summary>
        public static string ADPassword
        {
            get { return Value.password; }
        }

        /// <summary>
        /// ��ȡAD ��·����DN��
        /// </summary>
        public static string ADFullPath
        {
            get { return Value.fullPath; }
        }

        /// <summary>
        /// ��ȡAD �û���
        /// </summary>
        public static string ADFullName
        {
            get { return String.Format("{0}@{1}", Value.userName, Value.domain); }
        }
        
    }
}
