using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawnManager : MonoBehaviour
{

    //スポーンポイントアタッチ
    public GameObject spawnPointPrefab;

    ////スポーンポイントの場所
    //public Vector3[] spawnPoint;
    GameObject landmarkObj;
    public string landmarkPath;

    GameObject earthObj;
    public string earthPath;

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
       // Init();
    }

    public void Init()
    {
        earthObj = GameObject.Find(earthPath);
        //ランドマークObj取得
        landmarkObj = GameObject.Find(landmarkPath);

        //子のリストを作成
        var landChild = landmarkObj.GetComponentInChildren<Transform>();
        //スポーンポイント生成
        spawnPointObject = new SpawnPoint[landmarkObj.transform.childCount];
        spawnNum = landmarkObj.transform.childCount;
        int count = 0;
        foreach (Transform child in landChild)
        {
            SpawnPointCreate(count, child);
            count++;
        }
        HumanCreate(0, Human.GROUPTYPE.PEAR,SpawnPoint.PASSENGER_ORDER.FIRST);
    }

    // 更新
    void Update()
    {
        if (Input.GetKeyDown("0"))
        {
            HumanCreate(0, Human.GROUPTYPE.PEAR, SpawnPoint.PASSENGER_ORDER.FIRST);
        }
        if (Input.GetKeyDown("1"))
        {
            HumanCreate(1, Human.GROUPTYPE.SMAlLL, SpawnPoint.PASSENGER_ORDER.FIRST);
        }
        if (Input.GetKeyDown("2"))
        {
            HumanCreate(2, Human.GROUPTYPE.BIG, SpawnPoint.PASSENGER_ORDER.FIRST);
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
    ******************************************************************************
    *2017/11/24
    *佐藤峻一：ランドマークオブジェクト作成処理を追加
    *****************************************************************************/
    void SpawnPointCreate(int num, Transform trans)
    {
        

        //生成
        GameObject SpawnPoint = Instantiate(spawnPointPrefab,                       //ゲームオブジェクト
                                               trans.position,                            //位置
                                               trans.rotation) as GameObject;  //回転
        Vector3 upVec = SpawnPoint.transform.position - earthObj.transform.position;
        SpawnPoint.transform.up = upVec.normalized;

        //自分の親を自分にする
        SpawnPoint.transform.parent = transform;

        //表示
        SpawnPoint.name = "Spown" + num;

        //生成したブロックを配列に保存
        spawnPointObject[num] = SpawnPoint.GetComponent<SpawnPoint>();

        /*******ここから先が追加分*/
        // ランドマークオブジェクトを生成
        GameObject landMark = Resources.Load("Prefabs/Game/LandMarkObject/LandMark" + num) as GameObject;

        // プレハブからインスタンスを生成
        GameObject landMarkSet = Instantiate(landMark, new Vector3(trans.position.x, trans.position.y, trans.position.z), trans.rotation);

        // 親オブジェクトをスポーンポイントに設定
        landMarkSet.transform.parent = SpawnPoint.transform;
        /****追加分終了*****/
        /*******ここから先が追加分
         * */
        //string path = "/Assets/Resources/Prefabs/Game/LandMarkObject/LandMark" + num;
        //GameObject landMark = GameObject.CreatePrimitive()
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
    public void HumanCreate(int spawnPointNum, Human.GROUPTYPE groupType,SpawnPoint.PASSENGER_ORDER passengerOrder )
    {
        spawnPointObject[spawnPointNum].HumanSpawn(spawnPointNum, groupType , passengerOrder);
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
    public void SpawnHumanGroup(int spawnPlace, Human.GROUPTYPE groupType)
    {
        //相方たちの人数
        int passengerNum = 0;

        //グループによって相方の人数を決める
        switch (groupType)
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
        for (int nCnt = 0; nCnt < spawnNum; nCnt++)
        {
            existPlace[nCnt] = false;
        }

        existPlace[spawnPlace] = true;

        //相方生成を人数分生成
        for (int nCnt = 0; nCnt < passengerNum; nCnt++)
        {
            while (true)
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
            HumanCreate(randam, groupType,SpawnPoint.PASSENGER_ORDER.DEFOULT);
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
    public void HumanCreateByVehicleType( PlayerVehicle.Type vehicleType, int lastHumanSpawnPlace, int pearNum, int smallGroupNum, int BigGroubNum)
    {
        List<int> posList = new List<int>();
        posList.Add(lastHumanSpawnPlace);
        int pos;

        //乗物の種類によっての生成方法
        switch (vehicleType)
        {
            //バイクのとき
            case PlayerVehicle.Type.BIKE:
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
                    HumanCreate(pos, Human.GROUPTYPE.PEAR,SpawnPoint.PASSENGER_ORDER.FIRST);
                }
                break;

            //車のとき
            case PlayerVehicle.Type.CAR:
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
                    HumanCreate(pos, Human.GROUPTYPE.PEAR, SpawnPoint.PASSENGER_ORDER.FIRST);
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
                    HumanCreate(pos, Human.GROUPTYPE.SMAlLL, SpawnPoint.PASSENGER_ORDER.FIRST);
                }
                break;

            //バスのとき
            case PlayerVehicle.Type.BUS:
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
                    HumanCreate(pos, Human.GROUPTYPE.PEAR, SpawnPoint.PASSENGER_ORDER.FIRST);
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
                    HumanCreate(pos, Human.GROUPTYPE.SMAlLL, SpawnPoint.PASSENGER_ORDER.FIRST);
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
                    HumanCreate(pos, Human.GROUPTYPE.BIG, SpawnPoint.PASSENGER_ORDER.FIRST);
                }

                break;
            //飛行機のとき
            case PlayerVehicle.Type.AIRPLANE:
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
                    HumanCreate(pos, Human.GROUPTYPE.PEAR, SpawnPoint.PASSENGER_ORDER.FIRST);
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
                    HumanCreate(pos, Human.GROUPTYPE.SMAlLL, SpawnPoint.PASSENGER_ORDER.FIRST);
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
                    HumanCreate(pos, Human.GROUPTYPE.BIG, SpawnPoint.PASSENGER_ORDER.FIRST);
                }

                break;
        }
    }
}
