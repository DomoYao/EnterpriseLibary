
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Enterprises.Framework.Plugin.Domain.AdManager.Enum;
using Enterprises.Framework.Plugin.Domain.AdManager.Exception;

namespace Enterprises.Framework.Plugin.Domain.AdManager.ADObject
{
    /// <summary>
    /// ����AD����Ļ��ࡣ
    /// </summary>
    public class BaseObject
    {
        #region PROPERTY Const

        public const string PROPERTY_OBJECTCLASS    = "objectClass";            // ����,string[]��eg:user is {"top","person","organizationalPerson","user"}
        public const string PROPERTY_OBJECTCATEGORY = "objectCategory";         // ���,string��eg:user is "CN=Person,CN=Schema,CN=Configuration,DC=maodou,DC=com"

        public const string PROPERTY_NAME           = "name";                   // RDN,����ͨ��ֱ�Ӹ�ֵ�޸�,string

        public const string PROPERTY_OBJECTGUID     = "objectGUID";             // GUID,byte[16]�����ֽ�ת��Ϊ16���ƣ���NativeGuid����������AD�󶨣���ת��ΪGuid�ṹ��
        public const string PROPERTY_OBJECTSID      = "objectSid";              // SID,byte[28]
        public const string PROPERTY_DN             = "distinguishedName";      // DN,string

        public const string PROPERTY_WHENCREATED    = "whenCreated";            // ����ʱ��,DateTime
        public const string PROPERTY_WHENCHANGED    = "whenChanged";            // ����ʱ��,DateTime
        public const string PROPERTY_INSTANCETYPE   = "instanceType";           // int��eg:user is 4
        public const string PROPERTY_USNCREATED     = "uSNCreated";             // ?,IADsLargeInteger
        public const string PROPERTY_USNCHANGED     = "uSNChanged";             // ?,IADsLargeInteger
        public const string PROPERTY_SECURITY       = "nTSecurityDescriptor";   // ?,?

        #endregion


        #region fields

        /// <summary>
        /// Guid
        /// </summary>
        protected Guid guid;
        /// <summary>
        /// SID
        /// </summary>
        protected byte[] objectSid;
        //Distinguished Name
        private string dn;
        /// <summary>
        /// Object Class
        /// </summary>
        protected string[] objectClass;
        /// <summary>
        /// ObjectCategory DN
        /// </summary>
        protected string objectCategory;
        /// <summary>
        /// Name
        /// </summary>
        protected string name;
        /// <summary>
        /// SchemaClass Name����Ӧ��ö�٣�
        /// </summary>
        protected SchemaClass schema;
        // path
        private string path;

        private DateTime whenCreated;

        // guid:227451df-dbe1-4988-9ed6-1523cdfcece9
        // nativeguid:df517422e1db88499ed61523cdfcece9
        #endregion


        #region properity

        /// <summary>
        /// Guid
        /// </summary>
        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        /// <summary>
        /// NativeGuid��������LDAP�󶨲�ѯ����GUID��
        /// </summary>
        public string NativeGuid
        {
            get { return Utils.ConvertGuidToNativeGuid(this.guid); }
        }

        /// <summary>
        /// ObjectSID
        /// </summary>
        public byte[] ObjectSid
        {
            get { return objectSid; }
            set { objectSid = value; }
        }

        /// <summary>
        /// ObjectSID��16�����ַ�����ʽ��������LDAP�󶨲�ѯ����ObjectSid��
        /// </summary>
        public string OctetObjectSid
        {
            get { return Utils.ConvertToOctetString(objectSid); }
        }

        /// <summary>
        /// Distinguished Name����ת�巴б��/�ַ�
        /// </summary>
        /// <remarks>������ֱ�����á�</remarks>
        public string Dn
        {
            get { return dn; }
            set 
            { 
                //dn = value; 
            }
        }

        /// <summary>
        /// ADsPath��The fully qualified path 
        /// </summary>
        public string Path
        {
            get { return this.path; }
        }

        /// <summary>
        /// ��DN���ɵ�ADsPath
        /// </summary>
        public string DNPath
        {
            get { return ParaMgr.LDAP_IDENTITY + this.dn; }
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// SchemaClassName
        /// </summary>
        public string SchemaClassName
        {
            get
            {
                return this.schema.ToString("F");
            }
        }

        /// <summary>
        /// ObjectCategory Name
        /// </summary>
        public string ObjectCategory
        {
            get
            {
                return Utils.GetRDNValue(this.objectCategory);
            }
        }

        /// <summary>
        /// ��ȡ����ʱ��
        /// </summary>
        public DateTime WhenCreated
        {
            get
            {
                return this.whenCreated;
            }
        }

        #endregion


        #region .ctors

        /// <summary>
        /// Ĭ�Ϲ��캯����������ֱ�Ӵ�������
        /// </summary>
        protected BaseObject()
        { }

        #endregion


        /// <summary>
        /// ����DirectoryEntry����
        /// </summary>
        /// <param name="entry">DirectoryEntry����</param>
        protected virtual void Parse(DirectoryEntry entry)
        {
            this.ParsePrivate(entry);
        }

        private void ParsePrivate(DirectoryEntry entry)
        {
            this.guid = entry.Guid;
            this.dn = Utils.EscapeDNBackslashedChar(Utils.GetProperty(entry, BaseObject.PROPERTY_DN));     // ת�巴б��'/'�ַ�
            this.path = entry.Path;
            this.name = Utils.GetProperty(entry, BaseObject.PROPERTY_NAME);

            List<string> ocList = new List<string>();
            foreach (object oc in (object[])(entry.Properties[BaseObject.PROPERTY_OBJECTCLASS].Value))
            {
                ocList.Add(oc.ToString());
            }
            this.objectClass = ocList.ToArray();
            this.objectCategory = Utils.GetProperty(entry, BaseObject.PROPERTY_OBJECTCATEGORY);

            this.schema = (SchemaClass)(System.Enum.Parse(typeof(SchemaClass), entry.SchemaClassName));

            // OU û��objectSid
            if (schema == SchemaClass.user || schema == SchemaClass.group || schema == SchemaClass.builtinDomain ||
                schema == SchemaClass.computer || schema == SchemaClass.domainDNS)
                this.objectSid = (byte[])(entry.Properties[BaseObject.PROPERTY_OBJECTSID].Value);
            else
                this.objectSid = new byte[] { };
        }

        /// <summary>
        /// ����DirectoryEntry����
        /// </summary>
        /// <param name="entry">DirectoryEntry����</param>
        /// <param name="demandSchema">Ҫ��Ķ���SchemaClass���Ա����������͡�</param>
        protected void Parse(DirectoryEntry entry, SchemaClass demandSchema)
        {
            // this.Parse(entry);      // ERROR -- ��ε���this��

            this.ParsePrivate(entry);

            if (this.schema != demandSchema)
            {
                entry.Close();
                entry.Dispose();

                throw new SchemaClassException("�������Ͳ���" + demandSchema.ToString("F"));
            }
        }

        /// <summary>
        /// ����SearchResult����
        /// </summary>
        /// <param name="result">SearchResult����Ӧ��������Ҫ�����ԡ�</param>
        /// <param name="demandSchema">Ҫ��Ķ���SchemaClass���Ա����������͡�</param>
        protected void Parse(SearchResult result, SchemaClass demandSchema)
        {
            string nativeGuid = "";
            foreach (byte g in (byte[])result.Properties["objectguid"][0])
            {
                nativeGuid += g.ToString("x2");
            }
            this.guid = Utils.ConvertNativeGuidToGuid(nativeGuid);
            this.dn = Utils.EscapeDNBackslashedChar(Utils.GetProperty(result, "distinguishedname"));     // ת�巴б��'/'�ַ�

            this.path = Utils.GetProperty(result, "adspath");
            this.name = Utils.GetProperty(result, "name");

            List<string> ocList = new List<string>();
            foreach (object oc in result.Properties["objectclass"])
            {
                ocList.Add(oc.ToString());
            }
            this.objectClass = ocList.ToArray();
            this.objectCategory = Utils.GetProperty(result, "objectcategory");

            this.schema = SchemaClass.none;
            foreach(string oc in this.objectClass)
            {
                // ��ʱֻ����������
                switch(oc)
                {
                    case "organizationalUnit":
                        this.schema = SchemaClass.organizationalUnit;
                        break;
                    case "group":
                        this.schema = SchemaClass.group;
                        break;
                    case "user":
                        this.schema = SchemaClass.user;
                        break;

                }
                if (this.schema != SchemaClass.none)
                    break;
            }

            // OU û��objectSid
            if (schema == SchemaClass.user || schema == SchemaClass.group)
                this.objectSid = (byte[])(result.Properties["objectsid"][0]);
            else
                this.objectSid = new byte[] { };


            this.whenCreated = DateTime.Parse(Utils.GetProperty(result, "whenCreated"));

            if (this.schema != demandSchema)
            {
                throw new SchemaClassException("�������Ͳ���" + demandSchema.ToString("F"));
            }
        }

        ///<summary>
        ///�ƶ�DirectoryEntry��ָ��λ�á�
        ///</summary>
        ///<param name="newLocationPath">�ƶ�����λ�õ�ADsPath</param>
        ///<param name="userName">�û���ݱ�ʶ--�û�����Ϊ��ʱʹ��Ĭ���û���ݱ�ʶ��</param>
        ///<param name="password">�û���ݱ�ʶ--���롣</param>
        public virtual void Move(string newLocationPath, string userName, string password)
        {
            DirectoryEntry de = ADManager.GetByDN(this.dn, userName, password);     // ������DN -- ��ADManager.Move����˵��

            ADManager.Move(de, newLocationPath, userName, password);

            this.Parse(de);

            de.Close();
            de.Dispose();
        }

        /// <summary>
        /// �ƶ�DirectoryEntry��ָ��λ�ã�ʹ��Ĭ���û���ݱ�ʶ��
        /// </summary>
        /// <param name="newLocationPath">�ƶ�����λ�õ�ADsPath</param>
        public virtual void Move(string newLocationPath)
        {
            this.Move(newLocationPath, null, null);
        }


        #region Identity

        /// <summary>
        /// ����ִ�ж������ʱʹ�õ�Ĭ���û���ݱ�ʶ
        /// </summary>
        /// <param name="userName">�û���ݱ�ʶ--�û�����</param>
        /// <param name="password">�û���ݱ�ʶ--���롣</param>
        public void SetIdentity(string userName, string password)
        {
            this.hasIdentity = true;
            this.iUserName = userName;
            this.iPassword = password;
        }

        /// <summary>
        /// ���ִ�ж������ʱʹ�õ�Ĭ���û���ݱ�ʶ
        /// ���ʱʹ��ִ�н��̵���ݱ�ʶ��
        /// </summary>
        public void ClearIdentity()
        {
            this.hasIdentity = false;
            this.iUserName = null;
            this.iPassword = null;
        }
        protected bool hasIdentity = false;
        protected string iUserName = null;
        protected string iPassword = null;

        #endregion
  
    }
}
