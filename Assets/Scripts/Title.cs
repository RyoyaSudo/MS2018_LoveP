using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour {

    GameObject fadePanel;   // フェードパネル
    public int fadeNum;     // 遷移先の番号

    void Start()
    {
        fadePanel = GameObject.Find("Fade");   //パネルオブジェクトを取得
    }

    // Update is called once per frame
    void Update()
    {
        //_____フェード関連_____________
        if (Input.GetKey(KeyCode.Space))
        {// RETURNキーで次のシーン
            fadePanel.GetComponent<Fade>().SetFadeIn(fadeNum);  //遷移先を設定する
        }
        //______________
    }
}
