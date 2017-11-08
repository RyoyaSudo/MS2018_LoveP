using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  //画面遷移を可能にする
using UnityEngine;

/// <summary>
/// ゲームシーン管理クラス
/// </summary>
/// <remarks>
/// シーンの状態管理などを主に行う。
/// </remarks>
public class Game : MonoBehaviour {

    public GameObject CityObj;
    public GameObject StarObj;
    public GameObject PlayerObj;
    public GameObject CameraObj;
    public GameObject CatInObj;
    public GameObject SpawnManagerObj;
    public GameObject TimeObj;

    int readyCount;

    public enum Phase
    {
        GAME_PAHSE_READY = 0,
        GAME_PAHSE_CITY,
        GAME_PAHSE_STAR
    }
    Phase phase;

    // Use this for initialization
    void Start () {
        phase = Phase.GAME_PAHSE_READY;
        SetPhase(Phase.GAME_PAHSE_READY);
        
    }
	
	// Update is called once per frame
	void Update () {
		switch( phase )
        {
            case Phase.GAME_PAHSE_READY:
                {
                    readyCount++;
                    if( readyCount > 180 )
                    {
                        SetPhase(Phase.GAME_PAHSE_CITY);
                    }
                    break;
                }
            case Phase.GAME_PAHSE_CITY:
                {
                    if( TimeObj.GetComponent<TimeCtrl>().GetTime() <= 0 )
                    {
                        SceneManager.LoadScene("Result");
                    }

                    break;
                }
            case Phase.GAME_PAHSE_STAR:
                {
                    break;
                }
        }
	}

    public void SetPhase( Game.Phase SetPhase )
    {
        phase = SetPhase;
        switch( phase )
        {
            case Phase.GAME_PAHSE_READY:
                {
                    PhaseReadyStart();
                    break;
                }
            case Phase.GAME_PAHSE_CITY:
                {
                    PhaseCityStart();
                    break;
                }
            case Phase.GAME_PAHSE_STAR:
                {
                    PhaseStarStart();
                    break;
                }
        }
    }

    //TODO 同じコンポーネントを使ってるところは後できれいにする
    void PhaseReadyStart()
    {
        Debug.Log("ready");
        //SetPhase(Phase.GAME_PAHSE_CITY);
        PlayerObj.transform.position = new Vector3(175.0f, 1.0f, 120.0f);
        PlayerObj.transform.rotation = new Quaternion(0.0f, 180.0f, 0.0f, 0.0f);
        PlayerObj.GetComponent<Player>().SetState(Player.State.PLAYER_STATE_IN_CHANGE);
        TimeObj.GetComponent<TimeCtrl>().SetState(TimeCtrl.State.TIME_STATE_STOP);
        readyCount = 0;
    }

    void PhaseCityStart()
    {
        Debug.Log("cityStart");
        CityObj.SetActive(true);
        StarObj.SetActive(false);
        CameraObj.GetComponent<LovePCameraController>().enabled = true;
        PlayerObj.GetComponent<Player>().SetState(Player.State.PLAYER_STATE_STOP);
        TimeObj.GetComponent<TimeCtrl>().SetState(TimeCtrl.State.TIME_STATE_RUN);
    }

    void PhaseStarStart()
    {
        CityObj.SetActive(false);
        StarObj.SetActive(true);
        PlayerObj.transform.position = new Vector3(250.0f, 290.0f, -300.0f);
        PlayerObj.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        PlayerObj.GetComponent<Player>().speed = 1300f;
        PlayerObj.GetComponent<Player>().speedMax = 60.0f;
        CameraObj.GetComponent<LovePCameraController>().enabled = false;
        CameraObj.GetComponent<StarCameraController>().enabled = true;
    }

    //タイトルシーンから移行
    //初期設定を行う（Playerの位置、GameStateの設定、初期乗客スポーン）
    //ゲーム開始準備（Ready,Playerの操作を受け付けない）
    //ゲーム開始（Playerを動けるようにする、タイムの動作開始）
    //ゲーム中
    //時間切れでリザルトに遷移
}
