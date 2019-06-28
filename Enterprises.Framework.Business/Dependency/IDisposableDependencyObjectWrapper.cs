using System;

namespace Enterprises.Framework.Dependency
{
    /// <summary>
    /// This interface is used to wrap an object that is resolved from IOC container.
    /// �ýӿ����ڰ�װ��IOC���������Ķ���
    /// It inherits <see cref="IDisposable"/>, so resolved object can be easily released.
    /// In <see cref="IDisposable.Dispose"/> method, <see cref="IIocResolver.Release"/> is called to dispose the object.
    /// This is non-generic version of <see cref="IDisposableDependencyObjectWrapper{T}"/> interface.
    /// </summary>
    public interface IDisposableDependencyObjectWrapper : IDisposableDependencyObjectWrapper<object>
    {

    }
}