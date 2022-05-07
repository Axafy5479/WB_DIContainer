using UnityEngine;
using WB.DI;

public class Test : MonoBehaviour
{

    /// <summary>
    /// ゲームオブジェクトが生成された後、
    /// 1番初めに呼ばれるメソッド。
    ///
    /// 事前に登録したHumanクラスのインスタンスが注入される
    /// </summary>
    /// <param name="human"></param>
    [Inject]
    private void Construct(Human human)
    {
        Human = human;
    }
    
    /// <summary>
    /// Constructメソッドの引数が登録される
    /// </summary>
    private Human Human { get; set; }
    
    /// <summary>
    /// 最後にAwakeが呼ばれ、
    /// HumanクラスのRun()メソッドが実行される
    /// </summary>
    void Awake()
    {
        Human.Open();
    }
    
}


/// <summary>
/// つかむことができるオブジェクトが実装するインターフェース
/// </summary>
public interface IGrabbable
{
    void Grab();
}

/// <summary>
/// 開けることができるオブジェクトが実装するインターフェース
/// 開けられるものには取っ手が付いているはずなので、IGrabbableを実装している
/// ※自動ドア? 何それ??
/// </summary>
public interface IOpenable:IGrabbable
{
    void Open();
}

/// <summary>
/// ドアを表現するクラス
/// 開けることができるためIOpenableインターフェースを実装する
/// </summary>
public class Door:IOpenable
{
    [Inject]
    public Door(){ }

    /// <summary>
    /// 握られたときに呼ばれるメソッド
    /// </summary>
    public void Grab()
    {
        Debug.Log("ドアノブが握られた");
    }
    
    /// <summary>
    /// 開けられた時に呼ばれるメソッド
    /// </summary>
    public void Open()
    {
        Debug.Log("＼＼がっちゃっ／／");
    }
    
}

/// <summary>
/// 人間クラス
/// ドアを開けるためだけに生まれた存在()
/// </summary>
public class Human
{
    /// <summary>
    /// コンストラクタ
    ///
    /// 事前に登録しておいたドアが注入される
    /// </summary>
    /// <param name="openable"></param>
    [Inject]
    public Human(IOpenable openable)
    {
        Openable = openable;
    }

    /// <summary>
    /// 引数として渡されたドアを保持するプロパティ
    /// </summary>
    public IOpenable Openable { get; }

    /// <summary>
    /// 開けるメソッド
    /// </summary>
    public void Open()
    {
        //握って、、、
        Openable.Grab();
        
        //開ける!
        Openable.Open(); 
    }
}


/// <summary>
/// ゲーム起動時に一度だけ呼ばれる
///
/// 型に対して実体を紐づける
/// </summary>
public class Installer:ContainerInstaller
{
    protected override void ContainerInstall(IBinder binder)
    {
        binder.BindSingleton<IOpenable,Door>();
        binder.BindSingleton<Human>();
        binder.BindSingleton<IGrabbable,IOpenable>();
    }
}