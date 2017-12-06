using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideArrow : MonoBehaviour {
    GameObject player;      // プレイヤー
    void Start()
    {
        player = GameObject.Find("Player");
    }

    public Vector3 Geth()
    {
        Vector3 a = new Vector3(0.0f,0.0f,0.0f);
        float dis = 99999.0f;
        float dis2;
        int tourokuNum = -999;
        int cnt = 0;
        foreach (Transform child in transform)
        {
            dis2 = Vector3.Distance(child.transform.position, player.transform.position);
            if(dis > dis2)
            {
                a = child.transform.position;
                tourokuNum = cnt;
                if(dis2 <= 20.0f)
                {
                    a = new Vector3(-999.0f,-1999.0f,-11123.0f);
                    tourokuNum = -999;
                    break;
                }
            }
            cnt++;
            dis = dis2;
        }
        return a;
    }
}
