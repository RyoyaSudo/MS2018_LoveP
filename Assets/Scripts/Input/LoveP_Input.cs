using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppKit;

/// <summary>
/// 本プロジェクトの入力処理クラス
/// </summary>
public class LoveP_Input : MonoBehaviour {

    /// <summary>
    /// Unity標準の入力処理を使う判定値。
    /// 独自デバイスを利用する場合、アタッチされたオブジェクトのInspector内でfalseに設定する。
    /// </summary>
    [SerializeField] bool isDefaultInputUse;

    // シリアル通信用スクリプト
    ArduinoSerial serial;

    // シリアル用ポート番号（USBポートによって変化、Inspectorでの記載例：COM4　）
    public string portNum;

    // 水平軸、垂直軸
    private float device_H = 0.0f;
    private float device_V = 0.0f;

    // ボタン
    private bool pushA = false;

    /// <summary>
    /// 垂直軸補正値
    /// </summary>
    public float Device_V_Default { get { return device_V_Default; } }
    [SerializeField][Range(-1f,1f)] float device_V_Default;

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        serial = null;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start () {
        // 現在のデバイス状態を確認する
        if( isDefaultInputUse )
        {
            Debug.LogWarning( "入力モード:Unity標準" );
            device_V_Default = 0.0f;
        }
        else
        {
            Debug.LogWarning( "入力モード:独自デバイス" );

            // 以下、シリアルポートが通信可能か否かのコード
            serial = ArduinoSerial.Instance;

            bool success = serial.Open(portNum, ArduinoSerial.Baudrate.B_115200);

            if (!success)
            {
                return;
            }

            // OnDaraReceivedに関数を追加することで、ArduinoSerial.csから呼ばれるようになる
            serial.OnDataReceived += SerialCallBack;
        }
	}

    void SerialCallBack(string m)
    {
        objRotation(m);
        objBrake(m);
        objPush(m);
    }

    // シリアル通信の終了処理
    private void OnDestroy()
    {
        if( serial != null )
        {
            serial.Close();
            serial.OnDataReceived -= SerialCallBack;
        }
    }

    // 水平軸をシリアル通信から読み取る
    void objRotation(string message)
    {
        string[] a;

        a = message.Split("="[0]);
        if (a.Length != 2)
        {
            print("Out of Length : " + a.Length);
            return;
        }

        if (a[0] == "y")
        {
            float v = float.Parse( a[1].Trim() );

            // 水平軸分解能に関して
            // -1024～1024の間のため、1024で割って-1～1の範囲に収める
            v = ( v / 1024.0f );

            device_H = v;
        }
    }

    // 垂直軸をシリアル通信から読み取る
    void objBrake(string message)
    {
        string[] a;

        a = message.Split("="[0]);
        if (a.Length != 2)
        {
            print("Out of Length : " + a.Length);
            return;
        }

        if (a[0] == "x")
        {
            float v = float.Parse(a[1].Trim());

            // 垂直軸分解能に関して
            // -1024～1024の間のため、1024で割って-1～1の範囲に収める
            v = ( v / 1024.0f );

            device_V = v;
        }
    }

    // ボタン押下をシリアル通信から読み取る
    void objPush(string message)
    {
        string[] a;

        a = message.Split("="[0]);

        if (a.Length != 2)
        {
            print("Out of Length : " + a.Length);
            return;
        }

        if (a[0] == "button")
        {
            if (a[1].Trim() == "true")
            {
                pushA = true;
            }
            if (a[1].Trim() == "false")
            {
                pushA = false;
            }
        }
    }

    /// <summary>
    /// ボタン入力状況取得処理
    /// </summary>
    /// <param name="buttonName">
    /// 取得するボタン名。
    /// Fire1 : 決定ボタン
    /// Fire2 : キャンセルボタン
    /// </param>
    /// <returns>true or falseで返却</returns>
    public bool GetButton( string buttonName )
    {
        bool value = false;

        if( buttonName == "Fire1" )
        {
            value = GetButtonFire1();
        }
        else if( buttonName == "Fire2" )
        {
            value = GetButtonFire2();
        }
        else
        {
            Debug.LogError( "仮想デバイスエラー\n軸取得処理で不定のボタンが指定されました。\n指定名:" + buttonName );
        }

        return value;
    }

    /// <summary>
    /// 軸入力値取得処理
    /// </summary>
    /// <param name="axisName">
    /// 取得する軸名。
    /// Horizontal : 水平軸
    /// Vertical : 垂直軸
    /// </param>
    /// <returns>-1.0f～1.0fの間で返却</returns>
    public float GetAxis( string axisName )
    {
        float value = 0.0f;

        if( axisName == "Vertical" )
        {
            value = GetVerticalAxis();
        }
        else if( axisName == "Horizontal" )
        {
            value = GetHorizontalAxis();
        }
        else
        {
            Debug.LogError( "仮想デバイスエラー\n軸取得処理で不定の軸が指定されました。\n指定名:" + axisName );
        }

        return value;
    }

    /// <summary>
    /// 垂直軸取得処理
    /// </summary>
    /// <returns>-1.0f～1.0fで返却すること</returns>
    private float GetVerticalAxis()
    {
        float value = 0.0f;

        if( isDefaultInputUse )
        {
            value = Input.GetAxis( "Vertical" );
        }
        else
        {
            value = device_V;
        }

        return value;
    }

    /// <summary>
    /// 水平軸取得処理
    /// </summary>
    /// <returns>-1.0f～1.0fで返却すること</returns>
    private float GetHorizontalAxis()
    {
        float value = 0.0f;

        if( isDefaultInputUse )
        {
            value = Input.GetAxis( "Horizontal" );
        }
        else
        {
            value += device_H;
        }

        return value;
    }

    /// <summary>
    /// 決定ボタン押下判定処理
    /// </summary>
    /// <returns></returns>
    private bool GetButtonFire1()
    {
        bool value = false;

        if( isDefaultInputUse )
        {
            value = Input.GetButton( "Fire1" );
        }
        else
        {
            value = pushA;
        }

        return value;
    }

    /// <summary>
    /// 決定ボタン押下判定処理
    /// </summary>
    /// <returns></returns>
    private bool GetButtonFire2()
    {
        bool value = false;

        if( isDefaultInputUse )
        {
            value = Input.GetButton( "Fire2" );
        }
        else
        {
            // ここに独自デバイスのハンドル左ボタン取得処理を追加して！！
        }

        return value;
    }

}
