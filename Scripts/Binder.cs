using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WB.DI
{
    /// <summary>
    /// ゲーム開始時、
    /// ServiceTypeとImplementationTypeの紐づけを行うメソッド
    /// </summary>
    public interface IBinder
    {
        void BindSingleton<TService>(TService implementation);
        void BindSingleton<TService>();
        void BindSingleton<TService, TImplementation>() where TImplementation : TService;
        void BindTransient<TService>();
        void BindTransient<TService, TImplementation>() where TImplementation : TService;
    }

    /// <summary>
    /// ゲーム開始時のみでインスタンス化されるクラス
    /// このインスタンスは全てのContainerInstallerのサブクラスに渡され、
    /// 各クラス内で「ServiceType,ImplementationTypeのペア」の情報を受け取る
    ///
    /// これらの情報はServiceDescriptors変数に保持されており、
    /// 最終的にはServiceDescriptorsをもとにDiContainerをビルドする
    /// </summary>
    internal class Binder : IBinder
    {
        internal Binder()
        {
            ServiceDescriptors = new Dictionary<Type, ServiceDescriptor>();
        }
        
        /// <summary>
        /// 受け取ったペアの情報を保持する辞書
        /// これをもとにDiContainerをビルドする
        /// </summary>
        internal Dictionary<Type, ServiceDescriptor> ServiceDescriptors { get; }

        #region Register Methods (様々な方法でペアの情報を受け取れるだけで、どれもやっていることは同じ)

        private void Bind(Type type, ServiceDescriptor descriptor)
        {
            ServiceDescriptors.Add(type, descriptor);
        }

        private void Bind<T>(ServiceDescriptor descriptor)
        {
            Bind(typeof(T), descriptor);
        }

        public void BindSingleton<TService>(TService implementation)
        {
            Bind<TService>(new ServiceDescriptor(implementation, ServiceLifetime.Singleton));
        }

        public void BindSingleton<TService>()
        {
            Bind<TService>(new ServiceDescriptor(typeof(TService),FindCtorToInject(typeof(TService)), ServiceLifetime.Singleton));
        }

        public void BindSingleton<TService, TImplementation>() where TImplementation : TService
        {
            Bind<TService>(new ServiceDescriptor(typeof(TService), typeof(TImplementation),FindCtorToInject(typeof(TImplementation)), ServiceLifetime.Singleton));
        }

        public void BindTransient<TService>()
        {
            Bind<TService>(new ServiceDescriptor(typeof(TService),FindCtorToInject(typeof(TService)), ServiceLifetime.Transient));
        }

        public void BindTransient<TService, TImplementation>() where TImplementation : TService
        {
            Bind<TService>(new ServiceDescriptor(typeof(TService), typeof(TImplementation),FindCtorToInject(typeof(TImplementation)), ServiceLifetime.Transient));
        }
        #endregion

        /// <summary>
        /// [Inject]属性が付与されたコンストラクタを探す
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ConstructorInfo FindCtorToInject(Type type)
        {
            // ImplementationTypeがインターフェースまたは抽象クラスの場合、コンストラクタは現状nullとする
            if (type.IsInterface || type.IsAbstract) return null;
            
            // 全てのコンストラクタを取得
            var ctors = type.GetConstructors();
                
            // このうち[Inject]アトリビュートが付与された物を探す
            var hasAttributeCtors = ctors.Where(ctor=>ctor.GetCustomAttribute(typeof(InjectAttribute)) != null);

            return hasAttributeCtors.Count() switch
            {
                0 => throw new Exception(type+"内に[Inject]属性が付与されたコンストラクタが存在しません"),
                1 => hasAttributeCtors.ElementAt(0),
                _ => throw new Exception(type+"内に[Inject]属性が付与されたコンストラクタが複数存在します")
            };
        }
        
    }
}