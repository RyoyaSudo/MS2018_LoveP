using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour {

    GameObject player;  // プレイヤーキャラ

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");     // プレイヤーキャラ取得
    }
	
	// Update is called once per frame
	void LateUpdate () {
        // プレイヤーキャラの位置
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        this.transform.position = new Vector3(playerPos.x, 75.0f, playerPos.y);
        transform.rotation = player.transform.rotation;
        transform.Rotate(new Vector3(1, 0, 0), 90);
    }
}
