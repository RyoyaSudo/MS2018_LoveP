using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  //画面遷移を可能にする
using UnityEngine;

public class Nowloading : MonoBehaviour {

    public GameObject nowObj;
    private string loadSceneName;
    private bool loadingFlag;

    // Use this for initialization
    void Start () {
        loadingFlag = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadingStart(string sceneName)
    {
        if (loadingFlag == true) return;
        loadingFlag = true;
        loadSceneName = sceneName;
        StartCoroutine(StartNowloading());
    }

    IEnumerator StartNowloading()
    {
        var async = SceneManager.LoadSceneAsync(loadSceneName);
        nowObj.SetActive(true);

        while (async.progress < 0.9f)
        {
            Debug.Log(async.progress);
            //nowObj.transform.localScale = new Vector3(async.progress, 1.0f, 1.0f);
            nowObj.transform.Rotate(new Vector3(0.0f, 0.0f, 6.0f));
            yield return null;
        }
    }
}
