using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideArrow2 : MonoBehaviour
{
    GameObject player;      // プレイヤー
    GameObject passengers;      // 矢印

    void Start()
    {

        // プレイヤーと乗客オブジェクトを取得
        player = GameObject.Find("Player");
        passengers = GameObject.Find("Passengers");
    }

    void Update()
    {

        // プレイヤーに最も近い乗客を取得
        Vector3 target = passengers.GetComponent<GuideArrow>().GetPassengerPos();

        if (target.y <= -100)
        {// もし、ありえない位置が返されたら画面外へ下す
            transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y - 4900, transform.parent.position.z + 700);
        }
        else
        {// 矢印を乗客に向ける           
         // 矢印オブジェクトの位置をプレイヤーの位置に設定
            transform.position = player.transform.position;

            // 乗客の位置を設定して向かせる
            Vector3 iti = new Vector3(target.x, transform.position.y, target.z);
            transform.LookAt(iti);

            // ＧＵＩカメラの向きをプレイヤーの向きと同じにする
            transform.parent.rotation = new Quaternion(0.0f, player.transform.rotation.y, 0.0f, player.transform.rotation.w);

            // 定位置に設定
            transform.position = new Vector3(0.0f, 400.0f, 0.0f);
        }
    }
}
