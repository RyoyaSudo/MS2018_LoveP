using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLogoManager : MonoBehaviour {

    // 子オブジェクト
    [SerializeField] TweenAnimation airplaneObj;
    [SerializeField] TweenAnimation rocketObj;
    [SerializeField] TweenAnimation earthObj;
    [SerializeField] TweenAnimation cloudRightTopObj;
    [SerializeField] TweenAnimation cloudLeftTopObj;
    [SerializeField] TweenAnimation cloudLeftBottomObj;
    [SerializeField] TweenAnimation logoMainObj;
    [SerializeField] TweenAnimation logoPushStartObj;

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start () {
        airplaneObj.Play();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
    }
}
