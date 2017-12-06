using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideArrow2 : MonoBehaviour {
    GameObject player;      // プレイヤー
    GameObject passengers;      // 矢印
    //GameObject oi;
    void Start () {
        player = GameObject.Find("Player");
        passengers = GameObject.Find("Passengers");
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 target = passengers.GetComponent<GuideArrow>().Geth();
        if(target.y <= -100)
        {
            transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y - 4900, transform.parent.position.z + 700);
        }
        else
        {
            
            foreach (Transform child in transform)
            {
                transform.position = player.transform.position;
                Vector3 iti = new Vector3(target.x, transform.position.y, target.z);
                transform.LookAt(iti);          
                transform.parent.rotation = new Quaternion(0.0f, player.transform.rotation.y,0.0f, player.transform.rotation.w);
                transform.position = new Vector3(0.0f, 400.0f,0.0f);
            }
        }
    }
}
