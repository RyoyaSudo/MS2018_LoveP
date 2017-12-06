using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideArrow : MonoBehaviour
{
    GameObject player;      // プレイヤー
    void Start()
    {
        player = GameObject.Find("Player"); // プレイヤーオブジェクトを取得
    }

    /// <summary>
    /// プレイヤーに最も近い乗客の位置を取得
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPassengerPos()
    {
        Vector3 passengerPos = new Vector3(0.0f, 0.0f, 0.0f); // プレイヤーに最も近い乗客の位置
        float dis = 99999.0f;   // プレイヤーと乗客の位置の開き（比較対象）
        float dis2;             // プレイヤーと乗客の位置の開き

        foreach (Transform child in transform)
        {// 子要素である全乗客とプレイヤーの位置を測る

            // 乗客とプレイヤーの位置を測る
            dis2 = Vector3.Distance(child.transform.position, player.transform.position);

            if (dis > dis2)
            {// もし、プレイヤーとの近さを更新したら

                // 戻り値を更新した子要素の位置に設定
                passengerPos = child.transform.position;

                if (dis2 <= 20.0f)
                {// もし、近すぎたら
                    passengerPos = new Vector3(-999.0f, -1999.0f, -11123.0f);
                    break;
                }

                // 距離更新
                dis = dis2;
            }
        }
        return passengerPos;
    }
}
