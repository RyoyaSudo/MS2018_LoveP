using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour {

    const float ESCAPE_AREA = 175.0f;           // この中に入ったら逃げる（半径）
    const float ESCAPE_CHANGE_AREA = 90.0f;     // この中に入ったら逃げパターンを変える（半径）
    const float DELETE_AREA = 400.0f;   // 逃げ状態の時にこの範囲を出ると消える（半径）
    enum STATUS { NORMAL = 0, ESCAPE }; // 状態の種類(普通、逃げる)  
    STATUS status;                      // 状態 
    public Vector3 escapeSpeed;         // プレイヤーから逃げるスピード
    public Vector3 escapeRot;           // プレイヤーから逃げる時の向き
    public Vector3 escapeSpeed2;        // プレイヤーから逃げるスピード2
    public Vector3 escapeRot2;          // プレイヤーから逃げる時の向き2
    bool escapePattern;                 // 逃げ動作のパターン
    GameObject player;                  // プレイヤー

    // Use this for initialization                          
    void Start () {
        status = STATUS.NORMAL;                 // ふつう状態
        player = GameObject.Find("Player");     // プレイヤーキャラ取得
        escapePattern = false;                  // 逃げパターンを１に指定
    }
	
	// Update is called once per frame
	void Update () {

        // プレイヤーキャラとモブキャラの位置
        Vector2 mobPos = new Vector2(this.transform.position.x, this.transform.position.z);
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);

        if (((playerPos.x - mobPos.x) * (playerPos.x - mobPos.x)) +
            ((playerPos.y - mobPos.y) * (playerPos.y - mobPos.y)) <= ESCAPE_AREA && status == STATUS.NORMAL)
        {// プレイヤーキャラが近いと逃げる
            status = STATUS.ESCAPE;     // 逃げ状態にする
        }

        if (status == STATUS.ESCAPE)
        {// 逃げ状態の時は逃げる
            EscapeRotation(escapePattern);
            if(((playerPos.x - mobPos.x) * (playerPos.x - mobPos.x)) +
            ((playerPos.y - mobPos.y) * (playerPos.y - mobPos.y)) <= ESCAPE_CHANGE_AREA)
            {// もしプレイヤーキャラが近づいたら角度を変える
                escapePattern = true;
            }
            // 移動
            transform.Translate(escapeSpeed);
        }
	}

    void EscapeRotation(bool pattern)
    {
        if(!pattern)
        {
            if (transform.rotation.y < escapeRot.y && escapeRot.y >= 0.0f)
            {// 指定の角度を向く
                transform.Rotate(new Vector3(0.0f, 10.0f, 0.0f));
            }
            else if (transform.rotation.y > escapeRot.y && escapeRot.y <= 0.0f)
            {// 指定の角度を向く
                transform.Rotate(new Vector3(0.0f, -10.0f, 0.0f));
            }
        }
        else
        {
            if (transform.rotation.y < escapeRot2.y && escapeRot2.y >= 0.0f)
            {// 指定の角度を向く
                transform.Rotate(new Vector3(0.0f, 10.0f, 0.0f));
            }
            else if (transform.rotation.y > escapeRot2.y && escapeRot2.y <= 0.0f)
            {// 指定の角度を向く
                transform.Rotate(new Vector3(0.0f, -10.0f, 0.0f));
            }
        }
    }
}
