namespace Enterprises.Framework.Plugin.Domain.AdManager.Enum
{
    /// <summary>
    /// �˻�ѡ��
    /// </summary>
    public enum AccountOptions
    {
        /// <summary>
        /// ��¼�ű���־�����ͨ�� ADSI LDAP ���ж���д����ʱ���ñ�־ʧЧ�����ͨ�� ADSI WINNT���ñ�־Ϊֻ����
        /// </summary>
        ADS_UF_SCRIPT = 0X0001,

        /// <summary>
        /// �û��ʺŽ��ñ�־
        /// </summary>
        ADS_UF_ACCOUNTDISABLE = 0X0002,

        /// <summary>
        /// ���ļ��б�־
        /// </summary>
        ADS_UF_HOMEDIR_REQUIRED = 0X0008,

        /// <summary>
        /// ���ڱ�־
        /// </summary>
        ADS_UF_LOCKOUT = 0X0010,

        /// <summary>
        /// �û����벻�Ǳ����
        /// </summary>
        ADS_UF_PASSWD_NOTREQD = 0X0020,

        /// <summary>
        /// ���벻�ܸ��ı�־
        /// </summary>
        ADS_UF_PASSWD_CANT_CHANGE = 0X0040,

        /// <summary>
        /// ʹ�ÿ���ļ��ܱ�������
        /// </summary>
        ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0X0080,

        /// <summary>
        /// �����ʺű�־
        /// </summary>
        ADS_UF_TEMP_DUPLICATE_ACCOUNT = 0X0100,

        /// <summary>
        /// ��ͨ�û���Ĭ���ʺ�����
        /// </summary>
        ADS_UF_NORMAL_ACCOUNT = 0X0200,

        /// <summary>
        /// ����������ʺű�־
        /// </summary>
        ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 0X0800,

        /// <summary>
        /// ����վ�����ʺű�־
        /// </summary>
        ADS_UF_WORKSTATION_TRUST_ACCOUNT = 0x1000,

        /// <summary>
        /// �����������ʺű�־
        /// </summary>
        ADS_UF_SERVER_TRUST_ACCOUNT = 0X2000,

        /// <summary>
        /// �����������ڱ�־
        /// </summary>
        ADS_UF_DONT_EXPIRE_PASSWD = 0X10000,

        /// <summary>
        /// MNS �ʺű�־
        /// </summary>
        ADS_UF_MNS_LOGON_ACCOUNT = 0X20000,

        /// <summary>
        /// ����ʽ��¼����ʹ�����ܿ�
        /// </summary>
        ADS_UF_SMARTCARD_REQUIRED = 0X40000,

        /// <summary>
        /// �����øñ�־ʱ�������ʺţ��û��������ʺţ���ͨ�� Kerberos ί������
        /// </summary>
        ADS_UF_TRUSTED_FOR_DELEGATION = 0X80000,

        /// <summary>
        /// �����øñ�־ʱ����ʹ�����ʺ���ͨ�� Kerberos ί�����εģ������ʺŲ��ܱ�ί��
        /// </summary>
        ADS_UF_NOT_DELEGATED = 0X100000,

        /// <summary>
        /// ���ʺ���Ҫ DES ��������
        /// </summary>
        ADS_UF_USE_DES_KEY_ONLY = 0X200000,

        /// <summary>
        ///  ��Ҫ���� Kerberos Ԥ�����֤
        /// </summary>
        ADS_UF_DONT_REQUIRE_PREAUTH = 0X4000000,

        /// <summary>
        /// �û�������ڱ�־
        /// </summary>
        ADS_UF_PASSWORD_EXPIRED = 0X800000,

        /// <summary>
        /// �û��ʺſ�ί�б�־
        /// </summary>
        ADS_UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0X1000000
    }
}
