using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public enum BattleMode
    {
        Player,
        Enemy,
        Lose,
        Win,
        BreakeTime,
        Escape
    }
    public BattleMode battleTurn;

    //所持アイテム能力
    [System.NonSerialized]
    public float itemType_1 = 1;
    [System.NonSerialized]
    public float itemType_2 = 1;
    [System.NonSerialized]
    public float itemType_3 = 1;

    [System.NonSerialized]
    public float itemATK_1 = 0;
    [System.NonSerialized]
    public float itemATK_2 = 0;
    [System.NonSerialized]
    public float itemATK_3 = 0;

    [System.NonSerialized]
    public float itemPOWER_1 = 0;
    [System.NonSerialized]
    public float itemPOWER_2 = 0;
    [System.NonSerialized]
    public float itemPOWER_3 = 0;


    [Header("バトルの状況を書くText")]
    public Text battleText;
    [Header("ステータスゲージ")]
    public Image playerHpGage;
    public Image playerMpGage;
    public Image enemyHpGage;
    [Header("ステータステキスト")]
    public Text playerHpText;
    public Text playerMpText;
    public Text enemyHpText;
    [Header("Itemゲームオブジェクトの親")]
    public GameObject items;
    public AudioClip battleBgm;
    [Header("Resultの設定ここから")]
    public GameObject resultObj;
    public Text resultText;
    public PlayerPrefsCommon playerPrefsCommon;
    public GameObject resultOkbutton;

    private int enemyType = 1;
    private float enemyHp = 0;
    private float enemyAtk = 0;
    private bool turnchenge = false;
    private PlayerStatus playerStatus;
    private void Awake()
    {
        //アイテムステータスの読み込み
        playerPrefsCommon.BooksDataLoad();

        itemType_1 = float.Parse(PlayerPrefsCommon.BOOKS_DATA[0][0]);
        itemType_2 = float.Parse(PlayerPrefsCommon.BOOKS_DATA[1][0]);
        itemType_3 = float.Parse(PlayerPrefsCommon.BOOKS_DATA[2][0]);

        itemATK_1 = float.Parse(PlayerPrefsCommon.BOOKS_DATA[0][1]);
        itemATK_2 = float.Parse(PlayerPrefsCommon.BOOKS_DATA[1][1]);
        itemATK_3 = float.Parse(PlayerPrefsCommon.BOOKS_DATA[2][1]);

        itemPOWER_1 = float.Parse(PlayerPrefsCommon.BOOKS_DATA[0][2]);
        itemPOWER_2 = float.Parse(PlayerPrefsCommon.BOOKS_DATA[1][2]);
        itemPOWER_3 = float.Parse(PlayerPrefsCommon.BOOKS_DATA[2][2]);

        int value = Random.Range(1, 3);
        enemyType = value;
        switch (MapManager.SELECT_BATTLE_MODE)
        {
            case "Easy":
                enemyHp = (float)EnemyStatus.EnemyEasy.HP;
                enemyAtk = (float)EnemyStatus.EnemyEasy.ATK;
                break;
            case "Normal":
                enemyHp = (float)EnemyStatus.EnemyNormal.HP;
                enemyAtk = (float)EnemyStatus.EnemyNormal.ATK;
                break;
            case "Hard":
                enemyHp = (float)EnemyStatus.EnemyHard.HP;
                enemyAtk = (float)EnemyStatus.EnemyHard.ATK;
                break;
        }  
    }
        // Start is called before the first frame update
    void Start()
    {
        AudioManager2D.Instance.AudioBgm.clip = battleBgm;
        AudioManager2D.Instance.AudioBgm.Play();
        battleTurn = BattleMode.Player;
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーのターンである場合
        if (battleTurn == BattleMode.Player)
        {
            if (!turnchenge)
            {
                if(PlayerStatus.PLAYER_MP <= 0)
                {
                    battleText.text = "MPが足りない！\n休んでMPの回復を待つことにした";
                    StartCoroutine(turnCoroutine(BattleMode.Enemy));
                    turnchenge = true;
                }
                else
                {
                    items.SetActive(true);
                    battleText.text = "自分のターン";
                    EventSystem.current.SetSelectedGameObject(items);
                    turnchenge = true;
                }
                
            }    
        }
        
        //敵のターン
        if(battleTurn == BattleMode.Enemy)
        {
            if (!turnchenge)
            {
                turnchenge = true;
                battleText.text = "敵のターン\n";
                PlayerStatus.PLAYER_HP -= enemyAtk;
                battleText.text += string.Format("敵の攻撃!\n{0}ダメージくらった…！\n",enemyAtk);
                if(PlayerStatus.PLAYER_HP <= 0)
                {
                    PlayerStatus.PLAYER_HP = 0;
                    battleText.text = "HPが0になってしまった...";
                    StartCoroutine(turnCoroutine(BattleMode.Lose));
                    turnchenge = true;
                }
                else
                {
                    StartCoroutine(turnCoroutine(BattleMode.Player));
                    turnchenge = true;
                }
                
            }
            
        }

        //勝利した場合
        if(battleTurn == BattleMode.Win)
        {
            if (!turnchenge)
            {
                turnchenge = true;
                resultObj.SetActive(true);
                EventSystem.current.SetSelectedGameObject(resultOkbutton);
                float newAtk = Mathf.Ceil(Random.Range(1, 7));//ランダムにアタックポイントをつける
                float newMp = Mathf.Ceil(Random.Range(1, 7));//ランダムにマジックポイントをつける
                resultText.text = string.Format("ステータス\nATK：{0}\nMP：{1}", newAtk, newMp);
                //割り振られた乱数を渡してセーブ
                playerPrefsCommon.SaveItem(newAtk, newMp);
            }
        }

        if(battleTurn == BattleMode.Lose)
        {
            if (!turnchenge)
            {
                battleText.text += "私は全力でその場から逃げた";
                StartCoroutine(WaitMapLoad());
                turnchenge = true;
            }
            
        }

        if(battleTurn == BattleMode.Escape)
        {
            AudioManager2D.Instance.AudioBgm.Stop();
            SceneManager.LoadScene("Map");
        }
    }

    public void FixedUpdate()
    {
        //ステータスゲージの変更
        playerHpGage.fillAmount = PlayerStatus.PLAYER_HP / 100;
        playerMpGage.fillAmount = PlayerStatus.PLAYER_MP / 100;
        enemyHpGage.fillAmount = enemyHp / 100;

        enemyHpText.text = string.Format("HP：{0}",enemyHp);
        playerHpText.text = string.Format("HP：{0}",PlayerStatus.PLAYER_HP);
        playerMpText.text = string.Format("MP：{0}",PlayerStatus.PLAYER_MP);
    }

    public void OnClick1()
    {
        items.SetActive(false);
        battleText.text += string.Format("\nアイテム1を選択\nモンスターに{0}のダメージ!!\n",itemATK_1);
        enemyHp -= itemATK_1;
        battleText.text += string.Format("MPを{0}消費した", itemPOWER_1); 
        PlayerStatus.PLAYER_MP -= itemPOWER_1;
        if(PlayerStatus.PLAYER_MP <= 0)
        {
            PlayerStatus.PLAYER_MP = 0;
        }
        if(enemyHp <= 0)
        {
            enemyHp = 0;
            battleText.text = "モンスターを倒した！";
            StartCoroutine(turnCoroutine(BattleMode.Win));

        }
        else
        {
            StartCoroutine(turnCoroutine(BattleMode.Enemy));
        }
        
    }

    public void OnClick2()
    {
        items.SetActive(false);
        battleText.text += string.Format("\nアイテム2を選択\nモンスターに{0}のダメージ!!\n", itemATK_2);
        enemyHp -= itemATK_2;
        battleText.text += string.Format("MPを{0}消費した", itemPOWER_2);
        PlayerStatus.PLAYER_MP -= itemPOWER_2;
        if (PlayerStatus.PLAYER_MP <= 0)
        {
            PlayerStatus.PLAYER_MP = 0;
        }
        if (enemyHp <= 0)
        {
            enemyHp = 0;
            battleText.text = "モンスターを倒した！";
            StartCoroutine(turnCoroutine(BattleMode.Win));
            

        }
        else
        {
            StartCoroutine(turnCoroutine(BattleMode.Enemy));
        }
    }

    public void OnClick3()
    {
        items.SetActive(false);
        battleText.text += string.Format("\nアイテム3を選択\nモンスターに{0}のダメージ!!\n", itemATK_3);
        enemyHp -= itemATK_3;
        battleText.text += string.Format("MPを{0}消費した", itemPOWER_3);
        PlayerStatus.PLAYER_MP -= itemPOWER_3;
        if (PlayerStatus.PLAYER_MP <= 0)
        {
            PlayerStatus.PLAYER_MP = 0;
        }
        if (enemyHp <= 0)
        {
            enemyHp = 0;
            battleText.text = "モンスターを倒した！";
            StartCoroutine(turnCoroutine(BattleMode.Win));

        }
        else
        {
            StartCoroutine(turnCoroutine(BattleMode.Enemy));
        }
    }

    public void OnClickQuit()
    {
        battleText.text = "私は逃げることを選択した";
        StartCoroutine(turnCoroutine(BattleMode.Escape));
    }

    public void OnClickResultButton()
    {
        StartCoroutine(WaitMapLoad());
    }
    public IEnumerator turnCoroutine(BattleMode mode)
    {
        while (!Keyboard.current.spaceKey.isPressed) yield return null;
        yield return new WaitForSeconds(1.0f);
        battleTurn = mode;
        if(PlayerStatus.PLAYER_MP == 0)
        {
            PlayerStatus.PLAYER_MP += 20;
        }
        turnchenge = false;
    }

    public IEnumerator WaitMapLoad()
    {
        yield return new WaitForSeconds(2f);
        AudioManager2D.Instance.AudioBgm.Stop();
        SceneManager.LoadScene("Map");
    }
}
