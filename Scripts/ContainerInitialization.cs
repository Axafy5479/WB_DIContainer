using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WB.DI
{

    /// <summary>
    /// DIコンテナをビルドするクラス
    /// </summary>
    static class ContainerInitialization
    {
        /// <summary>
        /// DIコンテナをビルドする
        /// ゲーム開始時に一度だけ呼ばれる
        /// </summary>
        /// <exception cref="Exception"></exception>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ContainerInitialize()
        {
            // ServiceTypeとImplementationTypeのペアの情報を保持するための変数
            var binder = new Binder();

            //ContainerInstallerを継承するクラスを探す
            var initializerType = 
                //全てのアセンブリにたいして
                AppDomain.CurrentDomain.GetAssemblies()
                    //全ての型の情報を持ってきて
                .SelectMany(t => t.GetTypes())
                    //ContainerInstallerを継承している型を探す
                .Where(t => t.IsSubclassOf(typeof(ContainerInstaller)));

            
            foreach (var type in initializerType)
            {
                if (type.GetConstructors().First().GetParameters().Length != 0)
                {
                    throw new Exception("ContainerInitializerを継承するクラスのコンストラクタは、引数を取ってはいけません");
                }
                
                //ContainerInstallerを継承するクラスをインスタンス化
                var obj = Activator.CreateInstance(type);
                
                //ContainerInstallを実行
                var method = typeof(ContainerInstaller).GetMethod("ContainerInstall",
                    BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(obj, new[] {(object)binder});
            }

            //binderが集めたペア情報をもとに、DIコンテナをビルドする
            WBDI.Container = new DiContainer(binder.ServiceDescriptors);

        }
    }
    
}
