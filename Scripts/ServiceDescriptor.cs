using System;
using System.Reflection;

namespace WB.DI
{
    /// <summary>
    /// 型-実装ペアを保持するクラス
    /// </summary>
    internal class ServiceDescriptor
    {
        /// <summary>
        /// 登録する型
        /// </summary>
        public Type ServiceType { get; }
        
        /// <summary>
        /// 実装の型
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        /// インスタンス
        /// </summary>
        public object Implementation { get; internal set; }
        
        /// <summary>
        /// コンストラクタの情報
        /// </summary>
        public ConstructorInfo CtorInfo { get; internal set; }
        
        /// <summary>
        /// スコープ
        /// </summary>
        public ServiceLifetime Lifetime { get; }

        #region コンストラクタ
        internal ServiceDescriptor(object implementation, ServiceLifetime lifetime)
        {
            ServiceType = implementation.GetType();
            Implementation = implementation;
            Lifetime = lifetime;
        }

        internal ServiceDescriptor(Type serviceType,ConstructorInfo ctorInfo, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
            CtorInfo = ctorInfo;
        }

        internal ServiceDescriptor(Type serviceType, Type implementationType,ConstructorInfo ctorInfo, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
            ImplementationType = implementationType;
            CtorInfo = ctorInfo;
        }
        #endregion
    }
}
