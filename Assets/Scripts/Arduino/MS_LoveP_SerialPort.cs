using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppKit2;

public class MS_LoveP_SerialPort : MonoBehaviour {

    // シリアル通信用スクリプト
    ArduinoSerial2 serial;

    // シリアル用ポート番号（USBポートによって変化、Inspectorでの記載例：COM4　）
    public string portNum;

    // 送信文字列
    [SerializeField] string sendStr;

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        serial = null;

        StartCoroutine( LoadCom() );
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        // 以下、シリアルポートが通信可能か否かのコード
        serial = ArduinoSerial2.Instance;
        Debug.Log( "PortNum:" + portNum );
        bool success = serial.Open(portNum, ArduinoSerial2.Baudrate.B_115200);

        if( !success )
        {
            return;
        }

        serial.OnDataReceived += SerialCallBack;

        DontDestroyOnLoad( gameObject );
    }

    void SerialCallBack( string m )
    {

    }

    /// <summary>
    /// COM情報を外部から取得する処理
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadCom()
    {
        // C ドライブのみ利用可能
        var www = new WWW("file:C:/MS_LoveP/LoveP_Serial.txt");
        yield return www;

        if( string.IsNullOrEmpty( www.error ) )
        {
            var text = www.text;
            portNum = text;
            Debug.Log( "PortNumReadSuccess" );
        }
    }

    /// <summary>
    /// アルディーノ側にシリアル通信でメッセージを送信する処理
    /// </summary>
    public void SerialSendMessage()
    {
        serial.Write( sendStr );
    }

    public void ClosePort()
    {
        if( serial != null )
        {
            serial.Close();
            serial.OnDataReceived -= SerialCallBack;
        }
    }

    private void OnApplicationQuit()
    {
        ClosePort();
        Destroy( gameObject );
    }
}
