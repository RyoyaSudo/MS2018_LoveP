using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraManager : MonoBehaviour {

    //バーチャルカメラオブジェクト
    [SerializeField] private GameObject[] virtualCameraObjArray;

    // 初期化
    void Start ()
    {
        //最初はバーチャルカメラは使用しないので全てOFF
        for ( int nCnt = 0; nCnt < virtualCameraObjArray.Length; nCnt++ )
        {
            virtualCameraObjArray[nCnt].SetActive(false);
        }
    }

    // 更新
    void Update ()
    {
		
	}

    /// <summary>
    /// SetActiveのONとOFF
    /// </summary>
    /// <param name="type">
    /// バーチャルカメラの種類
    /// </param>
    /// <param name="bActive">
    /// ONかOFFか
    /// </param>
    public void SetActive (VirtualCamera.VIRTUALCAMERA_TYPE type , bool bActive)
    {
        for ( int nCnt = 0; nCnt < virtualCameraObjArray.Length; nCnt ++ )
        {
            if ( virtualCameraObjArray[nCnt].GetComponent<VirtualCamera>().virtualCameraType == type)
            {
                virtualCameraObjArray[nCnt].SetActive(bActive);
            }
        }
    }

    /// <summary>
    /// バーチャルカメラオブジェクトを返す
    /// </summary>
    /// <param name="type">
    /// バーチャルカメラの種類
    /// </param>
    public GameObject GetVirtualCamera (VirtualCamera.VIRTUALCAMERA_TYPE type)
    {
        for (int nCnt = 0; nCnt < virtualCameraObjArray.Length; nCnt++)
        {
            if (virtualCameraObjArray[nCnt].GetComponent<VirtualCamera>().virtualCameraType == type)
            {
                return virtualCameraObjArray[nCnt];
            }
        }
        Debug.LogError("バーチャルカメラオブジェクト取得に失敗しました。\n名前:" + type);
        return null;
    }
}
