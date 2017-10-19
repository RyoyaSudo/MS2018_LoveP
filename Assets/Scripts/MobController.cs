using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{

    const float ESCAPE_AREA = 175.0f;   // この中に入ったら逃げる（半径）
    const float ESCAPE_CHANGE_AREA = 75.0f;    // 逃げてるときこの中に入ったら方向転換（半径）
    const float DELETE_AREA = 400.0f;   // 逃げ状態の時にこの範囲を出ると消える（半径）
    const int ESCAPE_TIME = 40;         // 逃げるのに使う時間
    enum STATUS { NORMAL = 0, ESCAPE }; // 状態の種類(普通、逃げる)  
    STATUS status;                      // 状態 
    public Vector3 escapeSpeed;         // プレイヤーから逃げるスピード
    public Vector3 escapeRot;           // プレイヤーから逃げる時の向き
    int stateTime;                      // 普通以外の状態になっている時間
    GameObject player;                  // プレイヤー

    // Use this for initialization                          
    void Start()
    {
        status = STATUS.NORMAL;                 // ふつう状態
        stateTime = 0;                          // 逃げた時間を０にする
        player = GameObject.Find("Player");     // プレイヤーキャラ取得
    }

    // Update is called once per frame
    void Update()
    {

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

            // 普通以外状態の時間をカウント
            stateTime++;

            if (stateTime <= ESCAPE_TIME)
            {// 逃げる時間を過ぎていないときは逃げる

                if (transform.rotation.y < escapeRot.y && escapeRot.y >= 0.0f)
                {// 指定の角度を向く
                    transform.Rotate(new Vector3(0.0f, 20.0f, 0.0f));
                }
                else if (transform.rotation.y > escapeRot.y && escapeRot.y <= 0.0f)
                {// 指定の角度を向く
                    transform.Rotate(new Vector3(0.0f, -20.0f, 0.0f));
                }

                if (((playerPos.x - mobPos.x) * (playerPos.x - mobPos.x)) +
                    ((playerPos.y - mobPos.y) * (playerPos.y - mobPos.y)) <= ESCAPE_CHANGE_AREA)
                {// プレイヤーキャラがぶつかりそうになったら９０度向きを変える
                    stateTime = 0;
                    escapeRot.y += 90.0f * escapeSpeed.x;
                    if (escapeRot.y >= 180)
                    {
                        escapeRot.y -= 360.0f;
                    }
                    escapeSpeed.x *= Mathf.Cos(escapeRot.y);
                }
                // 移動
                transform.Translate(escapeSpeed);
            }
            else
            {// 逃げ状態の終わるタイミングで状態とそのカウンターを戻す
                stateTime = 0;
                status = STATUS.NORMAL;     // 普通状態にする
            }
        }
    }
}
