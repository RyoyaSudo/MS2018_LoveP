using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideArrow : MonoBehaviour {

    const int SET_MAX = 10;
    Vector3[] GuestPos = new Vector3[SET_MAX];
    GameObject player;
	// Use this for initialization
	void Start () {
		for(int i = 0; i < SET_MAX; i++)
        {
            GuestPos[i] = new Vector3(-230.0f, 0.0f, 125.0f);
        }
        player = GameObject.Find("Player");
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 400.0f, player.transform.position.z + 800.0f);       
    }
	
	// Update is called once per frame
	void Update () {
        /*int target = 0;
        float[] dis = new float[SET_MAX];

        dis[0] = Vector3.Distance(player.transform.position, GuestPos[0]);
        /*for (int i = 1; i < SET_MAX; i++)
        {
            dis[i] = Vector3.Distance(transform.position, GuestPos[i]);
            if (dis[i - 1] > dis[i])
            {
                target = i;
            }
        }
        // 客に近いと消える        
        foreach (Transform child in transform)
        {
            if (dis[target] <= 20.0f)
            {
              //  child.transform.position = new Vector3(0.0f, -9999, 390.0f);
            }
            else if(child.position.y <= 0.0f)
            {
               // child.transform.position = new Vector3(0.0f, ((float)Screen.height / 3.0f), 390.0f);
            }
        }//*/

        // Vector3 iti = new Vector3(player.transform.position.x, player.transform.position.y + 30.0f, player.transform.position.z + 400.0f);
        // 自分の向きを一番近い乗客に向ける
        //transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
        //transform.position = iti;
    }

    public int GuestPosAdd(Vector3 pos){

        /*int num = 0;
        for (num = 0; num < SET_MAX; num++)
        {
            if(GuestPos[num].y >= 100000)
            {
                GuestPos[num] = pos;
                break;
            }
        }*/
        return 0;
    }

    public void GuestPosKousin(int bangou)
    {
       // GuestPos[bangou] = new Vector3(0.0f, 999999.0f, 0.0f);
    }
}
