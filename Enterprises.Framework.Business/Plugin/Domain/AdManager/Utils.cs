using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;
using Enterprises.Framework.Plugin.Domain.AdManager.Enum;

namespace Enterprises.Framework.Plugin.Domain.AdManager
{
    /// <summary>
    /// ������
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// ȥ��ADsPath�е�ǰ�á�LADP://��
        /// </summary>
        /// <param name="adsPath">ADsPath</param>
        /// <returns></returns>
        internal static string RemoveLDAPPre(string adsPath)
        {
            return adsPath.Substring(7);
        }



        /// <summary>
        /// ����DirectoryEntry���������
        /// </summary>
        /// <param name="de">DirectoryEntry����</param>
        /// <param name="propertyName">������</param>
        /// <param name="propertyValue">����ֵ</param>
        internal static void SetProperty(DirectoryEntry de, string propertyName, string propertyValue)
        {
            if (de.Properties.Contains(propertyName))      // �Ѱ���
            {
                if (String.IsNullOrEmpty(propertyValue))
                {
                    de.Properties[propertyName].Value = null;            // ���
                }
                else
                {
                    de.Properties[propertyName][0] = propertyValue;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(propertyValue))
                {
                    de.Properties[propertyName].Add(propertyValue);
                }
            }
        }
        
        /// <summary>
        /// ����DirectoryEntry���������
        /// </summary>
        /// <param name="de">DirectoryEntry����</param>
        /// <param name="propertyName">������</param>
        /// <param name="propertyValue">����ֵ</param>
        internal static void SetProperty(DirectoryEntry de, string propertyName, int propertyValue)
        {
            if (de.Properties.Contains(propertyName))
            {
                de.Properties[propertyName][0] = propertyValue;
            }
            else
            {
                de.Properties[propertyName].Add(propertyValue);
            }
        }
        
        /// <summary>
        /// ��ȡDirectoryEntry���������ֵ
        /// </summary>
        /// <param name="de">DirectoryEntry����</param>
        /// <param name="propertyName">������</param>
        /// <returns></returns>
        internal static string GetProperty(DirectoryEntry de, string propertyName)
        {
            if (de.Properties.Contains(propertyName))
            {
                return de.Properties[propertyName][0].ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ��ȡSearchResult���������ֵ
        /// </summary>
        /// <param name="result">SearchResult����</param>
        /// <param name="propertyName">������</param>
        /// <returns></returns>
        internal static string GetProperty(SearchResult result, string propertyName)
        {
            if (result.Properties.Contains(propertyName))
            {
                return result.Properties[propertyName][0].ToString();
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// ���ֽ�����ת��Ϊ16������ʽ���ַ���
        /// </summary>
        /// <param name="ids">�ֽ�����</param>
        /// <returns></returns>
        public static string ConvertToOctetString(byte[] ids)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in ids)
            {
                sb.AppendFormat(@"\{0:x2}", b);
            }
            return sb.ToString();
        }

        /// <summary>
        /// ��Guidת��ΪNativeGuid
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns></returns>
        public static string ConvertGuidToNativeGuid(Guid guid)
        {
            // GUIDĬ�ϵ�ToString()��ת�������ֽ�Ϊ16����ʱ�������ǰ����ֽ�˳��������У�������������
            // NativeGuidҪ����ȫ�����ֽ�˳��
            StringBuilder sb = new StringBuilder();
            byte[] byteArr = guid.ToByteArray();
            foreach (byte b in byteArr)
            {
                sb.AppendFormat(@"{0:x2}", b);
            }
            return sb.ToString();
        }

        /// <summary>
        /// ��NativeGuidת��ΪGuid
        /// </summary>
        /// <param name="nativeGuid">NativeGuid</param>
        /// <returns></returns>
        public static Guid ConvertNativeGuidToGuid(string nativeGuid)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byteArr = new byte[16];
            for (int i = 0; i < 16;i++ )
            {
                byteArr[i] = byte.Parse(nativeGuid[2 * i].ToString().ToUpper() + nativeGuid[2 * i + 1].ToString().ToUpper(),
                    System.Globalization.NumberStyles.HexNumber);
            }
            return new Guid(byteArr);
        }

        /// <summary>
        /// ������ת��ΪDN��ʽ
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static string ConvertDomainNameToDN(string domainName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string dc in domainName.Split('.'))
            {
                sb.AppendFormat("DC={0},", dc.ToUpper());
            }
            return sb.ToString(0, sb.Length - 1);
        }

        /// <summary>
        /// ��DN��ʽ��ת��Ϊ������ʽ
        /// </summary>
        /// <param name="dn"></param>
        /// <returns></returns>
        public static string ConvertDNToDomainName(string dn)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string dc in dn.Split(','))
            {
                sb.AppendFormat("{0}.", dc.Trim().Split('=')[1]);
            }
            return sb.ToString(0, sb.Length - 1);

        }



        /// <summary>
        /// �ж�DirectoryEntry�����Ƿ������Ϊ����(container,ou,builtinDomain)
        /// </summary>
        /// <param name="de">DirectoryEntry����</param>
        /// <returns></returns>
        internal static bool CanAsContainer(DirectoryEntry de)
        {
            string schemaClassName = de.SchemaClassName;

            return ((schemaClassName == SchemaClass.organizationalUnit.ToString("F")) ||
                (schemaClassName == SchemaClass.builtinDomain.ToString("F")) ||
                (schemaClassName == SchemaClass.container.ToString("F")));
        }



        /// <summary>
        /// ����DN�õ�RDN
        /// </summary>
        /// <param name="dn">DN</param>
        /// <returns></returns>
        public static string GetRDNValue(string dn)
        {
            string part = SplitDN(dn)[0];
            return part.Substring(part.IndexOf('=')+1).Trim();
        }

        /// <summary>
        /// ����DN�õ�RDN
        /// </summary>
        /// <param name="dn">DN</param>
        /// <returns></returns>
        public static string GetRDN(string dn)
        {
            return SplitDN(dn)[0];
        }

        /// <summary>
        /// ���ݶ����GUID����ADsPath��LDAP://&lt;GUID=xxxxxxxxxxxxxxxx&gt;��ʽ
        /// </summary>
        /// <param name="guid">�����GUID��</param>
        /// <returns></returns>
        public static string GenerateADsPath(Guid guid)
        {
            return ParaMgr.LDAP_IDENTITY + "<GUID=" + Utils.ConvertGuidToNativeGuid(guid) + ">";
        }

        /// <summary>
        /// ת��DN��ת��'\'���š�
        /// </summary>
        /// <param name="odn">DN��</param>
        /// <returns></returns>
        public static string EscapeDNBackslashedChar(string odn)
        {
            // #+="><,/;\
            return odn.Replace("/", "\\/"); 
        }

        /// <summary>
        /// ת��DN
        /// </summary>
        /// <param name="odn">DN��</param>
        /// <returns></returns>
        /// <remarks>û��ת����ĵ�DN�����ܼ����ظ�ת�塣</remarks>
        internal static string EscapeDN(string odn)
        {
            // #+="><,/;\
            return odn.Replace("\\", "\\\\").Replace("#", "\\#").Replace("+", "\\+").Replace("=", "\\=").Replace("\"", "\\\"").Replace(">", "\\>").Replace("<", "\\<").Replace(",", "\\,").Replace(";", "\\;").Replace("/", "\\/");
        }


        /// <summary>
        /// ��ת��DN����ת��'\'���š�
        /// </summary>
        /// <param name="dn"></param>
        /// <returns></returns>
        public static string UnEscapeDNBackslashedChar(string dn)
        {
            // #+="><,/;\
            return dn.Replace("\\/", "/");
        }

        /// <summary>
        /// ��ת��DN��
        /// </summary>
        /// <param name="dn">ת����ĵ�DN��</param>
        /// <returns></returns>
        internal static string UnEscapeDN(string dn)
        {
            // #+="><,/;\
            return dn.Replace("\\#", "#").Replace("\\+", "+").Replace("\\=", "=").Replace("\\\"", "\"").Replace("\\>", ">").Replace("\\<", "<").Replace("\\,", ",").Replace("\\;", ";").Replace("\\/", "/").Replace("\\\\", "\\");
        }

        /// <summary>
        /// �ָ�DN(ת���δת���)��������
        /// </summary>
        /// <param name="dn">DN(ת���δת���)��</param>
        /// <returns></returns>
        /// <remarks>���ָ�����κθı䡣����ת������</remarks>
        public static string[] SplitDN(string dn)
        {
            List<string> parts = new List<string>();
            int l = 0;
            int i = 0;
            for (; i < dn.Length; i++)
            {
                if (dn[i] == ',' && dn[i - 1] != '\\')
                {
                    parts.Add(dn.Substring(l, i-l));
                    l = i + 1;
                }
            }
            if (l < i - 1)
            {
                string last = dn.Substring(l, i - l);
                if (!String.IsNullOrEmpty(last.Trim()))
                    parts.Add(last.Trim());
            }
            return parts.ToArray();
        }

        public static string GetParentDN(string dn)
        {
            int i = 0;
            for (; i < dn.Length; i++)
            {
                if (dn[i] == ',' && dn[i - 1] != '\\')
                {
                    break;
                }
            }

            if (i == dn.Length)
                return null;
            else
                return dn.Substring(i+1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rdn">rdn����ȫת�塣(��=��ǰ����)</param>
        /// <param name="parentDN">����DN����ȫת�塣</param>
        /// <returns>DN</returns>
        /// <remarks>����DN����ַ�����ת�塣</remarks>
        public static string GenerateDN(string rdn, string parentDN)
        {
            if (!String.IsNullOrEmpty(parentDN))
                return String.Format("{0},{1}",
                    rdn, 
                    parentDN);
            else
                return rdn;
        }

        /// <summary>
        /// ����RDN��CN=***
        /// </summary>
        /// <param name="cn">RDNֵ��δת�塣</param>
        /// <returns></returns>
        public static string GenerateRDNCN(string cn)
        {
            return String.Format("CN={0}",EscapeDN(cn));
        }

        /// <summary>
        /// ����RDN��OU=***
        /// </summary>
        /// <param name="ou">RDNֵ��δת�塣</param>
        /// <returns></returns>
        public static string GenerateRDNOU(string ou)
        {
            return String.Format("OU={0}", EscapeDN(ou));
        }


        /// <summary>
        /// δDirectorySearcher��ѯ����ת�塣
        /// </summary>
        /// <param name="cn"></param>
        /// <returns></returns>
        public static string Escape4Query(string cn)
        {
            return cn.Replace("(", "\\28").Replace(")", "\\29").Replace("\\", "\\5c");
        }



        /// <summary>
        /// SAMAccountName�����ַ���
        /// </summary>
        public static char[] InvalidSAMAccountNameChars = new char[] { 
                '/', '\\', '[', ']', ':', ';', '|', '=', 
                ',', '+',  '*', '?', '<', '>', '"'};

        // ����DirectoryEntryʱ��ͨ��UserName��Password�ṩƾ��
        // ���û���ṩ����ʹ�õ�ǰ���̵�ƾ��
        // ֻҪƾ����Ч���ɴ���DirectoryEntry
        // ��������в���ʱ����SetPassword������Ҫ��ƾ�ݽ��з��ʿ��ƣ�����Ȩ�ޡ�
    }
}
