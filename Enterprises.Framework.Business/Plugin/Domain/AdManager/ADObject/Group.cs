using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Enterprises.Framework.Plugin.Domain.AdManager.Enum;
using Enterprises.Framework.Plugin.Domain.AdManager.Exception;

namespace Enterprises.Framework.Plugin.Domain.AdManager.ADObject
{
    /// <summary>
    /// AD Group ����
    /// </summary>
    public class Group : BaseObject
    {
        #region PROPERTY const

        public const string PROPERTY_CN             = "cn";                     // Common name,string, RDN

        public const string PROPERTY_DESCRIPTION    = "description";            // ����
        public const string PROPERTY_INFO           = "info";                   // ע��
        public const string PROPERTY_MAIL           = "mail";                   // �����ʼ�
        public const string PROPERTY_GROUPTYPE      = "groupType";              // ������
        public const string PROPERTY_ACCOUNT        = "sAMAccountName";         // ������Windows2000ǰ��
        public const string PROPERTY_ACCOUNTYPE     = "sAMAccountType";         // �˻�����
        public const string PROPERTY_MANAGEDBY      = "managedBy";              // ������
        public const string PROPERTY_MEMBER         = "member";                 // ��Ա
        public const string PROPERTY_MEMBEROF       = "memberOf";               // ������


        #endregion


        #region fields

        private string description;
        private string info;
        private string accountName;
        private string[] members;
        private string[] memberOf;

        #endregion


        #region properties

        /// <summary>
        /// 
        /// </summary>
        public string AccountName
        {
            get { return this.accountName; }
            set 
            {
                this.accountName = value;

                foreach (char i in Utils.InvalidSAMAccountNameChars)
                {
                    this.accountName = this.accountName.Replace(i, '_');
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Info
        {
            get { return this.info; }
            set { this.info = value; }
        }

        /// <summary>
        /// ��ȡ���г�Ա��DN��
        /// </summary>
        /// <remarks>����ϵͳ�����飬��Ա�������û�������������</remarks>
        public string[] MembersDN
        {
            get { return this.members; }
        }

        #endregion


        #region maintain methods

        /// <summary>
        /// ����顣
        /// </summary>
        /// <param name="locationPath">�鱻��ӵ�λ�ã�ADsPath��DN��ʽ����ȫת�塣</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public void Add(string locationPath, string userName, string password)
        {
            if (locationPath.IndexOf(ParaMgr.LDAP_IDENTITY) >= 0)
                locationPath = locationPath.Substring(7);

            DirectoryEntry parent = null;
            DirectoryEntry newGroup = null;

            // Ĭ��λ�ã���Users������
            if (String.IsNullOrEmpty(locationPath))
                locationPath = "CN=Users," + ParaMgr.ADFullPath;

            if (!ADManager.Exists(locationPath))
                throw new EntryNotExistException("ָ����λ�ö��󲻴��ڡ�");

            string rdn = Utils.GenerateRDNCN(this.name);                                    // ʹ��name��CN
            // �����������Ҫ��DN��ʽ�ĵ�Path
            if (ADManager.Exists(Utils.GenerateDN(rdn, locationPath)))
                throw new EntryNotExistException("ָ����λ���´���ͬ������");

            try
            {
                parent = ADManager.GetByPath(locationPath, userName, password);
                newGroup = parent.Children.Add(rdn, "group");                           

                Utils.SetProperty(newGroup, Group.PROPERTY_ACCOUNT, this.accountName);
                Utils.SetProperty(newGroup, Group.PROPERTY_INFO, this.info);
                Utils.SetProperty(newGroup, Group.PROPERTY_DESCRIPTION, this.description);
                Utils.SetProperty(newGroup, Group.PROPERTY_GROUPTYPE, (int)GroupScope.ADS_GROUP_TYPE_GLOBAL_GROUP);

                newGroup.CommitChanges();

                // reload
                this.Parse(newGroup);
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
                if (newGroup != null)
                {
                    newGroup.Close();
                    newGroup.Dispose();
                }
            }
        }

        /// <summary>
        /// ����飬ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="locationPath">�鱻��ӵ�λ�ã�ADsPath</param>
        public void Add(string locationPath)
        {
            Add(locationPath, this.iUserName, this.iPassword);
        }


        /// <summary>
        /// �����顣
        /// </summary>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public void Update(string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.Guid, userName, password);

                Utils.SetProperty(de, Group.PROPERTY_ACCOUNT, this.accountName);       // ���������
                Utils.SetProperty(de, Group.PROPERTY_INFO, this.info);
                Utils.SetProperty(de, Group.PROPERTY_DESCRIPTION, this.description);

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
        /// �����飬ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        public void Update()
        {
            Update(this.iUserName, this.iPassword);
        }


        /// <summary>
        /// ɾ���顣
        /// </summary>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public void Remove(string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.guid, userName, password);

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
        /// ɾ���飬ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        public void Remove()
        {
            Remove(this.iUserName, this.iPassword);
        }


        /// <summary>
        /// ���������ơ�
        /// </summary>
        /// <param name="newName">����������ơ�</param>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public void Rename(string newName, string userName, string password)
        {
            DirectoryEntry de = null;

            string rdn = Utils.GenerateRDNCN(newName);

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
        /// ���������ƣ�ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="newName">����������ơ�</param>
        public void Rename(string newName)
        {
            Rename(newName,this.iUserName, this.iPassword);
        }


        /// <summary>
        /// ���û���ӵ��顣
        /// </summary>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <param name="usersDN">��Ҫ��ӵ��û���DN��</param>
        public void AddUser(string userName, string password, params string[] usersDN)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.guid, userName, password);

                // ���� -- ������һ���Ѵ��ڵ�member�����ܻ������쳣
                List<string> toAdd = new List<string>();
                foreach (string udn in usersDN)
                {
                    if (!(Array.BinarySearch(this.members, udn) >= 0))
                    {
                        toAdd.Add(udn);
                    }
                }

                foreach (string user in toAdd)
                {
                    de.Properties[Group.PROPERTY_MEMBER].Add(Utils.UnEscapeDNBackslashedChar(user));
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
        /// <param name="usersDN">��Ҫ��ӵ��û���DN��</param>
        public void AddUser(params string[] usersDN)
        {
            AddUser(this.iUserName, this.iPassword, usersDN);
        }


        /// <summary>
        /// ���û��������Ƴ���
        /// </summary>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <param name="usersDN">��Ҫ�Ƴ����û���DN��</param>
        public void RemoveUser(string userName, string password, params string[] usersDN)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.guid, userName, password);

                // ���� -- ����Ƴ�һ�������ڵ�member���������쳣
                List<string> toRemoves = new List<string>();
                foreach (string user in usersDN)
                {
                    if (Array.BinarySearch(this.members, user) >= 0)
                        toRemoves.Add(user);
                }
                foreach (string user in toRemoves)
                {
                    de.Properties[Group.PROPERTY_MEMBER].Remove(
                        Utils.UnEscapeDNBackslashedChar(user));  // ȥ��/ת�壬�Ա�ƥ��
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
        /// ���û��������Ƴ���ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="usersDN">��Ҫ�Ƴ����û���DN��</param>
        public void RemoveUser(params string[] usersDN)
        {
            RemoveUser(this.iUserName, this.iPassword, usersDN);
        }

        #endregion


        #region .ctors

        /// <summary>
        ///  Ĭ�Ϲ�������
        /// </summary>
        public Group() {}

        /// <summary>
        /// ���캯��������DirectoryEntry������й��졣
        /// </summary>
        /// <param name="entry">DirectoryEntry����</param>
        internal Group(DirectoryEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException();

            this.Parse(entry);
        }

        /// <summary>
        /// ���캯��������SearchResult������й��졣
        /// </summary>
        /// <param name="result">SearchResult����</param>
        internal Group(SearchResult result)
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
            base.Parse(entry, SchemaClass.group);

            this.accountName = Utils.GetProperty(entry, Group.PROPERTY_ACCOUNT);
            this.description = Utils.GetProperty(entry, Group.PROPERTY_DESCRIPTION);
            this.info = Utils.GetProperty(entry, Group.PROPERTY_INFO);
            if (entry.Properties.Contains(Group.PROPERTY_MEMBER))
            {
                List<string> ms = new List<string>();
                foreach (object m in entry.Properties[Group.PROPERTY_MEMBER])
                {
                    ms.Add(Utils.EscapeDNBackslashedChar(m.ToString()));            // ת��/
                }
                ms.Sort();          // ������ -- �Ա��ڲ�ʹ��
                this.members = ms.ToArray() ;
            }
            else
                this.members = new string[] { };
            if (entry.Properties.Contains(Group.PROPERTY_MEMBEROF))
            {
                List<string> ms = new List<string>();
                foreach (object m in entry.Properties[Group.PROPERTY_MEMBEROF])
                {
                    ms.Add(Utils.EscapeDNBackslashedChar(m.ToString()));            // ת��/
                }
                ms.Sort();          // ������ -- �Ա��ڲ�ʹ��
                this.memberOf = ms.ToArray();
            }
            else
                this.memberOf = new string[] { };

        }

        /// <summary>
        /// ����SearchResult����
        /// </summary>
        /// <param name="result">SearchResult����Ӧ��������Ҫ�����ԡ�</param>
        protected void Parse(SearchResult result)
        {
            base.Parse(result, SchemaClass.group);

            this.accountName = Utils.GetProperty(result, "samaccountname");
            this.description = Utils.GetProperty(result, "description");
            this.info = Utils.GetProperty(result, "info");
            if (result.Properties.Contains("member"))
            {
                List<string> ms = new List<string>();
                foreach (object m in result.Properties["member"])
                {
                    ms.Add(Utils.EscapeDNBackslashedChar(m.ToString()));            // ת��/
                }
                ms.Sort();          // ������ -- �Ա��ڲ�ʹ��
                this.members = ms.ToArray();
            }
            else
                this.members = new string[] { };
            if (result.Properties.Contains("memberof"))
            {
                List<string> ms = new List<string>();
                foreach (object m in result.Properties["memberof"])
                {
                    ms.Add(Utils.EscapeDNBackslashedChar(m.ToString()));            // ת��/
                }
                ms.Sort();          // ������ -- �Ա��ڲ�ʹ��
                this.memberOf = ms.ToArray();
            }
            else
                this.memberOf = new string[] { };

        }

        #endregion


        /// <summary>
        /// ��ȡ�������������DN��
        /// </summary>
        /// <returns></returns>
        public List<string> GetMemberOfDN()
        {
            // ��ֹ����
            List<string> dn = new List<string>();
            dn.AddRange(this.memberOf);
            return dn;
        }


        /// <summary>
        /// ��ȡ�����ĳ�Ա��
        /// </summary>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns></returns>
        public List<DirectoryEntry> GetMembers(string userName, string password)
        {
            List<DirectoryEntry> entries = new List<DirectoryEntry>();

            DirectoryEntry de = null;

            try
            {         
                foreach (string member in this.members)
                {
                    de = ADManager.GetByDN(member, userName, password);

                    if (de != null)
                        entries.Add(de);
                }

                return entries;
            }
            catch (DirectoryServicesCOMException dsce)
            {
                foreach (DirectoryEntry d in entries)
                {
                    if (d != null)
                    {
                        d.Close();
                        d.Dispose();
                    }
                }

                throw dsce;
            }
        }

        /// <summary>
        /// ��ȡ�����ĳ�Ա��ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <returns></returns>
        public List<DirectoryEntry> GetMembers()
        {
            return GetMembers(this.iUserName, this.iPassword);
        }


        /// <summary>
        /// ��ȡ�����ĳ�Ա�������û���
        /// </summary>
        /// <param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        /// <returns></returns>
        public List<User> GetUserMembers(string userName, string password)
        {
            List<User> users = new List<User>();

            DirectoryEntry de = null;
            string userSchemaClassName = SchemaClass.user.ToString("F");

            try
            {
                foreach (string member in this.members)
                {
                    de = ADManager.GetByDN(member, userName, password);

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

                return users;
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
        /// ��ȡ�����ĳ�Ա�������û���
        /// </summary>
        /// <returns></returns>
        public List<User> GetUserMembers()
        {
            return GetUserMembers(this.iUserName, this.iPassword);
        }



        #region reference

        #region group properity
        /*
objectClass=top,group,
cn=ou1group1,
description=����,
member=CN=ou1user1,OU=OU1,DC=maodou,DC=com,
distinguishedName=CN=ou1group1,OU=OU1,DC=maodou,DC=com,
instanceType=4,
whenCreated=2007/8/14 3:44:35,
whenChanged=2007/8/16 21:11:22,
uSNCreated=System.__ComObject,
memberOf=CN=Administrators,CN=Builtin,DC=maodou,DC=com,
info=ע��,
uSNChanged=System.__ComObject,
name=ou1group1,
objectGUID=System.Byte[],
objectSid=System.Byte[],
sAMAccountName=ou1group1,
sAMAccountType=268435456,
managedBy=CN=sherwinzhu,CN=Users,DC=maodou,DC=com,
groupType=-2147483646,
objectCategory=CN=Group,CN=Schema,CN=Configuration,DC=maodou,DC=com,
mail=mail@126.com,
nTSecurityDescriptor=System.__ComObject,

         */

        #endregion

        #endregion
    }
}
