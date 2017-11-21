using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobSyoutotu : MonoBehaviour {
    GameObject human;   // 親オブジェクトの乗客
                                    // Use this for initialization
    void Start () {
        // 親オブジェクトを取得
        human = transform.root.gameObject;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        human.GetComponent<MobController>().ChangeEscapeRot();
    }
}
