using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class MapManager : MonoBehaviour
{
    public static string SELECT_EPISODE_NAME;
    public static string SELECT_BATTLE_MODE = "Easy";

    public enum MapMode
    {
        Select,
        Option,
        Stop
    }
    [HideInInspector] public MapMode mapMode = MapMode.Select;
    public UIButtonManager btManager;

    [Header("各UIの親オブジェクト")]
    public GameObject select;
    public GameObject chapterSelect;
    public GameObject episodeSelect;
    public GameObject battleModeSelect;
    public GameObject citySelect;//街マップの親
    public GameObject options;
    public GameObject messagePanel;

    [Header("最初に選択状態にするボタン等")]
    public GameObject firstChapter;
    public GameObject firstSelect;
    public GameObject firstEpisode;
    public GameObject firstMode;
    public GameObject firstCity;//街マップ最初の選択
    public GameObject firstOption;

    public GameObject battleStartButton;
    public Text chapterName;

    [Header("BGMに使うAudioClip")]
    public AudioClip mapBgm;

    private string selectTextName;
    
    private void Awake()
    {
        if(PlayerPrefsCommon.BOOKS_DATA.Count == 0 || PlayerPrefsCommon.MATERIALS_DATA.Count == 0 )
        {
            //保存データが無ければセーブデータ読み込み
            PlayerPrefsCommon.SaveFilesLoad();
            Debug.Log("データ読み込み");
        }
        if (PlayerPrefsCommon.BooksPlayData.Count == 0 || PlayerPrefsCommon.MaterialsPlayData.Count == 0)
        {
            //プレイデータが空なら読み込む
            PlayerPrefsCommon.PlaydataNewLoad();
            Debug.Log("プレイデータ読み込み");
        }
    }
    void Start()
    {
        AudioManager2D.Instance.AudioBgm.clip = mapBgm;
        AudioManager2D.Instance.AudioBgm.Play();
        ActiveSelectChange(true);
    }

    private void Update()
    {
        if(mapMode == MapMode.Select)
        {
            //O Keyでオプション表示
            if (Keyboard.current.oKey.wasPressedThisFrame && !options.activeInHierarchy)
            {
                UIButtonManager.actBtnMode = ActiveButtonMode.Pose;
                mapMode = MapMode.Option;
                options.SetActive(true);
                EventSystem.current.SetSelectedGameObject(firstOption);
            }
        }
        
        if (mapMode == MapMode.Option)
        {
            //backSpaceキーでオプションを閉じる
            if (Keyboard.current.backspaceKey.wasPressedThisFrame && options.activeInHierarchy)
            {
                UIButtonManager.actBtnMode = ActiveButtonMode.Play;
                mapMode = MapMode.Select;
                options.SetActive(false);
                EventSystem.current.SetSelectedGameObject(firstSelect);
            }
        }
        
    }
    /// <summary>
    /// モードセレクトとオプション制御
    /// </summary>
    /// <param name="isActive"></param>
    public void ActiveSelectChange(bool isActive)
    {
        if (isActive)
        {
            UIButtonManager.actBtnMode = ActiveButtonMode.Play;
            mapMode = MapMode.Select;
        }
        else
        {
            UIButtonManager.actBtnMode = ActiveButtonMode.Pose;
            mapMode = MapMode.Stop;
        }
        
    }
    /// <summary>
    /// マップの選択ボタンに設定するイベント
    /// </summary>
    public void SetOnClickEvent()
    {
        for(int i = 0; i < btManager.btControllers.Count; i++)
        {
            Button button = btManager.btControllers[i].button;
            switch (i)
            {
                case 0:
                    button.onClick.AddListener(() => { OnClickCity(); }) ;
                    
                    break;
                case 1:
                    button.onClick.AddListener(() => { OnClickBattle(); });
                    break;
                case 2:
                    button.onClick.AddListener(() => { OnClickStory(); });
                    break;
            }
        }
    }
    /// <summary>
    /// ストーリーモード選択
    /// </summary>
    public void OnClickStory()
    {
        ActiveSelectChange(false);
        select.SetActive(false);
        chapterSelect.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstChapter);
    }
    /// <summary>
    /// ストーリーのチャプター選択時
    /// </summary>
    /// <param name="SelectchapterName"></param>
    public void OnClickChapter(Text SelectchapterName)
    {
        chapterSelect.SetActive(false);
        episodeSelect.SetActive(true);
        chapterName.text = SelectchapterName.text;
        EventSystem.current.SetSelectedGameObject(firstEpisode);
    }
    /// <summary>
    /// ストーリーの第何話か選択したとき
    /// </summary>
    /// <param name="buttonText"></param>
    public void OnClickEpisode(Text buttonText)
    {
        selectTextName = "Episode1";
        SELECT_EPISODE_NAME = buttonText.text;
        AudioManager2D.Instance.AudioBgm.Stop();
        SceneManager.LoadScene(selectTextName);
    }
    
    /// <summary>
    /// 戻るボタンを押したとき
    /// </summary>
    public void OnClickBack()
    {
        //何話選択時なら
        if (episodeSelect.activeInHierarchy)
        {
            episodeSelect.SetActive(false);
            chapterSelect.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstChapter);
        }
        else if (chapterSelect.activeInHierarchy)//ストーリーチャプター選択画面なら
        {
            chapterSelect.SetActive(false);
            select.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelect);
            ActiveSelectChange(true);
        }
        else if (battleModeSelect.activeInHierarchy)//バトルモード選択画面なら
        {
            battleModeSelect.SetActive(false);
            select.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelect);
            ActiveSelectChange(true);
        }
        else if (citySelect.activeInHierarchy)//街マップにいるなら
        {
            citySelect.SetActive(false);
            select.SetActive(true);//前のマップに戻る
            ActiveSelectChange(true);
        }
    }
    /// <summary>
    /// バトル選択時
    /// </summary>
    public void OnClickBattle()
    {
        ActiveSelectChange(false);
        battleModeSelect.SetActive(true);
        select.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstMode);
    }
    //バトルモード選択時
    public void OnClickBattleMode(GameObject gameObject)
    {
        SELECT_BATTLE_MODE = gameObject.name;
        battleStartButton.SetActive(true);
    }
    /// <summary>
    /// 確認画面でスタート押下時
    /// </summary>
    public void OnClickStart()
    {
        AudioManager2D.Instance.AudioBgm.Stop();
        if (battleModeSelect.activeInHierarchy)
        {
            SceneManager.LoadScene("Battle");
        }
        
    }
    //タイトルバック選択時
    public void OnClickTitle()
    {
        AudioManager2D.Instance.AudioBgm.Stop();
        SceneManager.LoadScene("Title");
    }
    
    /// <summary>
    /// 街へ移動する
    /// </summary>
    public void OnClickCity()
    {
        ActiveSelectChange(false);
        mapMode = MapMode.Stop;
        //街マップを見えるように
        citySelect.SetActive(true);
        select.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstCity);
    }
    //強化モード選択時
    public void OnClickReinforcement()
    {
        AudioManager2D.Instance.AudioBgm.Stop();
        SceneManager.LoadScene("Reinforcement");
    }
    /// <summary>
    /// ショップへ移動
    /// </summary>
    public void OnClickShop()
    {

    }
    public void OnClickStatus(GameObject StatusPanel)
    {
        mapMode = MapMode.Stop;
        Text hp = StatusPanel.transform.Find("backImage/PlayerHp/HpText").gameObject.GetComponent<Text>();
        Text mp = StatusPanel.transform.Find("backImage/PlayerHp/Mp/MpText").gameObject.GetComponent<Text>();
        Text gord = StatusPanel.transform.Find("backImage/GordText").gameObject.GetComponent<Text>();
        hp.text = string.Format("HP:{0}", PlayerStatus.PLAYER_HP);
        mp.text = string.Format("MP:{0}", PlayerStatus.PLAYER_MP);
        gord.text = string.Format("${0}", PlayerStatus.Gord);
        StatusPanel.SetActive(true);
        StartCoroutine(BackPanelCoroutine(StatusPanel, null));
    }

    public void OnMessagePanel(string message)
    {
        mapMode = MapMode.Stop;
        messagePanel.SetActive(true);
        Text messagetxt = messagePanel.transform.Find("Text").GetComponent<Text>();
        Action act = () => {
            messagetxt.text = message;
            StartCoroutine(BackPanelCoroutine(messagePanel, null));
        };
        StartCoroutine(MyCoroutine.Delay(3, act));
        
    }
    public IEnumerator BackPanelCoroutine(GameObject backPanel,GameObject nextPanel)
    {
        while (!Keyboard.current.backspaceKey.isPressed)yield return null;
        if(backPanel != null)backPanel.SetActive(false);
        if(nextPanel != null)nextPanel.SetActive(true);
        mapMode = MapMode.Option;
    }

}
