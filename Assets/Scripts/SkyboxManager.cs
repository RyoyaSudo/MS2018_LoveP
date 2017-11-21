using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxManager : MonoBehaviour {

    //宇宙のスカイボックス
    public Material starSkyBox;

    //街のスカイボックス
    public Material citySkyBox;

    //星、宇宙のスカイボックスに変更
    public void SetStarSkyBox( )
    {
        RenderSettings.skybox = starSkyBox;
    }

    //街のスカイボックスに変更
    public void SetCitySkyBox()
    {
        RenderSettings.skybox = citySkyBox;
    }
}
