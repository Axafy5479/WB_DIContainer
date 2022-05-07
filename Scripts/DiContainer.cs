using System;
using System.Collections.Generic;
using System.Linq;

namespace WB.DI
{
    internal class DiContainer
    {
        internal DiContainer(Dictionary<Type, ServiceDescriptor> serviceDescriptors)
        {
            ServiceDescriptors = serviceDescriptors;
        }

        private Dictionary<Type, ServiceDescriptor> ServiceDescriptors { get; }

        /// <summary>
        /// インスタンスの解決
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public object GetService(Type serviceType)
        {
            //型-インスタンス　マップからserviceTypeに該当するインスタンスを取得
            ServiceDescriptors.TryGetValue(serviceType, out var descriptor);
            
            //登録されていない場合はエラー
            if (descriptor == null)
            {
                throw new Exception($"{serviceType.Name}型に紐づけられたインスタンスは存在しません。");
            }
            
            //既にインスタンスが生成されている場合は(スコープがシングルトンであれば)、それを返す
            if (descriptor.Implementation != null)
            {
                return descriptor.Implementation;
            }

            //インスタンスが生成されていない場合はここで生成する必要がある
            
            //そのためdescriptorからコンストラクタの情報を引っ張り出してくる
            var constructorInfo = descriptor.CtorInfo;


            
            //コンストラクタの情報がない場合(実装の型がインターフェースや抽象クラスの場合)
            if (constructorInfo==null)
            {
                //serviceTypeにたいして紐づけられている実装の型を調べる
                var actualType = descriptor.ImplementationType ?? descriptor.ServiceType;
                
                //serviceTypeと実装の型が同じ場合、インスタンスを生成することは不可能なためエラーを出す
                if (actualType == serviceType)
                {
                    throw new Exception($"インターフェースまたは抽象クラス{serviceType}を継承するクラスのインスタンスが見つかりませんでした");
                }
                else
                {
                    //異なる場合、「実装の型をサービスの型としているインスタンス」を探すため、再度コンテナ内を探す(再帰計算)
                    // 実際のインスタンスの型を取得し、
                    actualType = GetService(actualType).GetType();
                    
                    // コンストラクタの情報を更新する
                    descriptor.CtorInfo = ServiceDescriptors[actualType].CtorInfo;
                }
                
            }


            
            var parameters = 
                //コンストラクタの情報から引数の型を取得し、
                constructorInfo.GetParameters()
                    //それぞれの型に紐づいたインスタンスを、DiContainerから探す
                    .Select(x => GetService(x.ParameterType)).ToArray();

            // 前コードで得たインスタンスを引数に渡してコンストラクタを実行
            var implementation = constructorInfo.Invoke(parameters);
            
            //スコープがシングルトンであれば、ここで作成したインスタンスを保存。次回以降はこのインスタンスを使いまわす
            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                descriptor.Implementation = implementation;
            }

            return implementation;
        }

        internal T GetService<T>()
        {
            return (T) GetService(typeof(T));
        }

    }
}
