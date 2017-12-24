using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCamera : MonoBehaviour {

    //バーチャルカメラ
    public enum VIRTUALCAMERA_TYPE
    {
        READY_VCAM1 ,
        READY_VCAM2 ,
        READY_VCAM3 ,
        RIDE_VCAM1 ,
        GETOFF_VCAM1 ,
        GETOFF_VCAM2 ,
        CHANGE_VCAM1 ,
        GETOFFSMALL_VCAM1,
        GETOFFSMALL_VCAM2,
        GETOFFBIG_VCAM1 ,
        GETOFFBIG_VCAM2 ,
        GETOFFBIG_VCAM3 ,
    }
    public VIRTUALCAMERA_TYPE virtualCameraType;

	// 初期化
	void Start ()
    {
		
	}
	
	// 更新
	void Update ()
    {
		
	}
}
