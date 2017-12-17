using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobSyoutotu : MonoBehaviour {
    private MobController mob;   // 親オブジェクトの乗客

    private void Start()
    {
        // 親オブジェクトを取得
        // 2017/12/01 数藤
        //   オブジェクト取得方法をrootからparentに変更
        mob = transform.parent.gameObject.GetComponent<MobController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if( mob != null ) mob.ChangeEscapeRot();
    }
}
