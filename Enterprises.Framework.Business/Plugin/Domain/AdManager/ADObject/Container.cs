using System;
using System.DirectoryServices;
using Enterprises.Framework.Plugin.Domain.AdManager.Enum;

namespace Enterprises.Framework.Plugin.Domain.AdManager.ADObject
{
    /// <summary>
    /// AD Container
    /// </summary>
    public class Container : BaseObject
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


        #region ctors
        /// <summary>
        ///  Ĭ�Ϲ�������
        /// </summary>
        public Container() {}
        /// <summary>
        /// ���캯��������DirectoryEntry������й��졣
        /// </summary>
        /// <param name="entry">DirectoryEntry����</param>
        internal Container(DirectoryEntry entry)
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
            base.Parse(entry, SchemaClass.container);

            this.description = Utils.GetProperty(entry, Container.PROPERTY_DESCRIPTION);
        }



        #region reference

        #region container properity

        /*
         * 
        objectClass=top,container,
        cn=Users,
        description=Default container for upgraded user accounts,
        distinguishedName=CN=Users,DC=maodou,DC=com,
        instanceType=4,
        whenCreated=2007/8/10 8:08:36,
        whenChanged=2007/8/10 8:08:36,
        uSNCreated=System.__ComObject,
        uSNChanged=System.__ComObject,
        showInAdvancedViewOnly=False,
        name=Users,
        objectGUID=System.Byte[],
        systemFlags=-1946157056,
        objectCategory=CN=Container,CN=Schema,CN=Configuration,DC=maodou,DC=com,
        isCriticalSystemObject=True,
        nTSecurityDescriptor=System.__ComObject,
                    
        */

        #endregion

        #endregion
    }
}
