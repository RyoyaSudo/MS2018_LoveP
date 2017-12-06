using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceController : MonoBehaviour {

    /// <summary>
    /// 顔の種類
    /// </summary>
    public enum FaceType
    {
        DEFAULT = 0,
        FACE_TYPE_TEAR,
        FACE_TYPE_CLOSE_EYE,
        FACE_TYPE_SMILE,
        FACE_TYPE_NORMAL,
        NUM_MAX
    }
    public FaceType faceType;

    public GameObject faceObj;
    private Material faceMaterial;
    public Vector2 atlasSize;

    /// <summary>
    /// アニメーション情報
    /// </summary>
    private struct AnimationInfo
    {
        public Vector2 textureSize; //テクスチャのサイズ
        public Rect atlas;          //アトラス情報
        public int type;            //種類
        public Material mat;        //マテリアル
        public int vNum;            //縦方向のアトラス数
        public int hNum;            //横方向のアトラス数
    }
    private AnimationInfo info;

    public enum FaceState
    {
        FACE_STATE_NORMAL = 0,
        FACE_STATE_WINK
    }
    public FaceState faceState;

    float winkCount;

    private void Awake()
    {
        faceMaterial = faceObj.GetComponent<SkinnedMeshRenderer>().materials[0];
        info.mat = faceMaterial;
        Texture texture = info.mat.mainTexture;
        info.textureSize.x = texture.width;
        info.textureSize.y = texture.height;
        info.atlas.width = atlasSize.x;
        info.atlas.height = atlasSize.y;
        info.vNum = (int)(info.textureSize.y / info.atlas.height);
        info.hNum = (int)(info.textureSize.x / info.atlas.width);
        winkCount = 0;
    }

    ///<summary>
    ///種類を変更
    ///</summary>
    ///<param name = "type">Type.</param>
    public void ChangeFace( FaceType type)
    {
        info.type = (int)type;

        // HACK: 座標を求める
        //       2017/12/06現在、0除算が発生したため、今後も同様のエラーが起きたときに気を付けること！
        info.atlas.x = ((int)type / info.vNum);
        info.atlas.y = ((int)type - (info.atlas.x * info.vNum));
        info.atlas.x *= info.atlas.width;
        info.atlas.y *= info.atlas.height;

        //UV座標計算
        Vector2 offset;
        offset = new Vector2(info.atlas.x / info.textureSize.x,
                            1.0f - (info.atlas.y / info.textureSize.y));

        //適用
        info.mat.SetTextureOffset("_MainTex", offset);
    }

    /// <summary>
    /// ウインク用関数
    /// 
    /// </summary>
    /// <param name="type"></param>
    public void Wink(FaceType type)
    {
        //info.type = (int)type;

        //座標を求める
        info.atlas.x = ((int)type / info.vNum);
        info.atlas.y = ((int)type - (info.atlas.x * info.vNum));
        info.atlas.x *= info.atlas.width;
        info.atlas.y *= info.atlas.height;

        //UV座標計算
        Vector2 offset;
        offset = new Vector2(info.atlas.x / info.textureSize.x,
                            1.0f - (info.atlas.y / info.textureSize.y));

        //適用
        info.mat.SetTextureOffset("_MainTex", offset);
    }


    // Use this for initialization
    void Start()
    {
        ChangeFace(faceType);
    }

    // Update is called once per frame
    void Update()
    {
        switch( faceState )
        {
            case FaceState.FACE_STATE_NORMAL:
                {
                    break;
                }
            case FaceState.FACE_STATE_WINK:
                {
                    winkCount += Time.deltaTime;
                    if (winkCount % 3 > 2.7f)
                    {
                        Wink(FaceType.FACE_TYPE_CLOSE_EYE);
                    }
                    else
                    {
                        ChangeFace(faceType);
                    }
                    break;
                }
        }
    }

    private void OnDestroy()
    {
        //初期顔に戻す
        ChangeFace(FaceType.FACE_TYPE_NORMAL);
    }
}

// HACK: ウインクから戻る表情はfaceTypeに設定されている表情