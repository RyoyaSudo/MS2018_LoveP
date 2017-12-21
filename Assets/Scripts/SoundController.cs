using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioStructsSe
{
    public AudioClip clip;
    [Range(0f,1f)]public float volume;
}

public class SoundController : MonoBehaviour {

    public enum SoundsSeType
    {
        PICTURE_TRANSITION = 0,     //画面遷移
        CITY_DRIVE_SOUND,
        PASSENGER_RIDE,
        PASSENGER_COMPLETE,
        BUMP_SMALL,
        BUMP_MIDDLE,
        BUMP_BIG,
        VEHICLE_BUMP_SMALL,
        VEHICLE_BUMP_MIDDLE,
        VEHICLE_BUMP_BIG,
        VEHICLE_CHANGE_BIKE,
        VEHICLE_CHANGE_CAR,
        VEHICLE_CHANGE_BUS,
        VEHICLE_CHANGE_AIRPLANE,
        TypeMax,
        


        //BRAKE_SOUND = 1,            //ブレーキ音
        //TYPE_CHANGE = 2,            //車両の種類チェンジ
        //DOOR_OPEN_BUS = 3,          //バスのドアオープン
        //DOOR_OPEN_CAR = 4,          //車のドアオープン
        //CREATING_PEAR = 5,          //ペア作成音
        //CUSTOMER_ESCAPE = 6,        //住人が逃げる音
        //BIKE_RUN = 7,               //バイク走行音
        //CAR_RUN = 8,                //車走行音
        //BUS_RUN = 9,                //バス走行音
        //AIRPLANE_RUN = 10,          //飛行機飛行音
        //ROCKET_LAUNCH = 11,         //ロケット発射音
        //ROCKET_SITE_OPEN = 12,      //発射場開場音
        //COLLISION_DONTMOVE = 13,    //衝突音（動かないやつ）
        //COLLISION_SMALL = 14,       //衝突音(小)
        //COLLISION_BIG = 15,         //衝突音（大）
        //OPPOSITE_RUN_WARNING = 16,  //逆走警告音？？
        //SEVERAL_COUNTS = 17,        //数字のカウント
        //DECISIVE_SOUND = 18,        //決定音(システム)
        //CANCEL_SOUND = 19,          //キャンセル音（システム）
        //SELECT_SOUND = 20,          //選択音（システム）
        //RESULT_MIDDLE_SOUND = 21,   //リザルト各項目表示
        //RESULT_END_SOUND = 22       //リザルト最終結果表示
    }


    [SerializeField] AudioStructsSe[] seList = new AudioStructsSe[(int)SoundsSeType.TypeMax];


    [SerializeField]
    private AudioClip[] soundArray;

    public AudioClip AudioClipCreate(SoundsSeType type)
    {
        int num = (int)type;
        AudioClip clip;
        clip = soundArray[num];
        return clip;
    }

    public void PlayOneShot(SoundsSeType type , AudioSource source )
    {
        AudioStructsSe se = seList[(int)type];

        source.volume = se.volume;
        source.PlayOneShot(se.clip);
    }

    public AudioStructsSe GetSeList(SoundsSeType type)
    {
        AudioStructsSe se = seList[(int)type];
        return se;
    }
}
