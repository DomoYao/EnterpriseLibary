
namespace Enterprises.Framework.Plugin.Domain.AdManager.Enum
{
    /// <summary>
    /// �����ͺ�������
    /// </summary>
    public enum GroupScope
    {
        /// <summary>
        /// ��ȫ�鱾����( = -2147483648 + 4)
        /// </summary>
        ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP = -2147483644,
        /// <summary>
        /// ��ȫ��ȫ��( = -2147483648 + 2)
        /// </summary>
        ADS_GROUP_TYPE_GLOBAL_GROUP = -2147483646,
        ///// <summary>
        ///// ��ȫ��ͨ��
        ///// </summary>
        //ADS_GROUP_TYPE_UNIVERSAL_GROUP = -2147483640,
        ///// <summary>
        ///// ͨѶ�鱾����
        ///// </summary>
        //ADC_GROUP_TYPE_DOMAIN_LOCAL_GROUP = 4,
        ///// <summary>
        ///// ͨѶ��ȫ��
        ///// </summary>
        //ADC_GROUP_TYPE_GLOBAL_GROUP = 2,
        ///// <summary>
        ///// ͨѶ��ͨ��
        ///// </summary>
        //ADC_GROUP_TYPE_UNIVERSAL_GROUP = 8

        // �������ð�ȫ��
        //-2147483643
    }

    /// <summary>
    /// ������ö��
    /// </summary>
    public enum ADS_GROUP_TYPE_ENUM
    {
        /// <summary>
        /// ������
        /// </summary>
        ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP = 4,
        /// <summary>
        /// ȫ��
        /// </summary>
        ADS_GROUP_TYPE_GLOBAL_GROUP = 2,
        /// <summary>
        /// ��������
        /// </summary>
        ADS_GROUP_TYPE_LOCAL_GROUP = 4,     
        /// <summary>
        /// ��ȫ
        /// </summary>
        ADS_GROUP_TYPE_SECURITY_ENABLED = -2147483648,      // 32λ��������һλΪ1 -- ���Ϊ���
        /// <summary>
        /// ͨ��
        /// </summary>
        ADS_GROUP_TYPE_UNIVERSAL_GROUP = 8
    }

}
