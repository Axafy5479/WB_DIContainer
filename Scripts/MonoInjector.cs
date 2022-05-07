using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WB.DI
{
    /// <summary>
    /// シーンが開始された際に
    /// 全てのMonoBehaviourクラスを調べ、
    /// [Inject]アトリビュートが付与されたメソッド実行
    /// </summary>
    internal class MonoInjector : MonoBehaviour
    {
        /// <summary>
        /// 設定により、必ず1番初めに実行されることが保証されている
        /// </summary>
        private void Awake()
        {
            //ヒエラルキー上の全てのMonoBehaviourを取得
            var allMono = 
                
                //全てのMonoBehaviour
                Resources.FindObjectsOfTypeAll(typeof(MonoBehaviour))
                    
                    //Resourcesフォルダは除外(Hierarchy上のみに限定)
                .Where(c => c.hideFlags != HideFlags.NotEditable && c.hideFlags != HideFlags.HideAndDontSave);
            
            
            
            foreach (var m in allMono)
            {
                //全てのMonoBehaviourクラスにたいして、
                
                //全てのMethodを取得
                var type = m.GetType();
                var methods =
                    type.GetMethods(BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                
                foreach (var method in methods)
                {
                    //それらmethodのうち
                    
                    //[Inject]アトリビュートが付与されたものを取り出し、
                    if (method.GetCustomAttribute(typeof(InjectAttribute)) != null)
                    {
                        //引数の情報を取り出す
                        var paramTypes = method.GetParameters();

                        //引数の型からインスタンスを取得
                        var paramObjects = Array.ConvertAll(paramTypes, t => WBDI.Get(t.ParameterType));

                        //methodを実行
                        method.Invoke(m, paramObjects);
                    }
                }


            }
        }
    }
}
