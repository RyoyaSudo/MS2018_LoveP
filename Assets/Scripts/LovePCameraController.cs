using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LovePCameraController : MonoBehaviour
{
    /// <summary>
    /// プレイヤーオブジェクト。
    /// 位置などを参照して追従する処理を実装するのに利用。
    /// </summary>
    GameObject player;
    public string playerObjPath;

    Vector3 dirStdV;                // 基準方向ベクトル
    float radius;

    // Use this for initialization
    void Start()
    {
        // オブジェクトを取得
        player = GameObject.Find( playerObjPath );

        Vector3 offsetV = ( transform.position + player.transform.position ) - player.transform.position;
        radius = offsetV.magnitude;

        // Degree値の基準方向をベクトルに
        Matrix4x4 mtxR = Matrix4x4.Rotate( transform.rotation );

        Vector3 trsStdV = transform.position;

        dirStdV = mtxR.MultiplyVector( trsStdV.normalized );
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 向き設定
        transform.rotation = player.transform.rotation;

        // プレイヤーからオフセット分ずらす値の作成
        Matrix4x4 mtxR = Matrix4x4.Rotate( transform.rotation );

        Vector3 trsV = dirStdV.normalized * radius;

        // 現在位置を設定
        //   現在角度に応じてオフセット分のベクトルを回転させる
        transform.position = mtxR.MultiplyVector( trsV ) + player.transform.position;
    }

    private void OnGUI()
    {
        if( Game.IsOnGUIEnable )
        {
            GUIStyleState styleState;
            styleState = new GUIStyleState();
            styleState.textColor = Color.white;

            GUIStyle guiStyle = new GUIStyle();
            guiStyle.fontSize = 48;
            guiStyle.normal = styleState;

            string str;

            str = "向き:" + transform.rotation.eulerAngles + "\n半径:" + radius + "\n基準方向ベクトル:" + dirStdV;

            GUI.Label( new Rect( 0 , 0 , 800 , 600 ) , str , guiStyle );
        }
    }
}
