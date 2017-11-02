using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapIcon : MonoBehaviour {

    GameObject human;   // 親オブジェクトの乗客
	// Use this for initialization
	void Start () {
        //親オブジェクトを取得
        human = transform.root.gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        int state = human.GetComponent<Human>().GetStateType(); // 親オブジェクトの状態を取得

        if(state != 2 && state != 1 && state != 0)
        {// もし表示する必要のない状態なら消す。
            Destroy(gameObject);
        }
	}
}
