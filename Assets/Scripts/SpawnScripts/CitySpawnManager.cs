using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySpawnManager : MonoBehaviour {

    [SerializeField] string spawnPointListPath;
    private GameObject spawnPointListObj;

    /// <summary>
    /// スポーンポイント数。
    /// 初期化時にスポーンポイントの場所を管理する配列から数を取得。
    /// </summary>
    public int SpawnNum { get; private set; }

    //スポーンポイントのゲームオブジェクト保存
    List< SpawnPoint > spawnPointObject;

    //乗車人数
    public int pearPassengerNum;
    public int smallPassengerNum;
    public int bigPassengerNum;

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        SpawnNum = 0;
    }

    // 初期化
    void Start()
    {
        // リストに登録されたスポーンポイントオブジェクトをすべて取得して蓄えておく
        spawnPointListObj = GameObject.Find( spawnPointListPath );
        spawnPointObject = new List<SpawnPoint>();

        foreach( Transform child in spawnPointListObj.transform )
        {
            SpawnPoint point = child.gameObject.GetComponent<SpawnPoint>();
            spawnPointObject.Add( point );
            SpawnNum++;
        }
        
        if( SpawnNum == 0 )
        {
            Debug.LogError( "スポーンポイント取得失敗" );
        }

        //初回スポーン
        HumanCreate(1, PassengerController.GROUPTYPE.Lovers, SpawnPoint.PASSENGER_ORDER.FIRST);
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
    /// <param name="passengerOrder">
    /// 乗客の乗車順番
    /// </param>
    public void HumanCreate(int spawnPointNum , PassengerController.GROUPTYPE groupType , SpawnPoint.PASSENGER_ORDER passengerOrder , int modelType = -1 )
    {
        spawnPointObject[spawnPointNum].PassengerSpawn( spawnPointNum , groupType , passengerOrder , modelType );
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
    public void SpawnHumanGroup ( int spawnPlace , PassengerController.GROUPTYPE groupType , Human.ModelType alreadyCreateModel )
    {
        //相方たちの人数
        int passengerNum=0;

        //グループによって相方の人数を決める
        switch ( groupType )
        {
            //ペア
            case PassengerController.GROUPTYPE.Lovers:
                passengerNum = pearPassengerNum - 1;
                break;

            //小グループ
            case PassengerController.GROUPTYPE.Family:
                passengerNum = smallPassengerNum - 1;
                break;

            //大グループ
            case PassengerController.GROUPTYPE.Friends:
                passengerNum = bigPassengerNum - 1;
                break;
        }

        //ランダム数
        int randam;

        //スポーンポイントに人が生成されているかどうか
        bool[] existPlace = new bool[SpawnNum];

        //初期化
        for (int nCnt = 0; nCnt < SpawnNum; nCnt++ )
        {
            existPlace[nCnt] = false;
        }

        existPlace[spawnPlace] = true;

        // 生成済み乗客を調査
        bool[] modelAlredyCreate = null;
        int index = 0;

        switch( groupType )
        {
            //ペア
            case PassengerController.GROUPTYPE.Lovers:
                modelAlredyCreate = new bool[ 2 ];

                index = ( alreadyCreateModel - Human.ModelType.LoversSt );

                modelAlredyCreate[ index ] = true;
                break;

            //小グループ
            case PassengerController.GROUPTYPE.Family:
                modelAlredyCreate = new bool[ 3 ];

                index = ( alreadyCreateModel - Human.ModelType.FamilySt );

                modelAlredyCreate[ index ] = true;
                break;

            //大グループ
            case PassengerController.GROUPTYPE.Friends:
                modelAlredyCreate = new bool[ 5 ];

                index = ( alreadyCreateModel - Human.ModelType.FriendsSt );

                modelAlredyCreate[ index ] = true;
                break;
        }

        //相方生成を人数分生成
        for( int nCnt = 0; nCnt < passengerNum; nCnt++ )
        {
            while(true)
            {

                bool bOut = false;
                randam = Random.Range(0, SpawnNum);

                for (int i = 0; i < SpawnNum; i++)
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

            // 生成するモデルを設定
            int randomSt = 0;
            int randomEnd = 0;
            int modelID = -1;

            switch( groupType )
            {
                //ペア
                case PassengerController.GROUPTYPE.Lovers:
                    randomSt = ( int )Human.ModelType.LoversSt;
                    randomEnd = ( int )Human.ModelType.LoversEnd + 1;
                    break;

                //小グループ
                case PassengerController.GROUPTYPE.Family:
                    randomSt = ( int )Human.ModelType.FamilySt;
                    randomEnd = ( int )Human.ModelType.FamilyEnd + 1;
                    break;

                //大グループ
                case PassengerController.GROUPTYPE.Friends:
                    randomSt = ( int )Human.ModelType.FriendsSt;
                    randomEnd = ( int )Human.ModelType.FriendsEnd + 1;
                    break;
            }

            while(true)
            {
                modelID = Random.Range( randomSt , randomEnd );

                int refIdx = modelID - randomSt;

                if( modelAlredyCreate[ refIdx ] == false )
                {
                    modelAlredyCreate[ refIdx ] = true;
                    break;
                }
            }

            switch( groupType )
            {
                //ペア
                case PassengerController.GROUPTYPE.Lovers:
                    passengerNum = pearPassengerNum - 1;
                    break;

                //小グループ
                case PassengerController.GROUPTYPE.Family:
                    passengerNum = smallPassengerNum - 1;
                    break;

                //大グループ
                case PassengerController.GROUPTYPE.Friends:
                    passengerNum = bigPassengerNum - 1;
                    break;
            }

            //人生成
            HumanCreate(randam,groupType, SpawnPoint.PASSENGER_ORDER.DEFOULT, modelID );
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
    public void HumanCreateByVehicleType(PlayerVehicle.Type vehicleType,int lastHumanSpawnPlace,int pearNum,int smallGroupNum,int BigGroubNum)
    {
        List<int> posList = new List<int>();
        posList.Add(lastHumanSpawnPlace);
        int pos;

        //乗物の種類によっての生成方法
        switch(vehicleType)
        {
            //バイクのとき
            case PlayerVehicle.Type.BIKE:
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
                    HumanCreate(pos, PassengerController.GROUPTYPE.Lovers,SpawnPoint.PASSENGER_ORDER.FIRST);
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
                    HumanCreate(pos, PassengerController.GROUPTYPE.Lovers, SpawnPoint.PASSENGER_ORDER.FIRST);
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
                    HumanCreate(pos, PassengerController.GROUPTYPE.Family, SpawnPoint.PASSENGER_ORDER.FIRST);
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
                    HumanCreate(pos, PassengerController.GROUPTYPE.Family, SpawnPoint.PASSENGER_ORDER.FIRST);
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
                    HumanCreate(pos, PassengerController.GROUPTYPE.Family, SpawnPoint.PASSENGER_ORDER.FIRST);
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
                    HumanCreate(pos, PassengerController.GROUPTYPE.Friends, SpawnPoint.PASSENGER_ORDER.FIRST);
                }

                break;
        }
    }
}
