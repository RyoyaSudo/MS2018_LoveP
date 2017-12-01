using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 本プロジェクトの入力処理クラス
/// </summary>
public class LoveP_Input : MonoBehaviour {

    /// <summary>
    /// Unity標準の入力処理を使う判定値。
    /// 独自デバイスを利用する場合、アタッチされたオブジェクトのInspector内でfalseに設定する。
    /// </summary>
    [SerializeField] bool isDefaultInputUse;

	/// <summary>
    /// 初期化処理
    /// </summary>
	void Start () {
        // 現在のデバイス状態を確認する
        if( isDefaultInputUse )
        {
            Debug.LogAssertion( "入力モード:Unity標準" );
        }
        else
        {
            Debug.Log( "入力モード:独自デバイス" );
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
            // ここに独自デバイスの垂直軸取得処理を追加して！！
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
            // ここに独自デバイスの水平軸取得処理を追加して！！
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
            // ここに独自デバイスのハンドル右ボタン取得処理を追加して！！
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
