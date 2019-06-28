using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Enterprises.Framework.Plugin.Domain.AdManager.Enum;
using Enterprises.Framework.Plugin.Domain.AdManager.Exception;

namespace Enterprises.Framework.Plugin.Domain.AdManager.ADObject
{
    /// <summary>
    /// AD OrganizationalUnit ����
    /// </summary>
    public class OU : BaseObject
    {
        #region PROPERTY const

        public const string PROPERTY_OU                     = "ou";                         // Common name,string, RDN

        public const string PROPERTY_DESCRIPTION            = "description";                // ����
        public const string PROPERTY_MANAGEDBY              = "managedBy";                  // ������
        public const string PROPERTY_ADDRESS_COUNTRY        = "co";                         // ��ַ-����/����,string
        public const string PROPERTY_ADDRESS_COUNTRYAB      = "c";                          // ��ַ-����/������д,string
        public const string PROPERTY_ADDRESS_COUNTRYCODE    = "countryCode";                // ��ַ-����/��������,int���й���������ֵΪCHINA,CN,156
        public const string PROPERTY_ADDRESS_PROVINCE       = "st";                         // ��ַ-ʡ/������,string
        public const string PROPERTY_ADDRESS_CITY           = "l";                          // ��ַ-��/��,string
        public const string PROPERTY_ADDRESS_STREET         = "street";                     // ��ַ-�ֵ�,string
        public const string PROPERTY_ADDRESS_POSTALCODE     = "postalCode";                 // ��ַ-��������,string

        #endregion


        #region fields

        private string description;
        private string managedBy;

        private OU parent;
        private List<OU> children;

        #endregion


        #region properties

        /// <summary>
        /// ����
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// ������DN
        /// </summary>
        public string ManagedBy
        {
            get { return this.managedBy; }
            set { this.managedBy = value; }
        }

        /// <summary>
        /// ������
        /// </summary>
        public OU Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// �Ӷ���
        /// </summary>
        public List<OU> Children
        {
            get 
            {
                if (this.children == null)
                    this.children = new List<OU>();
                return this.children; 
            }
        }


        #endregion


        #region maintain methods

        /// <summary>
        /// �����֯��λ��
        /// </summary>
        /// <param name="locationPath">��֯��λ����ӵ�λ�ã�ADsPath��DN��ʽ����ȫת�塣</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public void Add(string locationPath, string userName, string password)
        {
            if (locationPath.IndexOf(ParaMgr.LDAP_IDENTITY) >= 0)
                locationPath = locationPath.Substring(7);

            DirectoryEntry parent = null;
            DirectoryEntry newOU = null;

            // Ĭ��λ�ã�����������
            if (String.IsNullOrEmpty(locationPath))
                locationPath = ParaMgr.ADFullPath;

            if (!ADManager.Exists(locationPath))
                throw new EntryNotExistException("ָ����λ�ö��󲻴��ڡ�");

            string rdn = Utils.GenerateRDNOU(this.name);                                    // ʹ��name��OU
            // �����������Ҫ��DN��ʽ�ĵ�Path
            if (ADManager.Exists(Utils.GenerateDN(rdn, locationPath)))
                throw new EntryNotExistException("ָ����λ���´���ͬ������");

            try
            {
                parent = ADManager.GetByPath(locationPath, userName, password);
                newOU = parent.Children.Add(rdn, "organizationalUnit");

                Utils.SetProperty(newOU, OU.PROPERTY_DESCRIPTION, this.description);
                Utils.SetProperty(newOU, OU.PROPERTY_MANAGEDBY, this.managedBy);            // ע�⣬������ת��/��DN

                newOU.CommitChanges();

                // reload
                this.Parse(newOU);

            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (parent != null)
                {
                    parent.Close();
                    parent.Dispose();
                }
                if (newOU != null)
                {
                    newOU.Close();
                    newOU.Dispose();
                }
            }
        }

        /// <summary>
        /// �����֯��λ��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="locationPath">��֯��λ����ӵ�λ�ã�ADsPath</param>
        public void Add(string locationPath)
        {
            Add(locationPath, this.iUserName, this.iPassword);
        }

        /// <summary>
        /// ������֯��λ��
        /// </summary>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public void Update(string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.Guid, userName, password);

                Utils.SetProperty(de, OU.PROPERTY_DESCRIPTION, this.description);
                Utils.SetProperty(de, OU.PROPERTY_MANAGEDBY, this.managedBy);       // ע�⣬������ת��/��DN

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
        /// ������֯��λ��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        public void Update()
        {
            Update(this.iUserName, this.iPassword);
        }

        /// <summary>
        /// ɾ����֯��λ��
        /// </summary>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public void Remove(string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.guid, userName, password);

                if (de.Children.GetEnumerator().MoveNext())
                {
                    throw new ExistChildException("��֯��λ�´����Ӷ���");
                }

                de.DeleteTree();

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
        /// ɾ����֯��λ��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        public void Remove()
        {
            Remove(this.iUserName, this.iPassword);
        }

        /// <summary>
        /// ������֯��λ���ơ�
        /// </summary>
        /// <param name="newName">����������ơ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public void Rename(string newName, string userName, string password)
        {
            DirectoryEntry de = null;

            string rdn = Utils.GenerateRDNOU(newName);
            if (ADManager.Exists(Utils.GenerateDN(rdn, Utils.GetParentDN(this.Dn))))
                throw new SameRDNException("�Ѵ���ͬ������");

            try
            {
                de = ADManager.GetByDN(this.Dn, userName, password);        // ������DN��ʽ������ȫת�塣

                de.Rename(rdn);

                de.CommitChanges();

                // Reload
                this.Parse(de);
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
        /// ������֯��λ���ƣ�ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="newName">����������ơ�</param>
        public void Rename(string newName)
        {
            Rename(newName, this.iUserName, this.iPassword);
        }

        /// <summary>
        /// �ƶ�OU��ָ��λ�á�
        /// </summary>
        /// <param name="newLocationPath">�ƶ�����λ�õ�ADsPath��������DN��ʽ������ȫת�塣</param>
        /// <param name="mustOU">�ƶ�����λ�ö�Ӧ��DirectoryEntry�Ƿ��������֯��λ��</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public void Move(string newLocationPath, bool mustOU, string userName, string password)
        {
            DirectoryEntry de = ADManager.GetByDN(this.Dn, userName, password);         // ����DN -- ��ADManager.MoveOU����

            ADManager.MoveOU(de, newLocationPath, mustOU, userName, password);

            this.Parse(de);

            de.Close();
            de.Dispose();

        }

        /// <summary>
        /// �ƶ�OU��ָ��λ�ã�ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="newLocationPath">�ƶ�����λ�õ�ADsPath��������DN��ʽ������ȫת�塣</param>
        /// <param name="mustOU">�ƶ�����λ�ö�Ӧ��DirectoryEntry�Ƿ��������֯��λ��</param>
        public void Move(string newLocationPath, bool mustOU)
        {
            Move(newLocationPath, mustOU, this.iUserName, this.iPassword);
        }


        #endregion


        #region ctors
        /// <summary>
        ///  Ĭ�Ϲ�������
        /// </summary>
        public OU()
        {
        }

        internal OU(DirectoryEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException();

            this.Parse(entry);
        }

        /// <summary>
        /// ���캯��������SearchResult������й��졣
        /// </summary>
        /// <param name="result">SearchResult����</param>
        internal OU(SearchResult result)
        {
            if (result == null)
                throw new ArgumentNullException();

            this.Parse(result);
        }

        #endregion


        #region parse

        /// <summary>
        /// ����DirectoryEntry����
        /// </summary>
        /// <param name="entry">DirectoryEntry����</param>
        protected override void Parse(DirectoryEntry entry)
        {
            this.Parse(entry, SchemaClass.organizationalUnit);

            this.description = Utils.GetProperty(entry, OU.PROPERTY_DESCRIPTION);
            this.managedBy = Utils.GetProperty(entry, OU.PROPERTY_MANAGEDBY);
        }

        /// <summary>
        /// ����SearchResult����
        /// </summary>
        /// <param name="result">SearchResult����Ӧ��������Ҫ�����ԡ�</param>
        protected void Parse(SearchResult result)
        {
            this.Parse(result, SchemaClass.organizationalUnit);

            this.description = Utils.GetProperty(result, "description");
            this.managedBy = Utils.GetProperty(result, "managedby");
        }

        #endregion


        /// <summary>
        /// ��ȡ��֯��λ�����������
        /// </summary>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns></returns>
        public OU GetSubTree(string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.guid, userName, password);

                this.GetSubTree(de.Path, userName, password);

                return this;
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

        private void GetSubTree(string parentPath, string userName, string password)
        {
            this.children = new List<OU>();
            OU ou = null;
            foreach (DirectoryEntry c in
                    ADManager.Search(null, "organizationalUnit", null, parentPath, SearchScope.OneLevel, userName, password))
            {
                ou = new OU(c);
                ou.parent = this;
                this.children.Add(ou);

                ou.GetSubTree(c.Path, userName, password);

                if (c != null)
                {
                    c.Close();
                    c.Dispose();
                }
            }
        }

        /// <summary>
        /// ��ȡ��֯��λ�����������ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <returns></returns>
        public OU GetSubTree()
        {
            return GetSubTree(this.iUserName, this.iPassword);
        }

        /// <summary>
        ///  ��ȡ��֯��λ����֯��λ��
        /// </summary>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns></returns>
        public List<OU> GetChildren(string userName, string password)
        {
            this.children = new List<OU>();
            OU ou = null;
            foreach (DirectoryEntry c in
                    ADManager.Search(null, "organizationalUnit", null, this.Path, SearchScope.OneLevel, userName, password))
            {
                ou = new OU(c);
                ou.parent = this;
                this.children.Add(ou);

                if (c != null)
                {
                    c.Close();
                    c.Dispose();
                }
            }

            return this.children;
        }

        /// <summary>
        ///  ��ȡ��֯��λ����֯��λ��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <returns></returns>
        public List<OU> GetChildren()
        {
            return GetChildren(this.iUserName, this.iPassword);
        }

        /// <summary>
        /// ��ȡ��֯��λ�ĸ���֯��λ��
        /// </summary>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns></returns>
        public OU GetParent(string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.Guid, userName, password);

                DirectoryEntry parentDe = null;
                try
                {
                    parentDe = de.Parent;
                    if (parent.SchemaClassName == SchemaClass.organizationalUnit.ToString("F"))
                        this.parent = new OU(parentDe);
                    else
                        this.parent = null;

                    
                }
                catch (DirectoryServicesCOMException dsce)
                {
                    this.parent = null;
                    throw dsce;
                }
                finally
                {
                    if (parentDe != null)
                    {
                        parentDe.Close();
                        parentDe.Dispose();
                    }
                }
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

            return this.parent;
        }

        /// <summary>
        /// ��ȡ��֯��λ�ĸ���֯��λ��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <returns></returns>
        public OU GetParent()
        {
            return GetParent(this.iUserName, this.iPassword);
        }



        #region reference

        #region organizationalUnit properity

        /*
objectClass=top,organizationalUnit,
c=CN,
l=����,
st=����,
street=֪��·,
ou=OU1,
description=����,
postalCode=100083,
distinguishedName=OU=OU1,DC=maodou,DC=com,
instanceType=4,
whenCreated=2007/8/14 3:36:01,
whenChanged=2007/8/15 18:38:30,
uSNCreated=System.__ComObject,
uSNChanged=System.__ComObject,
co=�й�,
name=OU1,
objectGUID=System.Byte[],
countryCode=156,
managedBy=CN=ou1user1,OU=OU1,DC=maodou,DC=com,
objectCategory=CN=Organizational-Unit,CN=Schema,CN=Configuration,DC=maodou,DC=com,
nTSecurityDescriptor=System.__ComObject,

        */
        #endregion

        #endregion
    }
}
