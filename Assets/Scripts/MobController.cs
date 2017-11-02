using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{

    const float ESCAPE_AREA = 280.0f;   // この中に入ったら逃げる（半径）
    const float ESCAPE_CHANGE_AREA = 110.0f;    // 逃げてるときこの中に入ったら方向転換（半径）
                                                // const float DELETE_AREA = 400.0f;   // 逃げ状態の時にこの範囲を出ると消える（半径）
    const int ESCAPE_TIME = 40;         // 逃げるのに使う時間
    const float ESCAPE_SPEED = -1.5f;   // プレイヤーから逃げるスピード
    enum STATUS { NORMAL = 0, ESCAPE, ESCAPE2 }; // 状態の種類(普通、逃げる)  
    STATUS status;                      // 状態 
    float escapeRot;                    // プレイヤーから逃げる時の向き
    int stateTime;                      // 普通以外の状態になっている時間
    GameObject player;                  // プレイヤー

    // Use this for initialization                          
    void Start()
    {
        status = STATUS.NORMAL;                 // ふつう状態
        stateTime = 0;                          // 逃げた時間を０にする
        player = GameObject.Find("Player");     // プレイヤーキャラ取得
        escapeRot = -20.0f;                     // 逃げる角度の初期設定
    }

    // Update is called once per frame
    public void Escape()
    {

        // プレイヤーキャラとモブキャラの位置
        Vector2 mobPos = new Vector2(this.transform.position.x, this.transform.position.z);
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        float playerRot = player.transform.rotation.y;  // プレイヤーキャラの向き

        if (((playerPos.x - mobPos.x) * (playerPos.x - mobPos.x)) +
            ((playerPos.y - mobPos.y) * (playerPos.y - mobPos.y)) <= ESCAPE_AREA && status == STATUS.NORMAL)
        {// プレイヤーキャラが近いと逃げる

            status = STATUS.ESCAPE;     // 逃げ状態にする
            float turn = -80.0f;        // 角度調節

            if (playerPos.x - mobPos.x >= 0.0f)
            {// プレイヤーキャラの位置によって逃げる角度を変更
                turn *= -1;
            }
            escapeRot = (turn + playerRot); // 逃げる角度決定
        }

        if (status == STATUS.ESCAPE || status == STATUS.ESCAPE2)
        {// 逃げ状態の時は逃げる

            // 普通以外状態の時間をカウント
            stateTime++;

            if (stateTime <= ESCAPE_TIME)
            {// 逃げる時間を過ぎていないときは逃げる

                if (stateTime <= 3)
                {// 指定の角度を向く
                    transform.Rotate(new Vector3(0.0f, escapeRot / 3, 0.0f));
                }

                if (((playerPos.x - mobPos.x) * (playerPos.x - mobPos.x)) +
                    ((playerPos.y - mobPos.y) * (playerPos.y - mobPos.y)) <= ESCAPE_CHANGE_AREA && status == STATUS.ESCAPE)
                {// プレイヤーキャラがぶつかりそうになったら９０度向きを変える

                    stateTime = 0;              // 状態異常カウンターを０に戻す
                    status = STATUS.ESCAPE2;    // 逃げる角度変更状態へ
                    float turn2 = -80.0f;       // 逃げる角度
                    if (playerPos.x - mobPos.x >= 0.0f)
                    {// プレイヤーの位置によって角度を変える
                        turn2 *= -1;
                    }
                    escapeRot += turn2;
                }

                // 移動
                transform.Translate(new Vector3(ESCAPE_SPEED, 0.0f, 0.0f));
            }
            else
            {// 逃げ状態の終わるタイミングで状態とそのカウンターを戻す
                stateTime = 0;
                status = STATUS.NORMAL;     // 普通状態にする
            }
        }
    }

    /// <summary>
    /// 逃げる時の角度を変える（主にビルや障害物にぶつかったとき）
    /// </summary>
    public void ChangeEscapeRot()
    {
        escapeRot = (-170.0f + this.transform.rotation.y);
    }
}

