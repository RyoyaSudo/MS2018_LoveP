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
	void Update () {
        // プレイヤーキャラの位置
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        this.transform.position = new Vector3(playerPos.x, 34.0f, playerPos.y);
    }
}
