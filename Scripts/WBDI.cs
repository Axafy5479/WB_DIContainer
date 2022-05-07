using System;

namespace WB.DI
{
    /// <summary>
    /// 型からインスタンスを取得するAPIを提供
    /// </summary>
    public static class WBDI
    {
        /// <summary>
        /// DIコンテナ本体
        /// </summary>
        internal static DiContainer Container { get; set; }
        
        /// <summary>
        /// 型Tのインスタンスを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() => (T) Get(typeof(T));
        
        /// <summary>
        /// Type型のインスタンスを取得する
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static object Get(Type serviceType) => Container.GetService(serviceType);
    }
}
