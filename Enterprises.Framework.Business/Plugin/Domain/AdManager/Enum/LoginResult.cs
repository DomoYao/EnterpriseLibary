
namespace Enterprises.Framework.Plugin.Domain.AdManager.Enum
{
    /// <summary>
    /// ��¼���
    /// </summary>
    public enum LoginResult
    {
        /// <summary>
        /// ����
        /// </summary>
        LOGIN_OK = 0,
        /// <summary>
        /// �û�������
        /// </summary>
        LOGIN_USER_DOT_EXIST,
        /// <summary>
        /// �˻�������
        /// </summary>
        LOGIN_USER_ACCOUNT_INACTIVE,
        /// <summary>
        /// ���벻��ȷ
        /// </summary>
        LOGIN_USER_PASSWORD_INCORRECT
    }
}
