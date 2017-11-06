using System.Collections;
using System.Collections.Generic;
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
        SetPhase(Phase.GAME_PAHSE_CITY);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetPhase( Game.Phase phase )
    {
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

    void PhaseReadyStart()
    {

    }

    void PhaseCityStart()
    {
        CityObj.SetActive(true);
        StarObj.SetActive(false);
        CameraObj.GetComponent<LovePCameraController>().enabled = true;
    }

    void PhaseStarStart()
    {
        CityObj.SetActive(false);
        StarObj.SetActive(true);
        PlayerObj.transform.position = new Vector3(250.0f, 290.0f, -300.0f);
        PlayerObj.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        CameraObj.GetComponent<LovePCameraController>().enabled = false;
        CameraObj.GetComponent<StarCameraController>().enabled = true;
    }
}
