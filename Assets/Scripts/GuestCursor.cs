using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestCursor : MonoBehaviour {

    GameObject player;   // プレイヤーキャラ
    GameObject parent;  // 親オブジェクト

	// Use this for initialization
	void Start () {

        // プレイヤーオブジェクトと親オブジェクトを取得
        player = GameObject.Find("Player");
        parent = transform.root.gameObject;

    }
	
	// Update is called once per frame
	void Update () {
        Vector3 iti = parent.transform.position;    // 親オブジェクトの位置を取得

        // 自分の位置をプレイヤーの位置に設定
        transform.position = new Vector3(player.transform.position.x, 
                                         player.transform.position.y, 
                                         player.transform.position.z);

        // 自分の向きをプレイヤーに向ける
        transform.LookAt(iti);

        float dis = Vector3.Distance(transform.position, iti);

        if(dis >=  45.0f)
        {
            // Y座標を上に上げる
            transform.position = new Vector3(player.transform.position.x, 30.0f, player.transform.position.z);
        }
        else
        {
            // Y座標を上に上げる
            transform.position = new Vector3(player.transform.position.x, -1130.0f, player.transform.position.z);
        }
        
    }
}
