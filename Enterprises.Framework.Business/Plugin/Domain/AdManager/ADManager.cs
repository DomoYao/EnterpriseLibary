using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using Enterprises.Framework.Plugin.Domain.AdManager.ADObject;
using Enterprises.Framework.Plugin.Domain.AdManager.Enum;
using Enterprises.Framework.Plugin.Domain.AdManager.Exception;

namespace Enterprises.Framework.Plugin.Domain.AdManager
{
    /// <summary>
    /// ���ڽ���AD������Ķ����ṩ����AD�ľ�̬������
    /// </summary>
    /// <remarks>
    /// ����ADsPath������LDAP://DN��LDAP://&lt;GUID&gt;������ʽ����������������ʽ��
    /// һ�㷽�����ṩ2����ʽ��һ�����ò����ṩ���û���ݱ�ʶ��һ������Ĭ�ϵ��û���ݱ�ʶ��
    /// Ĭ���û���ݱ�ʶȡ���ڵ�ǰ���̵��û���ݱ�ʶ��
    /// </remarks>
    public class ADManager
    {
        #region ����Ϣ

        /// <summary>
        /// ���Ѻõ������ƣ�friendly domain name��ת��Ϊ�ϸ�������ƣ�fully qualified domain name����
        /// eg:tb -- > tb.com
        /// </summary>
        /// <param name="friendlyDomainName">�Ѻõ������ƣ�friendly domain name����
        /// �����ǣ�
        /// ��������� DNS ���ơ�
        /// ADAM �������� DNS ���ƺ� LDAP �˿ںţ��� adam_instance.fabrikam.com:389����
        /// ��� DNS ���ƣ��� sales.corp.fabrikam.com��
        /// �ֵ� DNS ���ƣ��� corp.fabrikam.com��
        /// Ӧ�ó�������� DNS ���ơ�
        /// ��������ӵ�����Ĺؼ���֮һ���÷������ӵ������ü��� ADAM ʵ��ע�ᡣ</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns></returns>
        public static string FriendlyDomainToLdapDomain(string friendlyDomainName, string userName, string password)
        {
            string ldapPath;
            try
            {
                DirectoryContext objContext = null;
                if (UseDefaultIdentity(userName, password))
                    objContext = new DirectoryContext(DirectoryContextType.Domain, friendlyDomainName);
                else
                    objContext = new DirectoryContext(DirectoryContextType.Domain, friendlyDomainName, userName, password);

                ldapPath = System.DirectoryServices.ActiveDirectory.Domain.GetDomain(objContext).Name;
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            return ldapPath;
        }

        /// <summary>
        /// ���Ѻõ������ƣ�friendly domain name��ת��Ϊ�ϸ�������ƣ�fully qualified domain name����
        /// eg:tb -- > tb.com
        /// </summary>
        /// <param name="friendlyDomainName">�Ѻõ������ƣ�friendly domain name����
        /// �����ǣ�
        /// ��������� DNS ���ơ�
        /// ADAM �������� DNS ���ƺ� LDAP �˿ںţ��� adam_instance.fabrikam.com:389����
        /// ��� DNS ���ƣ��� sales.corp.fabrikam.com��
        /// �ֵ� DNS ���ƣ��� corp.fabrikam.com��
        /// Ӧ�ó�������� DNS ���ơ�
        /// ��������ӵ�����Ĺؼ���֮һ���÷������ӵ������ü��� ADAM ʵ��ע�ᡣ</param>
        /// <returns></returns>
        public static string FriendlyDomainToLdapDomain(string friendlyDomainName)
        {
            return FriendlyDomainToLdapDomain(friendlyDomainName, null, null);
        }

        /// <summary>
        /// ��ȡ��ǰ�û������ĵ� Forest �����е����������ơ�
        /// </summary>
        /// <returns></returns>
        public static List<string> EnumerateDomains()
        {
            List<string> alDomains = new List<string>();
            Forest currentForest = Forest.GetCurrentForest();
            DomainCollection myDomains = currentForest.Domains;

            foreach (System.DirectoryServices.ActiveDirectory.Domain objDomain in myDomains)
            {
                alDomains.Add(objDomain.Name);
            }
            return alDomains;
        }

        /// <summary>
        /// ��ȡ��ǰ�û������ĵ� Forest ���������е�ȫ��Ŀ¼�� 
        /// </summary>
        /// <returns></returns>
        public static List<string> EnumerateGlobalCatalogs()
        {
            List<string> alGCs = new List<string>();
            Forest currentForest = Forest.GetCurrentForest();
            foreach (GlobalCatalog gc in currentForest.GlobalCatalogs)
            {
                alGCs.Add(gc.Name);
            }
            return alGCs;
        }

        /// <summary>
        /// ��ȡ��ǰ�û���ݱ�ʶ�� Domain �����е����������
        /// </summary>
        /// <returns></returns>
        public static List<string> EnumerateDomainControllers()
        {
            List<string> alDcs = new List<string>();
            System.DirectoryServices.ActiveDirectory.Domain domain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain();
            foreach (DomainController dc in domain.DomainControllers)
            {
                alDcs.Add(dc.Name);
            }
            return alDcs;
        }

        #endregion


        #region Common

        /// <summary>
        /// ����ָ����DirectoryEntry�Ƿ����
        /// </summary>
        /// <param name="path">ADsPath���Զ����LDAP_IDENTITY����ȫת����ġ�</param>
        /// <returns></returns>
        public static bool Exists(string path)
        {
            if (path.StartsWith(ParaMgr.LDAP_IDENTITY))
                return DirectoryEntry.Exists(path);
            else
                return DirectoryEntry.Exists(ParaMgr.LDAP_IDENTITY + path);

        }

        /// <summary>
        /// �ƶ�DirectoryEntry��ָ��λ�á�
        /// </summary>
        /// <param name="objectPath">Ҫ�ƶ���DirectoryEntry��ADsPath��������DN��ʽ����ȫת����ġ�</param>
        /// <param name="newLocationPath">�ƶ�����λ�õ�ADsPath��������DN��ʽ����ȫת����ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <remarks>���ƶ��Ķ����Ҫ�ƶ�����λ�ö������ʹ��DN��ʽ��·������������ʹ��GUID��ʽ��·��������������쳣��</remarks>
        public static void Move(string objectPath, string newLocationPath, string userName, string password)
        {
            if (!Exists(objectPath))
                throw new EntryNotExistException("��Ҫ���ƶ��Ķ��󲻴��ڡ�");

            DirectoryEntry de = null;
            try
            {
                de = GetByPath(objectPath, userName, password);

                Move(de, newLocationPath, userName, password);
            }
            finally
            {
                if (de != null)
                {
                    de.Close();
                    de.Dispose();
                }
            }
        }

        /// <summary>
        /// �ƶ�DirectoryEntry��ָ��λ�ã�ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="objectPath">Ҫ�ƶ���DirectoryEntry��ADsPath��������DN��ʽ����ȫת����ġ�</param>
        /// <param name="newLocationPath">�ƶ�����λ�õ�ADsPath��������DN��ʽ����ȫת����ġ�</param>
        public static void Move(string objectPath, string newLocationPath)
        {
            Move(objectPath, newLocationPath, null, null);
        }

        /// <summary>
        /// �ƶ�DirectoryEntry��ָ��λ�á�
        /// </summary>
        /// <param name="de">Ҫ�ƶ���DirectoryEntry����</param>
        /// <param name="newLocationPath">�ƶ�����λ�õ�ADsPath��������DN��ʽ����ȫת����ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <remarks>���ƶ��Ķ����Ҫ�ƶ�����λ�ö������ʹ��DN��ʽ��·������������ʹ��GUID��ʽ��·��������������쳣��</remarks>
        internal static void Move(DirectoryEntry de, string newLocationPath, string userName, string password)
        {
            if (!Exists(newLocationPath))
                throw new EntryNotExistException("�ƶ�����λ�ö��󲻴��ڡ�");

            DirectoryEntry newLocation = null;
            try
            {
                newLocation = GetByPath(newLocationPath, userName, password);

                string newLocationDN = Utils.EscapeDNBackslashedChar(newLocation.Properties[BaseObject.PROPERTY_DN].Value.ToString());
                string deDN = Utils.EscapeDNBackslashedChar(de.Properties[BaseObject.PROPERTY_DN].Value.ToString());
                if (Exists(Utils.GenerateDN(Utils.GetRDN(deDN), deDN)))
                    throw new SameRDNException("�ƶ�����λ���´���ͬ������");

                de.MoveTo(newLocation);

                de.CommitChanges();
            }
            catch (InvalidOperationException ioe)   // ָ���� DirectoryEntry ����������
            {
                throw new NotContainerException(ioe.Message, ioe);
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (newLocation != null)
                {
                    newLocation.Close();
                    newLocation.Dispose();
                }
            }
        }


        /// <summary>
        /// ��ȡӦ�ó������õ�Ĭ����
        /// </summary>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns></returns>
        public static DirectoryEntry GetAppSetDomain(string userName, string password)
        {
            return GetByDN(ParaMgr.ADFullPath, userName, password);
        }

        /// <summary>
        /// ��ȡӦ�ó������õ�Ĭ����ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <returns></returns>
        public static DirectoryEntry GetAppSetDomain()
        {
            return GetAppSetDomain(null, null);
        }

        // �����û��������룬�ж��Ƿ�Ӧ��ʹ��Ĭ���û���ݱ�ʶ��
        private static bool UseDefaultIdentity(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
                return true;
            else
                return false;
        }


        #endregion


        #region Get & Search

        /// <summary>
        /// ��ȡDirectoryEntry
        /// </summary>
        /// <param name="schema">�Զ���ģʽ</param>
        /// <param name="objectClass">����</param>
        /// <param name="objectCategory">���</param>
        /// <param name="rootDN">������DN��null��ʾ������</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        internal static DirectoryEntry Get(string schema, string objectClass, string objectCategory, string rootDN, string userName, string password)
        {
            DirectoryEntry de = GetByDN((String.IsNullOrEmpty(rootDN) ? (ParaMgr.ADFullPath) : rootDN), 
                userName, password);

            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            if (!String.IsNullOrEmpty(objectClass) ||
                !String.IsNullOrEmpty(objectCategory) ||
                !String.IsNullOrEmpty(schema))
            {
                deSearch.Filter = String.Format("(&{0}{1}{2})",
                    ((!String.IsNullOrEmpty(objectClass)) ? String.Format("(objectClass={0})", objectClass) : ""),
                    ((!String.IsNullOrEmpty(objectCategory)) ? String.Format("(objectCategory={0})", objectCategory) : ""),
                    ((!String.IsNullOrEmpty(schema)) ? String.Format("({0})", schema) : "")
                    );
            }
            deSearch.SearchScope = SearchScope.Subtree;
            SearchResult results = deSearch.FindOne();
            var rde = results != null ? GetByPath(results.Path) : null;

            de.Close();
            de.Dispose();

            return rde;
        }

        /// <summary>
        /// ��ȡDirectoryEntry��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="schema">�Զ���ģʽ</param>
        /// <param name="objectClass">����</param>
        /// <param name="objectCategory">���</param>
        /// <param name="rootDN">������DN��null��ʾ������</param>
        /// <returns>��������ڣ�����null��</returns>
        internal static DirectoryEntry Get(string schema, string objectClass, string objectCategory, string rootDN)
        {
            return Get(schema, objectClass, objectCategory, rootDN, null, null);
        }


        /// <summary>
        /// ����DirectoryEntry
        /// </summary>
        /// <param name="schema">�Զ���ģʽ</param>
        /// <param name="objectClass">����</param>
        /// <param name="objectCategory">���</param>
        /// <param name="rootPath">������ADsPath</param>
        /// <param name="scope">SearchScope</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ����ؿռ��ϡ�</returns>
        internal static List<DirectoryEntry> Search(string schema, string objectClass, string objectCategory,
            string rootPath, SearchScope scope, string userName, string password)
        {
            DirectoryEntry de = null;

            if (!String.IsNullOrEmpty(rootPath))
            {
                de = GetByPath(rootPath, userName, password);
            }
            if (de == null)
                de = GetByPath(ParaMgr.ADFullPath, userName, password);


            DirectorySearcher deSearch = new DirectorySearcher();
            if (de != null)
                deSearch.SearchRoot = de;
            if (!String.IsNullOrEmpty(objectClass) ||
                !String.IsNullOrEmpty(objectCategory) ||
                !String.IsNullOrEmpty(schema))
            {
                deSearch.Filter = String.Format("(&{0}{1}{2})",
                    ((!String.IsNullOrEmpty(objectClass)) ? String.Format("(objectClass={0})", objectClass) : ""),
                    ((!String.IsNullOrEmpty(objectCategory)) ? String.Format("(objectCategory={0})", objectCategory) : ""),
                    ((!String.IsNullOrEmpty(schema)) ? String.Format("({0})", schema) : "")
                    );
            }
            deSearch.SearchScope = scope;
            SearchResultCollection results = deSearch.FindAll();

            List<DirectoryEntry> entries = (from SearchResult se in results select se.GetDirectoryEntry()).ToList();

            if (de != null)
            {
                de.Close();
                de.Dispose();
            }

            return entries;

        }

        /// <summary>
        /// ����DirectoryEntry��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="schema">�Զ���ģʽ</param>
        /// <param name="objectClass">����</param>
        /// <param name="objectCategory">���</param>
        /// <param name="rootPath">������ADsPath</param>
        /// <param name="scope">SearchScope</param>
        /// <returns>��������ڣ����ؿռ��ϡ�</returns>
        internal static List<DirectoryEntry> Search(string schema, string objectClass, string objectCategory, 
            string rootPath, SearchScope scope)
        {
            return Search(schema, objectClass, objectCategory, rootPath, scope, null, null);
        }

        /// <summary>
        /// ����DirectoryEntry�����Ϊ�ַ�����ʽ
        /// </summary>
        /// <param name="schema">�Զ���ģʽ</param>
        /// <param name="objectClass">����</param>
        /// <param name="objectCategory">���</param>
        /// <param name="rootPath">������ADsPath</param>
        /// <param name="scope">SearchScope</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ����ؿռ��ϡ�</returns>
        /// <remarks>����distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass��
        /// ��������sAMAccountName��</remarks>
        internal static List<string[]> Search2(string schema, string objectClass, string objectCategory,
            string rootPath, SearchScope scope, string userName, string password)
        {
            DirectoryEntry de = null;

            if (!String.IsNullOrEmpty(rootPath))
            {
                de = GetByPath(rootPath, userName, password);
            }
            if (de == null)
                de = GetByPath(ParaMgr.ADFullPath, userName, password);


            DirectorySearcher deSearch = new DirectorySearcher();
            if (de != null)
                deSearch.SearchRoot = de;
            if (!String.IsNullOrEmpty(objectClass) ||
                !String.IsNullOrEmpty(objectCategory) ||
                !String.IsNullOrEmpty(schema))
            {
                deSearch.Filter = String.Format("(&{0}{1}{2})",
                    ((!String.IsNullOrEmpty(objectClass)) ? String.Format("(objectClass={0})", objectClass) : ""),
                    ((!String.IsNullOrEmpty(objectCategory)) ? String.Format("(objectCategory={0})", objectCategory) : ""),
                    ((!String.IsNullOrEmpty(schema)) ? String.Format("({0})", schema) : "")
                    );
            }
            deSearch.SearchScope = scope;
            SearchResultCollection results = deSearch.FindAll();

            List<string[]> entriesProperty = new List<string[]>();
            foreach (SearchResult se in results)
            {
                string nativeGuid = "";
                foreach(byte g in (byte[])se.Properties["objectguid"][0])
                {
                    nativeGuid += g.ToString("x2");
                }
                string oc = "";
                foreach (object c in se.Properties["objectclass"])
                {
                    oc = oc + "," + c.ToString();
                }

                string sAmAccountName = null;
                if (se.Properties.Contains("sAMAccountName"))
                    sAmAccountName = se.Properties["sAMAccountName"][0].ToString();


                entriesProperty.Add(new string[] {
                    se.Properties["distinguishedname"][0].ToString(),
                    Utils.ConvertNativeGuidToGuid(nativeGuid).ToString(),
                    se.Properties["name"][0].ToString(),
                    ((se.Properties["description"].Count > 0) ? se.Properties["description"][0].ToString() : null),
                    se.Properties["adspath"][0].ToString(),
                    se.Properties["objectcategory"][0].ToString(),
                    oc.Substring(1),
                    sAmAccountName
                });
            }

            if (de != null)
            {
                de.Close();
                de.Dispose();
            }

            return entriesProperty;

        }

        /// <summary>
        /// ����DirectoryEntry��ʹ��Ĭ���û���ݱ�ʶ�����Ϊ�ַ�����ʽ
        /// </summary>
        /// <param name="schema">�Զ���ģʽ</param>
        /// <param name="objectClass">����</param>
        /// <param name="objectCategory">���</param>
        /// <param name="rootPath">������ADsPath</param>
        /// <param name="scope">SearchScope</param>
        /// <returns>��������ڣ����ؿռ��ϡ�</returns>
        /// <remarks>����distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass</remarks>
        internal static List<string[]> Search2(string schema, string objectClass, string objectCategory,
            string rootPath, SearchScope scope)
        {
            return Search2(schema, objectClass, objectCategory, rootPath, scope, null, null);
        }

        /// <summary>
        /// ����DirectoryEntry
        /// </summary>
        /// <param name="schema">�Զ���ģʽ</param>
        /// <param name="objectClass">����</param>
        /// <param name="objectCategory">���</param>
        /// <param name="rootPath">������ADsPath</param>
        /// <param name="scope">SearchScope</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ����ؿռ��ϡ�</returns>
        internal static SearchResultCollection Search3(string schema, string objectClass, string objectCategory,
            string rootPath, SearchScope scope, string userName, string password)
        {
            DirectoryEntry de = null;

            if (!String.IsNullOrEmpty(rootPath))
            {
                de = GetByPath(rootPath, userName, password);
            }
            if (de == null)
                de = GetByPath(ParaMgr.ADFullPath, userName, password);


            DirectorySearcher deSearch = new DirectorySearcher();
            if (de != null)
                deSearch.SearchRoot = de;
            if (!String.IsNullOrEmpty(objectClass) ||
                !String.IsNullOrEmpty(objectCategory) ||
                !String.IsNullOrEmpty(schema))
            {
                deSearch.Filter = String.Format("(&{0}{1}{2})",
                    ((!String.IsNullOrEmpty(objectClass)) ? String.Format("(objectClass={0})", objectClass) : ""),
                    ((!String.IsNullOrEmpty(objectCategory)) ? String.Format("(objectCategory={0})", objectCategory) : ""),
                    ((!String.IsNullOrEmpty(schema)) ? String.Format("({0})", schema) : "")
                    );
            }
            deSearch.SearchScope = scope;
            
            SearchResultCollection results = deSearch.FindAll();

            de.Close();
            de.Dispose();

            return results;
        }

        /// <summary>
        /// ����DirectoryEntry��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="schema">�Զ���ģʽ</param>
        /// <param name="objectClass">����</param>
        /// <param name="objectCategory">���</param>
        /// <param name="rootPath">������ADsPath</param>
        /// <param name="scope">SearchScope</param>
        /// <returns>��������ڣ����ؿռ��ϡ�</returns>
        internal static SearchResultCollection Search3(string schema, string objectClass, string objectCategory,
            string rootPath, SearchScope scope)
        {
            return Search3(schema, objectClass, objectCategory, rootPath, scope, null, null);
        }

        /// <summary>
        /// ����DirectoryEntry��Guid�õ�DirectoryEntry����
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        internal static DirectoryEntry GetByGuid(Guid guid, string userName, string password)
        {
            return GetByPath(Utils.GenerateADsPath(guid), userName, password);
        }

        /// <summary>
        /// ����DirectoryEntry��Guid�õ�DirectoryEntry����ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>��������ڣ�����null��</returns>
        internal static DirectoryEntry GetByGuid(Guid guid)
        {
            return GetByGuid(guid, null,null );
        }


        /// <summary>
        /// ����DirectoryEntry��SID�õ�DirectoryEntry����
        /// </summary>
        /// <param name="sid">objectSID</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        internal static DirectoryEntry GetBySid(string sid, string userName, string password)
        {
            return Get("objectSID=" + sid, null, null, null, userName, password);
        }

        /// <summary>
        /// ����DirectoryEntry��SID�õ�DirectoryEntry����ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="sid">objectSID</param>
        /// <returns>��������ڣ�����null��</returns>
        internal static DirectoryEntry GetBySid(string sid)
        {
            return GetBySid(sid, null, null);
        }


        /// <summary>
        /// ����DirectoryEntry��DN�õ�DirectoryEntry����
        /// </summary>
        /// <param name="dn">DN����ȫת����ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        internal static DirectoryEntry GetByDN(string dn, string userName, string password)
        {
            return GetByPath(ParaMgr.LDAP_IDENTITY + dn, userName, password);
        }

        /// <summary>
        /// ����DirectoryEntry��DN�õ�DirectoryEntry����ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="dn">DN����ȫת����ġ�</param>
        /// <returns>��������ڣ�����null��</returns>
        internal static DirectoryEntry GetByDN(string dn)
        {
            return GetByDN(dn, null, null);
        }


        /// <summary>
        /// ����DirectoryEntry��ADsPath�õ�DirectoryEntry����
        /// </summary>
        /// <param name="path">������ADsPath���Զ����LDAP_IDENTITY����ȫת����ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        /// <returns></returns>
        internal static DirectoryEntry GetByPath(string path, string userName, string password)
        {
            if (Exists(path))
            {
                if (UseDefaultIdentity(userName, password))
                    return new DirectoryEntry((path.StartsWith(ParaMgr.LDAP_IDENTITY)) ? path : (ParaMgr.LDAP_IDENTITY + path));
                return new DirectoryEntry(
                    (path.StartsWith(ParaMgr.LDAP_IDENTITY)) ? path : (ParaMgr.LDAP_IDENTITY + path),
                    userName,
                    password,
                    AuthenticationTypes.Secure);
            }

            return null;
        }

        /// <summary>
        /// ����DirectoryEntry��ADsPath�õ�DirectoryEntry����ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="path">������ADsPath����ȫת����ġ�</param>
        /// <returns>��������ڣ�����null��</returns>
        /// <returns></returns>
        internal static DirectoryEntry GetByPath(string path)
        {
            return GetByPath(path, null, null);
        }


        #endregion


        #region User

        #region Search

        /// <summary>
        /// ��ȡָ�������û���
        /// </summary>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static List<User> GetUserAll(string rootPath, string userName, string password)
        {
            List<DirectoryEntry> entries = Search(null, "user", "person", rootPath, SearchScope.Subtree, userName, password);
            List<User> users = new List<User>();
            foreach (DirectoryEntry de in entries)
            {
                users.Add(new User(de));

                de.Close();
                de.Dispose();
            }

            return users;
        }

        /// <summary>
        /// ��ȡָ�������û���ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <returns>��������ڣ�����null��</returns>
        public static List<User> GetUserAll(string rootPath)
        {
            return GetUserAll(rootPath, null, null);
        }

        /// <summary>
        /// ��ȡָ�������û���
        /// </summary>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        /// <remarks>����distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass��
        /// ��������sAMAccountName��</remarks>
        public static List<string[]> GetUserAllSimple(string rootPath, string userName, string password)
        {
            return Search2(null, "user", "person", rootPath, SearchScope.Subtree, userName, password);
        }

        /// <summary>
        /// ��ȡָ�������û���ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <returns>��������ڣ�����null��</returns>
        /// <remarks>����distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass</remarks>
        public static List<string[]> GetUserAllSimple(string rootPath)
        {
            return GetUserAllSimple(rootPath, null, null);
        }

        /// <summary>
        /// ��ȡָ�������û���ֱ�ӽ�����ѯ������ٶȽ�GetUserAll�졣
        /// </summary>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static List<User> GetUserAllQuick(string rootPath, string userName, string password)
        {
            SearchResultCollection results = Search3(null, "user", "person", rootPath, SearchScope.Subtree, userName, password);

            List<User> users = new List<User>();
            foreach (SearchResult se in results)
            {
                users.Add(new User(se));
            }

            return users;
        }

        /// <summary>
        /// ��ȡָ�������û���ʹ��Ĭ���û���ݱ�ʶ��ֱ�ӽ�����ѯ������ٶȽ�GetUserAll�졣
        /// </summary>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <returns>��������ڣ�����null��</returns>
        public static List<User> GetUserAllQuick(string rootPath)
        {
            return GetUserAllQuick(rootPath, null, null);
        }

        /// <summary>
        /// ����userPrincipalName��ȡGroup��
        /// </summary>
        /// <param name="userPrincipalName">userPrincipalName��</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static User GetUserByPrincipalName(string userPrincipalName, string userName, string password)
        {
            List<DirectoryEntry> entries = Search("userPrincipalName=" + Utils.Escape4Query(userPrincipalName), 
                "user", "person", null, SearchScope.Subtree, userName, password);
            if (entries.Count == 1)
            {
                DirectoryEntry de = entries[0];

                User user = new User(de);

                de.Close();
                de.Dispose();

                return user;
            }

            return null;
        }

        /// <summary>
        /// ����sAMAccountName��ȡUser��
        /// </summary>
        /// <param name="sAmAccountName">sAMAccountName��</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static User GetUserBySamAccountName(string sAmAccountName, string userName, string password)
        {
            List<DirectoryEntry> entries = Search("sAMAccountName=" + Utils.Escape4Query(sAmAccountName), 
                "user", "person", null, SearchScope.Subtree, userName, password);
            if (entries.Count == 1)
            {
                DirectoryEntry de = entries[0];

                User user = new User(de);

                de.Close();
                de.Dispose();

                return user;
            }

            return null;
        }
        #endregion

        #region Get

        /// <summary>
        /// �����û���Guid�õ��û�����
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static User GetUserByGuid(Guid guid, string userName, string password)
        {
            return GetUserByPath(Utils.GenerateADsPath(guid), userName, password);
        }

        /// <summary>
        /// �����û���Guid�õ��û�����ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>��������ڣ�����null��</returns>
        public static User GetUserByGuid(Guid guid)
        {
            return GetUserByGuid(guid, null, null);
        }

        /// <summary>
        /// �����û���DN�õ��û�����
        /// </summary>
        /// <param name="dn">DN����ȫת����ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static User GetUserByDN(string dn, string userName, string password)
        {
            return GetUserByPath(dn, userName, password);    
        }

        /// <summary>
        /// �����û���DN�õ��û�����ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="dn">DN����ȫת����ġ�</param>
        /// <returns>��������ڣ�����null��</returns>
        public static User GetUserByDN(string dn)
        {
            return GetUserByDN(dn, null, null);
        }

        /// <summary>
        /// �����û���ADsPath�õ��û�����
        /// </summary>
        /// <param name="path">ADsPath����ȫת����ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static User GetUserByPath(string path, string userName, string password)
        {
            DirectoryEntry entry = GetByPath(path, userName, password);
            if (entry != null)
            {
                User user = new User(entry);
                entry.Close();
                entry.Dispose();

                return user;
            }
            return null;
        }

        /// <summary>
        /// �����û���ADsPath�õ��û�����ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="path">ADsPath����ȫת����ġ�</param>
        /// <returns>��������ڣ�����null��</returns>
        public static User GetUserByPath(string path)
        {
            return GetUserByPath(path, null, null);
        }

        #endregion

        #region Password

        /// <summary>
        /// �����û����롣
        /// </summary>
        /// <param name="guid">�û�DirectoryEntry��Guid��</param>
        /// <param name="newPassword">�����롣</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public static void SetUserPassword(Guid guid, string newPassword, string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = GetByGuid(guid, userName, password);

                if (de == null)
                    throw new EntryNotExistException("�û����󲻴��ڡ�");

                if (de.SchemaClassName != SchemaClass.user.ToString("F"))
                    throw new SchemaClassException("�������Ͳ���" + SchemaClass.user.ToString("F") + "��");

                de.Invoke("SetPassword", new object[] { newPassword });

                de.CommitChanges();
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (de != null)
                {
                    de.Close();
                    de.Dispose();
                }
            }
        }

        /// <summary>
        /// �����û����룬ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="guid">�û�DirectoryEntry��Guid��</param>
        /// <param name="newPassword">�����롣</param>
        public static void SetUserPassword(Guid guid, string newPassword)
        {
            SetUserPassword(guid, newPassword, null, null);
        }

        #endregion

        #region Move

        /// <summary>
        /// �ƶ��û�DirectoryEntry��ָ��λ�á�
        /// </summary>
        /// <param name="userPath">Ҫ�ƶ����û�DirectoryEntry��ADsPath��������DN��ʽ����ȫת����ġ�</param>
        /// <param name="newLocationPath">�ƶ�����λ�õ�ADsPath��������DN��ʽ����ȫת����ġ�</param>
        /// <param name="mustOu">�ƶ�����λ�ö�Ӧ��DirectoryEntry�Ƿ��������֯��λ��</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public static void MoveUser(string userPath, string newLocationPath, bool mustOu, string userName, string password)
        {
            if (!Exists(userPath))
                throw new EntryNotExistException("��Ҫ���ƶ��Ķ��󲻴��ڡ�");

            DirectoryEntry de = null;
            try
            {
                de = GetByPath(userPath, userName, password);

                MoveUser(de, newLocationPath, mustOu, userName, password);
            }
            finally
            {
                if (de != null)
                {
                    de.Close();
                    de.Dispose();
                }
            }
        }

        /// <summary>
        /// �ƶ��û�DirectoryEntry��ָ��λ�ã�ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="userPath">Ҫ�ƶ����û�DirectoryEntry��ADsPath��������DN��ʽ����ȫת����ġ�</param>
        /// <param name="newLocationPath">�ƶ�����λ�õ�ADsPath��������DN��ʽ����ȫת����ġ�</param>
        /// <param name="mustOu">�ƶ�����λ�ö�Ӧ��DirectoryEntry�Ƿ��������֯��λ��</param>
        public static void MoveUser(string userPath, string newLocationPath, bool mustOu)
        {
            MoveUser(userPath, newLocationPath, mustOu, null, null);
        }

        /// <summary>
        /// �ƶ��û�DirectoryEntry��ָ��λ�á�
        /// </summary>
        /// <param name="user">Ҫ�ƶ����û�DirectoryEntry��Guid</param>
        /// <param name="newLocation">�ƶ�����λ�õ�Guid</param>
        /// <param name="mustOu">�ƶ�����λ�ö�Ӧ��DirectoryEntry�Ƿ��������֯��λ��</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public static void MoveUser(Guid user, Guid newLocation, bool mustOu, string userName, string password)
        {
            MoveUser(GetUserByGuid(user).Dn, GetOUByGuid(newLocation).Dn, mustOu, userName, password);
        }

        /// <summary>
        /// �ƶ��û�DirectoryEntry��ָ��λ�ã�ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="user">Ҫ�ƶ����û�DirectoryEntry��Guid</param>
        /// <param name="newLocation">�ƶ�����λ�õ�Guid</param>
        /// <param name="mustOu">�ƶ�����λ�ö�Ӧ��DirectoryEntry�Ƿ��������֯��λ��</param>
        public static void MoveUser(Guid user, Guid newLocation, bool mustOu)
        {
            MoveUser(GetUserByGuid(user).Dn, GetOUByGuid(newLocation).Dn, mustOu, null, null);
        }

        /// <summary>
        /// �ƶ��û�DirectoryEntry��ָ��λ�á�
        /// </summary>
        /// <param name="de">Ҫ�ƶ����û�DirectoryEntry���󡣱�����ͨ��DN��ʽ·���õ��Ķ���</param>
        /// <param name="newLocationPath">�ƶ�����λ�õ�ADsPath��������DN��ʽ����ȫת����ġ�</param>
        /// <param name="mustOu">�ƶ�����λ�ö�Ӧ��DirectoryEntry�Ƿ��������֯��λ��</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        internal static void MoveUser(DirectoryEntry de, string newLocationPath, bool mustOu, string userName, string password)
        {
            if (!Exists(newLocationPath))
                throw new EntryNotExistException("�ƶ�����λ�ö��󲻴��ڡ�");

            DirectoryEntry newLocation = null;
            try
            {
                newLocation = GetByPath(newLocationPath, userName, password);

                if (de.SchemaClassName != SchemaClass.user.ToString("F"))
                    throw new SchemaClassException("��Ҫ���ƶ��Ķ������Ͳ���" + SchemaClass.user.ToString("F") + "��");

                if (mustOu && newLocation.SchemaClassName != SchemaClass.organizationalUnit.ToString("F"))
                    throw new SchemaClassException("�ƶ�����λ�ö������Ͳ���" + SchemaClass.organizationalUnit.ToString("F") + "��");

                if (Exists(Utils.GetRDNValue(de.Properties[BaseObject.PROPERTY_DN].Value.ToString()) + "," +
                    newLocation.Properties[BaseObject.PROPERTY_DN].Value.ToString()))
                    throw new SameRDNException("�ƶ�����λ���´���ͬ������");

                de.MoveTo(newLocation);
                de.CommitChanges();
            }
            catch (InvalidOperationException ioe)   // ָ���� DirectoryEntry ����������
            {
                throw new NotContainerException(ioe.Message, ioe);
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (newLocation != null)
                {
                    newLocation.Close();
                    newLocation.Dispose();
                }
            }
        }

        #endregion

        #region MemberOf

        /// <summary>
        /// ��ȡ�û�DirectoryEntry�����PrimaryGroup DirectoryEntry����
        /// </summary>
        /// <param name="userPath">�û�DirectoryEntry��ADsPath��</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>�����ڷ���null��</returns>
        public static DirectoryEntry GetUserPrimaryGroup(string userPath, string userName, string password)
        {
            DirectoryEntry de = GetByPath(userPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("�û����󲻴��ڡ�");

            if (de.SchemaClassName != SchemaClass.user.ToString("F"))
                throw new SchemaClassException("�������Ͳ���" + SchemaClass.user.ToString("F") + "��");

            return GetUserPrimaryGroup(de, userName, password);
        }

        /// <summary>
        /// ��ȡ�û�DirectoryEntry�����PrimaryGroup DirectoryEntry����ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="userPath">�û�DirectoryEntry��ADsPath��</param>
        /// <returns>�����ڷ���null��</returns>
        public static DirectoryEntry GetUserPrimaryGroup(string userPath)
        {
            return GetUserPrimaryGroup(userPath, null, null);
        }

        /// <summary>
        /// ��ȡ�û�DirectoryEntry�����PrimaryGroup DirectoryEntry����
        /// </summary>
        /// <param name="user">�û�DirectoryEntry����</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>�����ڷ���null��</returns>
        internal static DirectoryEntry GetUserPrimaryGroup(DirectoryEntry user, string userName, string password)
        {
            string primaryGroupSID = User.GeneratePrimaryGroupSID((byte[])(user.Properties[BaseObject.PROPERTY_OBJECTSID].Value),
                Convert.ToInt32(user.Properties[User.PROPERTY_MEMBEROF_PRIMARY].Value));

            return GetBySid(primaryGroupSID, userName, password);
        }

        /// <summary>
        /// ��ȡ�û�DirectoryEntry������������DN��
        /// </summary>
        /// <param name="userPath">�û�DirectoryEntry��ADsPath��</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <param name="includePrimaryGroup">�Ƿ����PrimaryGroup</param>
        /// <returns>�����ڷ��ؿռ��ϡ�</returns>
        public static List<string> GetUserMemberOfDN(string userPath, string userName, string password, bool includePrimaryGroup)
        {
            DirectoryEntry de = GetByPath(userPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("�û����󲻴��ڡ�");

            if (de.SchemaClassName != SchemaClass.user.ToString("F"))
                throw new SchemaClassException("�������Ͳ���" + SchemaClass.user.ToString("F") + "��");

            List<string> dn = new List<string>();

            if (includePrimaryGroup)
            {
                DirectoryEntry primary = GetUserPrimaryGroup(de, userName, password);
                if (primary != null)
                {
                    dn.Add(Utils.EscapeDNBackslashedChar(primary.Properties[BaseObject.PROPERTY_DN].Value.ToString()));

                    primary.Close();
                    primary.Dispose();
                }
            }
            if (de.Properties.Contains(User.PROPERTY_MEMBEROF_ALL))
            {
                foreach (object m in de.Properties[User.PROPERTY_MEMBEROF_ALL])
                {
                    dn.Add(Utils.EscapeDNBackslashedChar(m.ToString()));        // ת��/
                }
            }

            de.Close();
            de.Dispose();

            return dn;
        }

        /// <summary>
        /// ��ȡ�û�DirectoryEntry������������DN��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="userPath">�û�DirectoryEntry��ADsPath��</param>
        /// <param name="includePrimaryGroup">�Ƿ����PrimaryGroup</param>
        /// <returns>�����ڷ��ؿռ��ϡ�</returns>
        public static List<string> GetUserMemberOfDN(string userPath, bool includePrimaryGroup)
        {
            return GetUserMemberOfDN(userPath, null, null, includePrimaryGroup);
        }

        #endregion

        #endregion


        #region Group

        #region Search

        /// <summary>
        /// ��ȡָ�������顣
        /// </summary>
        /// <param name="cn">��CN��</param>
        /// <param name="description">��������</param>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static List<Group> SearchGroup(string cn, string description, string rootPath, string userName, string password)
        {
            string schema = null;
            if (!String.IsNullOrEmpty(cn) || !String.IsNullOrEmpty(description))
                schema = String.Format("(&{0}{1})", 
                    (!String.IsNullOrEmpty(cn) ? String.Format("(cn=*{0}*)", Utils.Escape4Query(cn)) : "" ),
                    (!String.IsNullOrEmpty(description) ? String.Format("(description=*{0}*)", Utils.Escape4Query(description)) : ""));

            List<DirectoryEntry> entries = Search(schema, "group", null, rootPath, SearchScope.Subtree, userName, password);
            List<Group> groups = new List<Group>();
            foreach (DirectoryEntry de in entries)
            {
                groups.Add(new Group(de));

                de.Close();
                de.Dispose();
            }

            return groups;
        }

        /// <summary>
        /// ��ȡָ�������飬ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="cn">��CN��</param>
        /// <param name="description">��������</param>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <returns>��������ڣ�����null��</returns>
        public static List<Group> SearchGroup(string cn, string description, string rootPath)
        {
            return SearchGroup(cn, description, rootPath, null, null);
        }

        /// <summary>
        /// ��ȡָ�������顣
        /// </summary>
        /// <param name="cn">��CN��</param>
        /// <param name="description">��������</param>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        /// <remarks>����distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass��
        /// ��������sAMAccountName��</remarks>
        public static List<String[]> SearchGroupSimple(string cn, string description, string rootPath, string userName, string password)
        {
            string schema = null;
            if (!String.IsNullOrEmpty(cn) || !String.IsNullOrEmpty(description))
                schema = String.Format("&{0}{1}",
                    (!String.IsNullOrEmpty(cn) ? String.Format("(cn=*{0}*)", Utils.Escape4Query(cn)) : ""),
                    (!String.IsNullOrEmpty(description) ? String.Format("(cn=*{0}*)", Utils.Escape4Query(description)) : ""));

            return Search2(schema, "group", null, rootPath, SearchScope.Subtree, userName, password);
        }

        /// <summary>
        /// ��ȡָ�������飬ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="cn">��CN��</param>
        /// <param name="description">��������</param>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <returns>��������ڣ�����null��</returns>
        /// <remarks>����distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass</remarks>
        public static List<String[]> SearchGroupSimple(string cn, string description, string rootPath)
        {
            return SearchGroupSimple(cn, description, rootPath, null, null);
        }

        /// <summary>
        /// ��ȡָ�������顣ֱ�ӽ�����ѯ������ٶȽ�SearchGroup�졣
        /// </summary>
        /// <param name="cn">��CN��</param>
        /// <param name="description">��������</param>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static List<Group> SearchGroupQuick(string cn, string description, string rootPath, string userName, string password)
        {
            string schema = null;
            if (!String.IsNullOrEmpty(cn) || !String.IsNullOrEmpty(description))
                schema = String.Format("&{0}{1}",
                    (!String.IsNullOrEmpty(cn) ? String.Format("(cn=*{0}*)", Utils.Escape4Query(cn)) : ""),
                    (!String.IsNullOrEmpty(description) ? String.Format("(cn=*{0}*)", Utils.Escape4Query(description)) : ""));

            SearchResultCollection results = Search3(schema, "group", null, rootPath, SearchScope.Subtree, userName, password);

            List<Group> groups = new List<Group>();
            foreach (SearchResult se in results)
            {
                groups.Add(new Group(se));
            }

            return groups;
        }

        /// <summary>
        /// ��ȡָ�������飬ʹ��Ĭ���û���ݱ�ʶ��ֱ�ӽ�����ѯ������ٶȽ�SearchGroup�졣
        /// </summary>
        /// <param name="cn">��CN��</param>
        /// <param name="description">��������</param>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <returns>��������ڣ�����null��</returns>
        public static List<Group> SearchGroupQuick(string cn, string description, string rootPath)
        {
            return SearchGroupQuick(null,null, rootPath, null, null);
        }

        /// <summary>
        /// ����sAMAccountName��ȡGroup��
        /// </summary>
        /// <param name="sAmAccountName">sAMAccountName��</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static Group GetGroupBySamAccountName(string sAmAccountName, string userName, string password)
        {
            List<DirectoryEntry> entries = Search("sAMAccountName=" + Utils.Escape4Query(sAmAccountName), 
                "group", null, null, SearchScope.Subtree, userName, password);
            if (entries.Count == 1)
            {
                DirectoryEntry de = entries[0];

                Group group = new Group(de);

                de.Close();
                de.Dispose();

                return group;
            }

            return null;
        }

        #endregion

        #region Get

        /// <summary>
        /// �����û���Guid�õ������
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static Group GetGroupByGuid(Guid guid, string userName, string password)
        {
            return GetGroupByPath(Utils.GenerateADsPath(guid), userName, password);

        }

        /// <summary>
        /// �����û���Guid�õ������ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>��������ڣ�����null��</returns>
        public static Group GetGroupByGuid(Guid guid)
        {
            return GetGroupByGuid(guid, null,null);
        }

        /// <summary>
        /// �����û���DN�õ��û��顣
        /// </summary>
        /// <param name="dn">DN����ȫת����ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static Group GetGroupByDN(string dn, string userName, string password)
        {
            return GetGroupByPath(dn, userName, password);
        }

        /// <summary>
        /// �����û���DN�õ������ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="dn">DN����ȫת����ġ�</param>
        /// <returns>��������ڣ�����null��</returns>
        public static Group GetGroupByDN(string dn)
        {
            return GetGroupByDN(dn, null, null);
        }

        /// <summary>
        /// �����û���ADsPath�õ������
        /// </summary>
        /// <param name="path">ADsPath����ȫת����ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static Group GetGroupByPath(string path, string userName, string password)
        {
            DirectoryEntry entry = GetByPath(path, userName, password);
            if (entry != null)
            {
                Group group = new Group(entry);
                entry.Close();
                entry.Dispose();

                return group;
            }
            else
                return null;

            
        }

        /// <summary>
        /// �����û���ADsPath�õ������ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="path">ADsPath����ȫת����ġ�</param>
        /// <returns>��������ڣ�����null��</returns>
        public static Group GetGroupByPath(string path)
        {
            return GetGroupByPath(path, null, null);
        }

        #endregion

        #region Rename

        /// <summary>
        /// ������DirectoryEntry��������ơ�
        /// </summary>
        /// <param name="groupPath">��DirectoryEntry��ADsPath</param>
        /// <param name="newName">����������ơ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public static void RenameGroup(string groupPath, string newName, string userName, string password)
        {
            DirectoryEntry de = GetByPath(groupPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("����󲻴��ڡ�");

            if (de.SchemaClassName != SchemaClass.group.ToString("F"))
                throw new SchemaClassException("�������Ͳ���" + SchemaClass.group.ToString("F") + "��");

            string dn = Utils.EscapeDNBackslashedChar(de.Properties[BaseObject.PROPERTY_DN].Value.ToString());
            string rdn = Utils.GenerateRDNCN(newName);
            if(Exists(Utils.GenerateDN(rdn, Utils.GetParentDN(dn))))
                throw new SameRDNException("�Ѵ���ͬ������");
            try
            {
                de.Rename(rdn);

                de.CommitChanges();
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (de != null)
                {
                    de.Close();
                    de.Dispose();
                }
            }
        }

        /// <summary>
        /// ������DirectoryEntry��������ƣ�ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="groupPath">��DirectoryEntry��ADsPath</param>
        /// <param name="newName">����������ơ�</param>
        public static void RenameGroup(string groupPath, string newName)
        {
            RenameGroup(groupPath, newName);
        }

        #endregion

        #region Member Change

        /// <summary>
        /// ���û���ӵ��顣
        /// </summary>
        /// <param name="groupPath">��DirectoryEntry��ADsPath����ȫת��ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <param name="userDN">��Ҫ��ӵ��û���DN����ȫת��ġ�</param>
        public static void AddUserToGroup(string groupPath, string userName, string password, params string[] userDN)
        {
            DirectoryEntry de = GetByPath(groupPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("����󲻴��ڡ�");

            if (de.SchemaClassName != SchemaClass.group.ToString("F"))
                throw new SchemaClassException("�������Ͳ���" + SchemaClass.group.ToString("F") + "��");

            // �õ����е�Member
            List<string> ms = new List<string>();
            foreach (object m in de.Properties[Group.PROPERTY_MEMBER])
            {
                ms.Add(Utils.EscapeDNBackslashedChar(m.ToString()));
            }
            ms.Sort();          // ������ -- �Ա��ڲ�ʹ��

            List<string> toAdd = new List<string>();
            foreach (string udn in userDN)
            {
                if (!(ms.BinarySearch(udn) >= 0))
                {
                    if (!toAdd.Exists(delegate(string a ) {return a == udn;}))
                        toAdd.Add(udn);
                }
            }

            try
            {
                foreach (string udn in toAdd)
                {
                    de.Invoke("Add", new object[] { ParaMgr.LDAP_IDENTITY + udn });         // ��ҪADsPath
                }

                de.CommitChanges();
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (de != null)
                {
                    de.Close();
                    de.Dispose();
                }
            }
        }

        /// <summary>
        /// ���û���ӵ��飬ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="groupPath">��DirectoryEntry��ADsPath����ȫת��ġ�</param>
        /// <param name="userDN">��Ҫ��ӵ��û���DN��</param>
        public static void AddUserToGroup(string groupPath, params string[] userDN)
        {
            AddUserToGroup(groupPath, null,null,userDN);
        }

        /// <summary>
        /// ���û���ӵ��顣
        /// </summary>
        /// <param name="groupPath">��DirectoryEntry��ADsPath����ȫת��ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <param name="userGuid">��Ҫ��ӵ��û���Guid��</param>
        public static void AddUserToGroup(string groupPath, string userName, string password, params Guid[] userGuid)
        {
            List<string> userDN = new List<string>();
            User user = null;
            foreach(Guid guid in userGuid)
            {
                user = GetUserByGuid(guid);
                if (user != null)
                {
                    userDN.Add(user.Dn);
                }
            }

            AddUserToGroup(groupPath, userName, password, userDN.ToArray());
        }

        /// <summary>
        /// ���û���ӵ��飬ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="groupPath">��DirectoryEntry��ADsPath����ȫת��ġ�</param>
        /// <param name="userGuid">��Ҫ��ӵ��û���Guid��</param>
        public static void AddUserToGroup(string groupPath, params Guid[] userGuid)
        {
            AddUserToGroup(groupPath, null, null, userGuid);
        }

        /// <summary>
        /// ���û��������Ƴ���
        /// </summary>
        /// <param name="groupPath">��DirectoryEntry��ADsPath����ȫת��ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <param name="userDN">��Ҫ�Ƴ����û���DN����ȫת��ġ�</param>
        public static void RemoveUserFromGroup(string groupPath, string userName, string password, params string[] userDN)
        {
            DirectoryEntry de = GetByPath(groupPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("����󲻴��ڡ�");

            if (de.SchemaClassName != SchemaClass.group.ToString("F"))
                throw new SchemaClassException("�������Ͳ���" + SchemaClass.group.ToString("F") + "��");

            // �õ����е�Group
            List<string> ms = new List<string>();
            foreach (object m in de.Properties[Group.PROPERTY_MEMBER])
            {
                ms.Add(Utils.EscapeDNBackslashedChar(m.ToString()));
            }
            ms.Sort();          // ������ -- �Ա��ڲ�ʹ��

            List<string> toRemove = new List<string>();
            foreach (string udn in userDN)
            {
                if (ms.BinarySearch(udn) >= 0)
                {
                    if (!toRemove.Exists(delegate(string a) { return a == udn; }))
                        toRemove.Add(udn);
                }
            }

            try
            {
                foreach (string udn in toRemove)
                {
                    de.Invoke("Remove", new object[] { ParaMgr.LDAP_IDENTITY + udn });         // ��ҪADsPath
                }

                //de.Invoke("Remove", userDN);        // TODO:�Ƿ���Ҫ����ת���/���Ƿ���ҪADsPath��like AddUserToGroup

                de.CommitChanges();
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (de != null)
                {
                    de.Close();
                    de.Dispose();
                }
            }
        }

        /// <summary>
        /// ���û��������Ƴ���ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="groupPath">��DirectoryEntry��ADsPath����ȫת��ġ�</param>
        /// <param name="userDN">��Ҫ�Ƴ����û���DN��</param>        
        public static void RemoveUserFromGroup(string groupPath, params string[] userDN)
        {
            RemoveUserFromGroup(groupPath, null,null,userDN);
        }

        /// <summary>
        /// ���û��������Ƴ���
        /// </summary>
        /// <param name="groupPath">��DirectoryEntry��ADsPath����ȫת��ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <param name="userGuid">��Ҫ�Ƴ����û���Guid��</param>
        public static void RemoveUserFromGroup(string groupPath, string userName, string password, params Guid[] userGuid)
        {
            List<string> userDN = new List<string>();
            User user = null;
            foreach(Guid guid in userGuid)
            {
                user = GetUserByGuid(guid);
                if (user != null)
                {
                    userDN.Add(user.Dn);
                }
            }

            RemoveUserFromGroup(groupPath, userName, password, userDN.ToArray());
        }

        /// <summary>
        /// ���û��������Ƴ���ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="groupPath">��DirectoryEntry��ADsPath����ȫת��ġ�</param>
        /// <param name="userGuid">��Ҫ�Ƴ����û���Guid��</param>
        public static void RemoveUserFromGroup(string groupPath, params Guid[] userGuid)
        {
            RemoveUserFromGroup(groupPath, null, null, userGuid);
        }

        #endregion

        #region MemberOf & Member

        /// <summary>
        /// ��ȡ����������DN
        /// </summary>
        /// <param name="groupPath">��DirectoryEntry��ADsPath����ȫת��ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns></returns>
        public static List<string> GetGroupMemberOfDN(string groupPath, string userName, string password)
        {
            DirectoryEntry de = GetByPath(groupPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("����󲻴��ڡ�");

            if (de.SchemaClassName != SchemaClass.group.ToString("F"))
                throw new SchemaClassException("�������Ͳ���" + SchemaClass.group.ToString("F") + "��");

            List<string> dn = new List<string>();
            if (de.Properties.Contains(Group.PROPERTY_MEMBEROF))
            {
                foreach (object m in de.Properties[Group.PROPERTY_MEMBEROF])
                {
                    dn.Add(Utils.EscapeDNBackslashedChar(m.ToString()));
                }
            }

            de.Close();
            de.Dispose();

            return dn;
        }

        /// <summary>
        /// ��ȡ����������DN��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="groupPath">��DirectoryEntry��ADsPath����ȫת��ġ�</param>
        /// <returns></returns>
        public static List<string> GetGroupMemberOfDN(string groupPath)
        {
            return GetGroupMemberOfDN(groupPath, null, null);
        }

        /// <summary>
        /// ��ȡ��ĳ�Ա�����û���
        /// </summary>
        /// <param name="groupPath">��DirectoryEntry��ADsPath����ȫת��ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns></returns>
        public static List<User> GetGroupUserMember(string groupPath, string userName, string password)
        {
            DirectoryEntry de = GetByPath(groupPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("����󲻴��ڡ�");

            if (de.SchemaClassName != SchemaClass.group.ToString("F"))
                throw new SchemaClassException("�������Ͳ���" + SchemaClass.group.ToString("F") + "��");

            List<User> users = new List<User>();
            string userSchemaClassName = SchemaClass.user.ToString("F");

            if (de.Properties.Contains(Group.PROPERTY_MEMBER))
            {
                foreach (object memberDN in de.Properties[Group.PROPERTY_MEMBER])
                {
                    de = GetByDN(Utils.EscapeDNBackslashedChar(memberDN.ToString()), userName, password);

                    if (de != null)
                    {
                        if (de.SchemaClassName == userSchemaClassName)
                        {
                            users.Add(new User(de));
                        }

                        de.Close();
                        de.Dispose();
                    }
                }
            }

            return users;
        }

        /// <summary>
        /// ��ȡ��ĳ�Ա�����û�����ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="groupPath">��DirectoryEntry��ADsPath����ȫת��ġ�</param>
        /// <returns></returns>
        public static List<User> GetGroupUserMember(string groupPath)
        {
            return GetGroupUserMember(groupPath, null, null);
        }

        #endregion

        #endregion


        #region OU

        #region Search

        /// <summary>
        /// ��ȡָ��������֯��λ��
        /// </summary>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static List<OU> GetOUAll(string rootPath, string userName, string password)
        {
            List<DirectoryEntry> entries = Search(null, "organizationalUnit", null, rootPath, SearchScope.Subtree, userName, password);
            List<OU> ous = new List<OU>();
            foreach (DirectoryEntry de in entries)
            {
                ous.Add(new OU(de));

                de.Close();
                de.Dispose();
            }

            return ous;
        }

        /// <summary>
        /// ��ȡָ��������֯��λ��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <returns>��������ڣ�����null��</returns>
        public static List<OU> GetOUAll(string rootPath)
        {
            return GetOUAll(rootPath, null, null);
        }

        /// <summary>
        /// ��ȡָ��������֯��λ��
        /// </summary>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        /// <remarks>����distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass</remarks>
        public static List<String[]> GetOUAllSimple(string rootPath, string userName, string password)
        {
            return Search2(null, "organizationalUnit", null, rootPath, SearchScope.Subtree, userName, password);
        }

        /// <summary>
        /// ��ȡָ��������֯��λ��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <returns>��������ڣ�����null��</returns>
        /// <remarks>����distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass</remarks>
        public static List<String[]> GetOUAllSimple(string rootPath)
        {
            return GetOUAllSimple(rootPath, null, null);
        }

        /// <summary>
        /// ��ȡָ��������֯��λ��ֱ�ӽ�����ѯ������ٶȽ�GetUserAll�졣
        /// </summary>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static List<OU> GetOUAllQuick(string rootPath, string userName, string password)
        {
            SearchResultCollection results = Search3(null, "organizationalUnit", null, rootPath, SearchScope.Subtree, userName, password);

            List<OU> ous = new List<OU>();
            foreach (SearchResult se in results)
            {
                ous.Add(new OU(se));
            }

            return ous;
        }

        /// <summary>
        /// ��ȡָ��������֯��λ��ʹ��Ĭ���û���ݱ�ʶ��ֱ�ӽ�����ѯ������ٶȽ�GetUserAll�졣
        /// </summary>
        /// <param name="rootPath">������ADsPath��null��ʾ������</param>
        /// <returns>��������ڣ�����null��</returns>
        public static List<OU> GetOUAllQuick(string rootPath)
        {
            return GetOUAllQuick(rootPath, null, null);
        }

        #endregion

        #region Get

        /// <summary>
        /// ������֯��λ��Guid�õ���֯��λ����
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static OU GetOUByGuid(Guid guid, string userName, string password)
        {
            return GetOUByPath(Utils.GenerateADsPath(guid), userName, password);
        }

        /// <summary>
        /// ������֯��λ��Guid�õ���֯��λ����ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>��������ڣ�����null��</returns>
        public static OU GetOUByGuid(Guid guid)
        {
            return GetOUByGuid(guid, null, null);
        }

        /// <summary>
        /// ������֯��λ��DN�õ���֯��λ����
        /// </summary>
        /// <param name="dn">DN����ȫת����ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static OU GetOUByDN(string dn, string userName, string password)
        {
            return GetOUByPath(dn, userName, password);
        }

        /// <summary>
        /// ������֯��λ��DN�õ���֯��λ����ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="dn">DN����ȫת����ġ�</param>
        /// <returns>��������ڣ�����null��</returns>
        public static OU GetOUByDN(string dn)
        {
            return GetOUByDN(dn, null, null);
        }

        /// <summary>
        /// ������֯��λ��ADsPath�õ���֯��λ����
        /// </summary>
        /// <param name="path">ADsPath����ȫת����ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static OU GetOUByPath(string path, string userName, string password)
        {
            DirectoryEntry entry = GetByPath(path, userName, password);
            if (entry != null)
            {
                OU ou = new OU(entry);
                entry.Close();
                entry.Dispose();

                return ou;
            }
            else
                return null;
        }

        /// <summary>
        /// ������֯��λ��ADsPath�õ���֯��λ����ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="path">ADsPath����ȫת����ġ�</param>
        /// <returns>��������ڣ�����null��</returns>
        public static OU GetOUByPath(string path)
        {
            return GetOUByPath(path, null, null);
        }

        #endregion

        #region Rename

        /// <summary>
        /// ������֯��λDirectoryEntry��������ơ�
        /// </summary>
        /// <param name="ouPath">��֯��λDirectoryEntry��ADsPath��������DN��ʽ������ȫת�塣</param>
        /// <param name="newName">����������ơ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public static void RenameOU(string ouPath, string newName, string userName, string password)
        {
            DirectoryEntry de = GetByPath(ouPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("��֯��λ���󲻴��ڡ�");

            if (de.SchemaClassName != SchemaClass.organizationalUnit.ToString("F"))
                throw new SchemaClassException("�������Ͳ���" + SchemaClass.organizationalUnit.ToString("F") + "��");

            string dn = Utils.EscapeDNBackslashedChar(de.Properties[BaseObject.PROPERTY_DN].Value.ToString());
            string rdn = Utils.GenerateRDNOU(newName);
            if (Exists(Utils.GenerateDN(rdn, Utils.GetParentDN(dn))))
                throw new SameRDNException("�Ѵ���ͬ������");
            try
            {
                de.Rename(rdn);

                de.CommitChanges();
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                de.Close();
                de.Dispose();
            }
        }

        /// <summary>
        /// ������DirectoryEntry��������ƣ�ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="ouPath">��֯��λDirectoryEntry��ADsPath��������DN��ʽ������ȫת�塣</param>
        /// <param name="newName">����������ơ�</param>
        public static void RenameOU(string ouPath, string newName)
        {
            RenameOU(ouPath, newName, null, null);
        }

        /// <summary>
        /// ������֯��λDirectoryEntry��������ơ�
        /// </summary>
        /// <param name="ouGuid">��֯��λDirectoryEntry��Guid</param>
        /// <param name="newName">����������ơ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public static void RenameOU(Guid ouGuid, string newName, string userName, string password)
        {
            RenameOU(ADManager.GetOUByGuid(ouGuid).Dn, newName, userName, password);
        }

        /// <summary>
        /// ������֯��λDirectoryEntry��������ƣ�ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="ouGuid">��֯��λDirectoryEntry��ADsPath</param>
        /// <param name="newName">����������ơ�</param>
        public static void RenameOU(Guid ouGuid, string newName)
        {
            RenameOU(Utils.GenerateADsPath(ouGuid), newName, null, null);
        }

        #endregion

        #region Move

        /// <summary>
        /// �ƶ���֯��λDirectoryEntry��ָ��λ�á�
        /// </summary>
        /// <param name="ouPath">Ҫ�ƶ�����֯��λDirectoryEntry��ADsPath��������DN��ʽ������ȫת�塣</param>
        /// <param name="newLocationPath">�ƶ�����λ�õ�ADsPath��������DN��ʽ������ȫת�塣</param>
        /// <param name="mustOu">�ƶ�����λ�ö�Ӧ��DirectoryEntry�Ƿ��������֯��λ��</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public static void MoveOU(string ouPath, string newLocationPath, bool mustOu, string userName, string password)
        {
            if (!Exists(ouPath))
                throw new EntryNotExistException("��Ҫ���ƶ��Ķ��󲻴��ڡ�");

            DirectoryEntry de = null;
            try
            {
                de = GetByPath(ouPath, userName, password);

                MoveOU(de, newLocationPath, mustOu, userName, password);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (de != null)
                {
                    de.Close();
                    de.Dispose();
                }
            }
        }

        /// <summary>
        /// �ƶ���֯��λDirectoryEntry��ָ��λ�ã�ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="ouPath">Ҫ�ƶ�����֯��λDirectoryEntry��ADsPath</param>
        /// <param name="newLocationPath">�ƶ�����λ�õ�ADsPath</param>
        /// <param name="mustOu">�ƶ�����λ�ö�Ӧ��DirectoryEntry�Ƿ��������֯��λ��</param>
        public static void MoveOU(string ouPath, string newLocationPath, bool mustOu)
        {
            MoveUser(ouPath, newLocationPath, mustOu, null, null);
        }

        /// <summary>
        /// �ƶ���֯��λDirectoryEntry��ָ��λ�á�
        /// </summary>
        /// <param name="ou">Ҫ�ƶ�����֯��λDirectoryEntry��Guid</param>
        /// <param name="newLocation">�ƶ�����λ�õ�Guid</param>
        /// <param name="mustOu">�ƶ�����λ�ö�Ӧ��DirectoryEntry�Ƿ��������֯��λ��</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public static void MoveOU(Guid ou, Guid newLocation, bool mustOu, string userName, string password)
        {
            MoveUser(ADManager.GetOUByGuid(ou).Dn,
               ADManager.GetOUByGuid(newLocation).Dn, mustOu, userName, password);
        }

        /// <summary>
        /// �ƶ���֯��λDirectoryEntry��ָ��λ�ã�ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="ou">Ҫ�ƶ�����֯��λDirectoryEntry��Guid</param>
        /// <param name="newLocationPath">�ƶ�����λ�õ�Guid</param>
        /// <param name="mustOu">�ƶ�����λ�ö�Ӧ��DirectoryEntry�Ƿ��������֯��λ��</param>
        public static void MoveOU(Guid ou, Guid newLocationPath, bool mustOu)
        {
            MoveUser(ou, newLocationPath, mustOu, null, null);
        }

        /// <summary>
        /// �ƶ���֯��λDirectoryEntry��ָ��λ�á�
        /// </summary>
        /// <param name="de">Ҫ�ƶ�����֯��λDirectoryEntry����</param>
        /// <param name="newLocationPath">�ƶ�����λ�õ�ADsPath</param>
        /// <param name="mustOu">�ƶ�����λ�ö�Ӧ��DirectoryEntry�Ƿ��������֯��λ��</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        internal static void MoveOU(DirectoryEntry de, string newLocationPath, bool mustOu, string userName, string password)
        {
            if (!Exists(newLocationPath))
                throw new EntryNotExistException("�ƶ�����λ�ö��󲻴��ڡ�");

            DirectoryEntry newLocation = null;
            try
            {
                newLocation = GetByPath(newLocationPath, userName, password);

                if (de.SchemaClassName != SchemaClass.organizationalUnit.ToString("F"))
                    throw new SchemaClassException("��Ҫ���ƶ��Ķ������Ͳ���" + SchemaClass.organizationalUnit.ToString("F") + "��");

                if (mustOu && newLocation.SchemaClassName != SchemaClass.organizationalUnit.ToString("F"))
                    throw new SchemaClassException("�ƶ�����λ�ö������Ͳ���" + SchemaClass.organizationalUnit.ToString("F") + "��");

                if (Exists(Utils.GetRDNValue(de.Properties[BaseObject.PROPERTY_DN].Value.ToString()) + "," +
                    newLocation.Properties[BaseObject.PROPERTY_DN].Value.ToString()))
                    throw new SameRDNException("�ƶ�����λ���´���ͬ������");

                de.MoveTo(newLocation);
                de.CommitChanges();
            }
            catch (InvalidOperationException ioe)   // ָ���� DirectoryEntry ����������
            {
                throw new NotContainerException(ioe.Message, ioe);
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (newLocation != null)
                {
                    newLocation.Close();
                    newLocation.Dispose();
                }
            }
        }

        #endregion

        #region Structure

        /// <summary>
        /// ��ȡ��֯��λ������
        /// </summary>
        /// <param name="ouGuid">��֯��λDirectoryEntry��Guid</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns></returns>
        public OU GetOUSubTree(Guid ouGuid, string userName, string password)
        {
            OU ou = GetOUByGuid(ouGuid);

            if (ou == null)
                throw new EntryNotExistException("��֯��λ���󲻴��ڡ�");

            return ou.GetSubTree(userName, password);
        }

        /// <summary>
        /// ��ȡ��֯��λ������ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="ouGuid">��֯��λDirectoryEntry��Guid</param>
        /// <returns></returns>
        public OU GetOUSubTree(Guid ouGuid)
        {
            return GetOUSubTree(ouGuid, null, null);
        }

        /// <summary>
        /// ��ȡ��֯��λ����֯��λ��
        /// </summary>
        /// <param name="ouGuid">��֯��λDirectoryEntry��Guid</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns></returns>
        public List<OU> GetOUChildren(Guid ouGuid, string userName, string password)
        {
            OU ou = GetOUByGuid(ouGuid);

            if (ou == null)
                throw new EntryNotExistException("��֯��λ���󲻴��ڡ�");

            return ou.GetChildren(userName, password);
        }

        /// <summary>
        /// ��ȡ��֯��λ����֯��λ��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="ouGuid">��֯��λDirectoryEntry��Guid</param>
        /// <returns></returns>
        public List<OU> GetOUChildren(Guid ouGuid)
        {
            return GetOUChildren(ouGuid, null, null);
        }

        /// <summary>
        /// ��ȡ��֯��λ����֯��λ��
        /// </summary>
        /// <param name="ouGuid">��֯��λDirectoryEntry��Guid</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns></returns>
        public OU GetOUParent(Guid ouGuid, string userName, string password)
        {
            OU ou = GetOUByGuid(ouGuid);

            if (ou == null)
                throw new EntryNotExistException("��֯��λ���󲻴��ڡ�");

            return ou.GetParent(userName, password);
        }

        /// <summary>
        /// ��ȡ��֯��λ����֯��λ��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="ouGuid">��֯��λDirectoryEntry��Guid</param>
        /// <returns></returns>
        public OU GetOUParent(Guid ouGuid)
        {
            return GetOUParent(ouGuid, null, null);
        }

        #endregion

        #endregion


        /// <summary>
        /// ͨ��ADsPath��ȡ����Ŀǰ����User,OU��Group
        /// </summary>
        /// <param name="path">ADsPath����ȫת����ġ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����null��</returns>
        public static BaseObject GetObjectByPath(string path, string userName, string password)
        {
            BaseObject baseObject = null;
            DirectoryEntry entry = GetByPath(path, userName, password);
            if (entry != null)
            {
                SchemaClass schema = SchemaClass.none;
                try
                {
                    schema = (SchemaClass)(System.Enum.Parse(typeof(SchemaClass), entry.SchemaClassName));
                    switch (schema)
                    {
                        case SchemaClass.user:
                            baseObject = new User(entry);
                            break;
                        case SchemaClass.group:
                            baseObject = new Group(entry);
                            break;
                        case SchemaClass.organizationalUnit:
                            baseObject = new OU(entry);
                            break;
                    }
                }
                catch
                { }
                
                entry.Close();
                entry.Dispose();

                return baseObject;
            }
            else
                return null;
        }

        /// <summary>
        /// ָ����SAMAccountName�û������Ƿ���ڡ�
        /// </summary>
        /// <param name="sAmAccountName">sAMAccountName</param>
        /// <param name="an">������ڣ���Ӧ��sAMAccountName��</param>
        /// <param name="dn">������ڣ���Ӧ��DN��</param>
        /// <param name="precision">true��ʾ��ȫƥ�䣬false��ʾǰ��ƥ�䡣</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns>��������ڣ�����false��</returns>
        public static bool SamAccountNameExists(string sAmAccountName, out string an, out string dn, bool precision,
            string userName, string password)
        {
            an = null;
            dn = null;
            List<DirectoryEntry> entries = Search("sAMAccountName=" + Utils.Escape4Query(sAmAccountName) + "*", null, null, null, SearchScope.Subtree, userName, password);
            if (entries.Count >= 1)
            {
                string schemaClassName = entries[0].SchemaClassName;
                bool valid = ((schemaClassName == SchemaClass.group.ToString("F")) || (schemaClassName == SchemaClass.user.ToString("F")));

                if (valid)
                {
                    an = entries[0].Properties["sAMAccountName"].Value.ToString();
                    if ((precision && (an == sAmAccountName)) || (!precision))
                    {
                        dn = Utils.EscapeDNBackslashedChar(entries[0].Properties[BaseObject.PROPERTY_DN].Value.ToString());
                    }
                    else
                    {
                        an = null;
                        valid = false;
                    }
                    
                }

                entries[0].Close();
                entries[0].Dispose();

                return valid;
            }

            return false;
        }


        #region special

//        /// <summary>
//        /// ��ȡ��ͳ�Ա�Ķ�Ӧ��ϵ
//        /// </summary>
//        /// <returns>�ֵ�,��ı�ʶ����Ϊ��,��Ӧ���û���Ϊֵ</returns>
//        public static Dictionary<string, List<User>>  GetGroupMembers()
//        {
//            return null;

//            // Ӧ���õ����ϵ��,Ȼ��Ӹ���ʼ����
//            // 
//            /*
//reuslt : dir -- user

//progress :
//foreach dic
//    get group with perm
//    foreach group
//        get group member
//        save dir -- member(user)


//how to get dic perm group



//how to get group member 
//can get all user
//can get all group
//can get uesr belong group
//can get group belong group

//so create a dictionary,group is key,user is value -- result

//first,get all user -->save in a dic,guid is key(sid?)
//second,get all group-->save in a dic,guid is key(sid?)
//third,foreach user get user belong group, save in result dic
//forth,foreach group get group belong group, get saved group member,then save in result dic
//    -- must from base group--how?
//    -- with a tree, save group belong relation,process from root



//todo,filter user who is not in db        
//             */
//        }

        #endregion

    }
}
