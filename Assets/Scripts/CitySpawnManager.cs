using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySpawnManager : MonoBehaviour {

    //スポーンポイントアタッチ
    public GameObject spawnPointPrefab;

    //スポーンポイントの場所
    public Vector3[] spawnPoint;

    /// <summary>
    /// スポーンポイント数。
    /// 初期化時にスポーンポイントの場所を管理する配列から数を取得。
    /// </summary>
    public int SpawnNum
    {
        get { return spawnNum; }
    }

    int spawnNum;

    //スポーンポイントのゲームオブジェクト保存
    SpawnPoint[] spawnPointObject;

    //乗車人数
    public int pearPassengerNum;
    public int smallPassengerNum;
    public int bigPassengerNum;

    // 初期化
    void Start()
    {
		spawnNum = spawnPoint.Length;
        //スポーンポイント生成
        spawnPointObject = new SpawnPoint[spawnNum];

        for (int i = 0; i < spawnNum; i++)
        {
            SpawnPointCreate(i, spawnPoint[i]);
        }
 
    }

    // 更新
    void Update()
    {
        if (Input.GetKeyDown("0"))
        {
            HumanCreate(0,Human.GROUPTYPE.PEAR);
        }
        if (Input.GetKeyDown("1"))
        {
            HumanCreate(1,Human.GROUPTYPE.SMAlLL);
        }
        if (Input.GetKeyDown("2"))
        {
            HumanCreate(2,Human.GROUPTYPE.BIG);
        }
        if (Input.GetKeyDown("5"))
        {
            SpawnHumanGroup(0, Human.GROUPTYPE.BIG);
        }
    }

    /*****************************************************************************
    * 関数名:SpawnPointCreate
    * 引数：num:番号
    * 引数:position:位置
    * 戻り値:0
    * 説明:スポーンポイントを生成
    *****************************************************************************/
    void SpawnPointCreate(int num, Vector3 position)
    {
        //生成
        GameObject SpawnPoint = Instantiate(spawnPointPrefab,                       //ゲームオブジェクト
                                               position,                            //位置
                                               Quaternion.identity) as GameObject;  //回転

        //自分の親を自分にする
        SpawnPoint.transform.parent = transform;

        //表示
        SpawnPoint.name = "Spown" + num;

        //生成したブロックを配列に保存
        spawnPointObject[num] = SpawnPoint.GetComponent<SpawnPoint>();
    }

    /// <summary>
    /// 乗客生成処理(個人)
    /// </summary>
    /// <param name="spawnPointNum">
    /// 生成する場所のID
    /// </param>
    /// <param name="groupType">
    /// 生成する乗客の所属グループタイプ
    /// </param>
    public void HumanCreate(int spawnPointNum , Human.GROUPTYPE groupType )
    {
        spawnPointObject[spawnPointNum].HumanSpawn( spawnPointNum , groupType);
    }

    /// <summary>
    /// 乗客生成処理(グループ)
    /// </summary>
    /// <param name="spawnPlace">
    /// 現在取得した人の場所番号。この場所は除外して考える。
    /// </param>
    /// <param name="groupType">
    /// 生成する乗客の所属グループタイプ
    /// </param>
    public void SpawnHumanGroup ( int spawnPlace , Human.GROUPTYPE groupType)
    {
        //相方たちの人数
        int passengerNum=0;

        //グループによって相方の人数を決める
        switch ( groupType )
        {
            //ペア
            case Human.GROUPTYPE.PEAR:
                passengerNum = pearPassengerNum - 1;
                break;

            //小グループ
            case Human.GROUPTYPE.SMAlLL:
                passengerNum = smallPassengerNum - 1;
                break;

            //大グループ
            case Human.GROUPTYPE.BIG:
                passengerNum = bigPassengerNum - 1;
                break;
        }

        //ランダム数
        int randam;

        //スポーンポイントに人が生成されているかどうか
        bool[] existPlace = new bool[spawnNum];

        //初期化
        for (int nCnt = 0; nCnt < spawnNum; nCnt++ )
        {
            existPlace[nCnt] = false;
        }

        existPlace[spawnPlace] = true;

        //相方生成を人数分生成
        for ( int nCnt = 0; nCnt < passengerNum; nCnt++ )
        {
            while(true)
            {

                bool bOut = false;
                randam = Random.Range(0, spawnNum);

                for (int i = 0; i < spawnNum; i++)
                {
                    if (existPlace[i] == false && i == randam)
                    {
                        existPlace[i] = true;
                        bOut = true;
                        break;
                    }
                }

                if (bOut)
                {
                    break;
                }
            }

            //人生成
            HumanCreate(randam,groupType);
        }
    }

    /// <summary>
    /// プレイヤーの乗物によって人を生成
    /// </summary>
    /// <param name="vehicleType">
    /// 現在のプレイヤーの乗物の種類
    /// </param>
    /// <param name="lastHumanSpawnPlace">
    /// 最後の乗客の場所
    /// </param>
    /// <param name="pearNum">
    /// ペアを何人生成するか
    /// </param>
    /// <param name="smallGroupNum">
    /// 小グループを何人生成するか
    /// </param>
    /// <param name="BigGroubNum">
    /// 大グループを何人生成するか
    /// </param>
    public void HumanCreateByVehicleType(Player.VehicleType vehicleType,int lastHumanSpawnPlace,int pearNum,int smallGroupNum,int BigGroubNum)
    {
        List<int> posList = new List<int>();
        posList.Add(lastHumanSpawnPlace);
        int pos;

        //乗物の種類によっての生成方法
        switch(vehicleType)
        {
            //バイクのとき
            case Player.VehicleType.VEHICLE_TYPE_BIKE:
                //ペア生成
                for ( int nCnt=0; nCnt < pearNum; nCnt++ )
                {
                    while(true)
                    {
                        pos = Random.Range(0, SpawnNum);

                        if (posList.IndexOf(pos) == -1 )
                        {
                            posList.Add(pos);
                            break;
                        }
                    }

                    // 人生成
                    HumanCreate(pos,Human.GROUPTYPE.PEAR);
                }
                break;

            //車のとき
            case Player.VehicleType.VEHICLE_TYPE_CAR:
                //ペア生成
                for (int nCnt = 0; nCnt < pearNum; nCnt++)
                {
                    while (true)
                    {
                        pos = Random.Range(0, SpawnNum);

                        if (posList.IndexOf(pos) == -1)
                        {
                            posList.Add(pos);
                            break;
                        }
                    }

                    // 人生成
                    HumanCreate(pos, Human.GROUPTYPE.PEAR);
                }
                //小グループ生成
                for (int nCnt = 0; nCnt < smallGroupNum; nCnt++)
                {
                    while (true)
                    {
                        pos = Random.Range(0, SpawnNum);

                        if (posList.IndexOf(pos) == -1)
                        {
                            posList.Add(pos);
                            break;
                        }
                    }

                    // 人生成
                    HumanCreate(pos, Human.GROUPTYPE.SMAlLL);
                }
                break;

            //バスのとき
            case Player.VehicleType.VEHICLE_TYPE_BUS:
                //ペア生成
                for (int nCnt = 0; nCnt < pearNum; nCnt++)
                {
                    while (true)
                    {
                        pos = Random.Range(0, SpawnNum);

                        if (posList.IndexOf(pos) == -1)
                        {
                            posList.Add(pos);
                            break;
                        }
                    }

                    // 人生成
                    HumanCreate(pos, Human.GROUPTYPE.PEAR);
                }
                //小グループ生成
                for (int nCnt = 0; nCnt < smallGroupNum; nCnt++)
                {
                    while (true)
                    {
                        pos = Random.Range(0, SpawnNum);

                        if (posList.IndexOf(pos) == -1)
                        {
                            posList.Add(pos);
                            break;
                        }
                    }

                    // 人生成
                    HumanCreate(pos, Human.GROUPTYPE.SMAlLL);
                }
                //大グループ生成
                for (int nCnt = 0; nCnt < BigGroubNum; nCnt++)
                {
                    while (true)
                    {
                        pos = Random.Range(0, SpawnNum);

                        if (posList.IndexOf(pos) == -1)
                        {
                            posList.Add(pos);
                            break;
                        }
                    }

                    // 人生成
                    HumanCreate(pos, Human.GROUPTYPE.BIG);
                }

                break;
        }
    }
}
