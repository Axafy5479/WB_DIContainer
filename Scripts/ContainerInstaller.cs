namespace WB.DI
{
    /// <summary>
    /// DIコンテナのビルドを行うクラスが継承する抽象クラス
    /// </summary>
    public abstract class ContainerInstaller
    {
        /// <summary>
        /// "ゲーム開始直後"に一度だけ呼ばれるメソッド
        /// binder.Bindメソッドを用いて、型とインスタンスの紐づけを行う
        /// </summary>
        /// <param name="binder"></param>
        protected abstract void ContainerInstall(IBinder binder);
    }
}
