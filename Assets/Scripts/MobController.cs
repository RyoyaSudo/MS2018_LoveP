using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{

    const float ESCAPE_AREA = 90.0f;   // この中に入ったら逃げる（半径）
    const int ESCAPE_TIME = 40;         // 逃げるのに使う時間
    const float ESCAPE_SPEED = -0.75f;   // プレイヤーから逃げるスピード
    enum STATUS { NORMAL = 0, ESCAPE, ESCAPE2 }; // 状態の種類(普通、逃げる,逃げる時に方向転換)  
    STATUS status;                      // 状態 
    float escapeRot;                    // プレイヤーから逃げる時の向き
    int stateTime;                      // 普通以外の状態になっている時間
    GameObject player;                  // プレイヤー
    Vector3 teiiti;                     // なんかあったらここへ戻る
    short btk;                           // 障害物にぶつかったかどうか
    float oldRot;                       // 移動前の古い向き
    Vector2 oldPlayerPos;               // 古いプレイヤーの位置

    // Use this for initialization                          
    void Start()
    {
        status = STATUS.NORMAL;                 // ふつう状態
        stateTime = 0;                          // 逃げた時間を０にする
        player = GameObject.Find("Player");     // プレイヤーキャラ取得
        escapeRot = 0.0f;                     // 逃げる角度の初期設定
        teiiti = transform.position;            // 定位置を定める
        btk = 0;                            // ぶつかっていない
        oldRot = transform.rotation.y;          // 古い向きを設定
        oldPlayerPos = new Vector2(player.transform.position.x, player.transform.position.y);   // 古い位置を設定
    }

    // Update is called once per frame
    public void Update()
    {
        // プレイヤーキャラとモブキャラの位置
        Vector2 mobPos = new Vector2(this.transform.position.x, this.transform.position.z);
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        float playerRot = player.transform.rotation.y;  // プレイヤーキャラの向き

        if (((playerPos.x - mobPos.x) * (playerPos.x - mobPos.x)) +
            ((playerPos.y - mobPos.y) * (playerPos.y - mobPos.y)) <= ESCAPE_AREA && status == STATUS.NORMAL)
        {// プレイヤーキャラが近いと逃げる

            //♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪
            // 逃げる音
            //♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪
            foreach (Transform child in transform)
            {
                child.GetComponent<test>().EvadeAnimON();
            }

            status = STATUS.ESCAPE;     // 逃げ状態にする

            if (btk == 0)
                escapeRot = (-playerRot - Random.Range(80.0f,90.0f)); // 逃げる角度決定
            else if(btk == 1)
                escapeRot = (-playerRot + Random.Range(80.0f, 90.0f)); btk = 2;
        }

        if (status == STATUS.ESCAPE || status == STATUS.ESCAPE2)
        {// 逃げ状態の時は逃げる

            // 普通以外状態の時間をカウント
            stateTime++;

            if (stateTime <= ESCAPE_TIME)
            {// 逃げる時間を過ぎていないときは逃げる

                if (stateTime <= 3)
                {// 指定の角度を向く
                    transform.Rotate(new Vector3(0.0f, escapeRot - oldRot / 3, 0.0f));
                }

                // 移動
                transform.Translate(new Vector3(ESCAPE_SPEED, 0.0f, 0.0f));
            }
            else
            {// 逃げ状態の終わるタイミングで状態とそのカウンターを戻す
                stateTime = 0;
                status = STATUS.NORMAL;     // 普通状態にする
                btk = 0;
                oldRot = transform.rotation.y;
                escapeRot = 0.0f;
            }
        }

        oldPlayerPos = playerPos;
    }

    /// <summary>
    /// 障害物にぶつかった時の反応
    /// </summary>
    public void ChangeEscapeRot()
    {
        stateTime = 0;
        status = STATUS.NORMAL;
        escapeRot = 0.0f; 
        btk = 1;
    }
}

