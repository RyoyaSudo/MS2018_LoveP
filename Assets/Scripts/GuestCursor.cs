using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestCursor : MonoBehaviour {

    GameObject player;   // プレイヤーキャラ
    GameObject parent;  // 親オブジェクト

	// Use this for initialization
	void Start () {

        // プレイヤーオブジェクトと親オブジェクトを取得
        // 2017/12/01 数藤
        //   ・親オブジェクトの取得の仕方を『root』から『parent』に変更しました。
        player = GameObject.Find("Player");
        parent = transform.parent.gameObject;
        parent.transform.position = new Vector3(parent.transform.position.x, 0.0f, parent.transform.position.z);
    }
	
	// Update is called once per frame
	void Update () {
        
        // 自分の位置をプレイヤーの位置に設定
        transform.position = new Vector3(player.transform.position.x,
                                         player.transform.position.y,
                                         player.transform.position.z);

        Vector3 iti = new Vector3(parent.transform.position.x, transform.position.y, parent.transform.position.z);// parent.transform.position;    // 親オブジェクトの位置を取得

        // 自分の向きをプレイヤーに向ける
        transform.LookAt(iti);
        transform.rotation = new Quaternion(0.0f,transform.rotation.y,0.0f,transform.rotation.w);
        float dis = Vector3.Distance(transform.position, iti);

        if(dis >=  140.0f)
        {
            // Y座標を上に上げる
            transform.position = new Vector3(player.transform.position.x, 3.0f, player.transform.position.z);
        }
        else
        {
            // Y座標を上に上げる0.37
            transform.position = new Vector3(player.transform.position.x, -1130.0f, player.transform.position.z);
        }

        transform.Translate(new Vector3(0.0f, 0.0f, 65.4f));
    }
}
