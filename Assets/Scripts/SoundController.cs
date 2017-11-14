using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    public enum Sounds
    {
        PICTURE_TRANSITION = 0,     //画面遷移
        BRAKE_SOUND = 1,            //ブレーキ音
        TYPE_CHANGE = 2,            //車両の種類チェンジ
        DOOR_OPEN_BUS = 3,          //バスのドアオープン
        DOOR_OPEN_CAR = 4,          //車のドアオープン
        CREATING_PEAR = 5,          //ペア作成音
        CUSTOMER_ESCAPE = 6,        //住人が逃げる音
        BIKE_RUN = 7,               //バイク走行音
        CAR_RUN = 8,                //車走行音
        BUS_RUN = 9,                //バス走行音
        AIRPLANE_RUN = 10,          //飛行機飛行音
        ROCKET_LAUNCH = 11,         //ロケット発射音
        ROCKET_SITE_OPEN = 12,      //発射場開場音
        COLLISION_DONTMOVE = 13,    //衝突音（動かないやつ）
        COLLISION_SMALL = 14,       //衝突音(小)
        COLLISION_BIG = 15,         //衝突音（大）
        OPPOSITE_RUN_WARNING = 16,  //逆走警告音？？
        SEVERAL_COUNTS = 17,        //数字のカウント
        DECISIVE_SOUND = 18,        //決定音(システム)
        CANCEL_SOUND = 19,          //キャンセル音（システム）
        SELECT_SOUND = 20,          //選択音（システム）
        RESULT_MIDDLE_SOUND = 21,   //リザルト各項目表示
        RESULT_END_SOUND = 22       //リザルト最終結果表示
    }

    [SerializeField]
    private AudioClip[] soundArray;

    public AudioClip AudioClipCreate(Sounds type)
    {
        int num = (int)type;
        AudioClip clip;
        clip = soundArray[num];
        return clip;
    }
}
