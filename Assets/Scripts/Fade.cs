using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;  //画面遷移を可能にする
using UnityEngine.UI;               //UIを使用可能にする

public class Fade : MonoBehaviour
{
    private const float FADE_SPEED = 0.025f; //遷移速度
    float speed;                             //透明化の速さ
    int senisaki;                            //画面遷移先
    private float alfa;                      //A値を操作するための変数
    private float red, green, blue;          //RGBを操作するための変数

    /******************************************************************************
    * 生成されたときの初期化
    ******************************************************************************/
    void Start()
    {
        senisaki = 1;
        speed = -FADE_SPEED;                    //遷移速度を設定
        red = GetComponent<Image>().color.r;    //赤
        green = GetComponent<Image>().color.g;  //緑
        blue = GetComponent<Image>().color.b;   //青
        alfa = 1.0f;                            //α値
    }

    /******************************************************************************
    * 更新
    ******************************************************************************/
    void Update()
    {
        if (senisaki != 0)
        {// 遷移先が決まっていたら

            // 色を設定する
            GetComponent<Image>().color = new Color(red, green, blue, alfa);

            // α値を変更
            alfa += speed;

            if (alfa >= 1.0)
            {// もしフェードが完全不透明になったら

                speed *= -1.0f;// α値の変更スピードを反転させる

                switch (senisaki)
                {// 画面を遷移させる

                    case 1:
                        {// タイトル画面に遷移する
                            SceneManager.LoadScene("Title");
                            break;
                        }
                    case 2:
                        {// ランキング画面に遷移する
                            SceneManager.LoadScene("Ranking");
                            break;
                        }
                    case 3:
                        {// チュートリアル画面に遷移する
                            SceneManager.LoadScene("Tutorial");
                            break;
                        }
                    case 4:
                        {// ゲームメイン画面に遷移する
                            SceneManager.LoadScene("Game");
                            break;
                        }
                    case 5:
                        {// リザルト画面に遷移する
                            SceneManager.LoadScene("Result");
                            break;
                        }
                    case 6:
                        {// コンティニュー画面に遷移する
                            SceneManager.LoadScene("Continue");
                            break;
                        }
                    case 7:
                        {// コンフィグ画面に遷移する
                            SceneManager.LoadScene("Config");
                            break;
                        }
                }
            }
            else if (alfa <= 0.0)
            {// もしフェードが完全透明になったら
                alfa = 0.0f; //α値を０にする
                speed = 0.0f;//変更速度をなくす
                senisaki = 0;//遷移先をなくす
            }
        }
    }

    /******************************************************************************
    * フェードイン完了後の遷移先を設定
    *
    * 引数：int gamenSenisaki（画面遷移先）
    *
    * 戻り値：なし
    ******************************************************************************/
    public void SetFadeIn(int gamenSenisaki)
    {
        senisaki = gamenSenisaki;               //フェードイン完了後の遷移先を設定
        speed = FADE_SPEED;                     //遷移速度を設定
    }
}
