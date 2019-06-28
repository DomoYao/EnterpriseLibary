
using System;
using System.DirectoryServices;
using Enterprises.Framework.Plugin.Domain.AdManager.Enum;

namespace Enterprises.Framework.Plugin.Domain.AdManager.ADObject
{
    public class Builtin : BaseObject
    {
        public const string PROPERTY_CN = "cn";                                // Common name,string, RDN
        public const string PROPERTY_DESCRIPTION = "description";              // ����

        private string description;

        /// <summary>
        /// ����
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        #region .ctors

        /// <summary>
        ///  Ĭ�Ϲ�������
        /// </summary>
        public Builtin() {}

        /// <summary>
        /// ���캯��������DirectoryEntry������й��졣
        /// </summary>
        /// <param name="entry">DirectoryEntry����</param>
        internal Builtin(DirectoryEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException();

            this.Parse(entry);
        }

        #endregion

        /// <summary>
        /// ����DirectoryEntry����
        /// </summary>
        /// <param name="entry">DirectoryEntry����</param>
        protected override void Parse(DirectoryEntry entry)
        {
            base.Parse(entry, SchemaClass.builtinDomain);

            this.description = Utils.GetProperty(entry, Builtin.PROPERTY_DESCRIPTION);
        }


        #region reference

        #region builtinDomain properity
        /*
         * 
        objectClass=top,builtinDomain,
        cn=Builtin,
        distinguishedName=CN=Builtin,DC=maodou,DC=com,
        instanceType=4,
        whenCreated=2007/8/10 8:08:38,
        whenChanged=2007/8/10 8:08:38,
        uSNCreated=System.__ComObject,
        uSNChanged=System.__ComObject,
        showInAdvancedViewOnly=False,
        name=Builtin,
        objectGUID=System.Byte[],
        creationTime=System.__ComObject,
        forceLogoff=System.__ComObject,
        lockoutDuration=System.__ComObject,
        lockOutObservationWindow=System.__ComObject,
        lockoutThreshold=0,
        maxPwdAge=System.__ComObject,
        minPwdAge=System.__ComObject,
        minPwdLength=0,
        modifiedCountAtLastProm=System.__ComObject,
        nextRid=1000,
        pwdProperties=0,
        pwdHistoryLength=0,
        objectSid=System.Byte[],
        serverState=1,
        uASCompat=1,
        modifiedCount=System.__ComObject,
        systemFlags=-1946157056,
        objectCategory=CN=Builtin-Domain,CN=Schema,CN=Configuration,DC=maodou,DC=com,
        isCriticalSystemObject=True,
        nTSecurityDescriptor=System.__ComObject,
            
        */

        #endregion

        #endregion
    }
}
