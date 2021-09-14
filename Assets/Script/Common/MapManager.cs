using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MapManager : MonoBehaviour
{
    public static string SELECT_EPISODE_NAME;
    public static string SELECT_BATTLE_MODE = "Easy";

    public GameObject select;
    public GameObject chapterSelect;
    public GameObject episodeSelect;
    public GameObject battleModeSelect;
    public GameObject citySelect;//街マップの親
    public GameObject options;

    public GameObject firstChapter;
    public GameObject firstSelect;
    public GameObject firstEpisode;
    public GameObject firstMode;
    public GameObject firstCity;//街マップ最初の選択
    public GameObject firstOption;

    public GameObject battleStartButton;
    public Text chapterName;

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
        EventSystem.current.SetSelectedGameObject(firstSelect);
    }

    private void Update()
    {
        //O Keyでオプション表示
        if (Keyboard.current.oKey.wasPressedThisFrame && !options.activeInHierarchy)
        {
            options.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstOption);
        }
        if (Keyboard.current.backspaceKey.wasPressedThisFrame && options.activeInHierarchy)
        {
            options.SetActive(false);
            EventSystem.current.SetSelectedGameObject(firstSelect);
        }
    }

    private void FixedUpdate()
    {
        
    }
    /// <summary>
    /// ストーリーモード選択
    /// </summary>
    public void OnClickStory()
    {
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
        }
        else if (battleModeSelect.activeInHierarchy)//バトルモード選択画面なら
        {
            battleModeSelect.SetActive(false);
            select.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelect);
        }else if (citySelect.activeInHierarchy)//街マップにいるなら
        {
            citySelect.SetActive(false);
            select.SetActive(true);//前のマップに戻る
        }
    }
    /// <summary>
    /// バトル選択時
    /// </summary>
    public void OnClickBattle()
    {
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
        
        Text hp = StatusPanel.transform.Find("backImage/PlayerHp/HpText").gameObject.GetComponent<Text>();
        Text mp = StatusPanel.transform.Find("backImage/PlayerHp/Mp/MpText").gameObject.GetComponent<Text>();
        Text gord = StatusPanel.transform.Find("backImage/GordText").gameObject.GetComponent<Text>();
        hp.text = string.Format("HP:{0}", PlayerStatus.PLAYER_HP);
        mp.text = string.Format("MP:{0}", PlayerStatus.PLAYER_MP);
        gord.text = string.Format("${0}", PlayerStatus.Gord);
        StatusPanel.SetActive(true);
        StartCoroutine(BackPanelCoroutine(StatusPanel, null));
    }

    public IEnumerator BackPanelCoroutine(GameObject backPanel,GameObject nextPanel)
    {
        while (!Keyboard.current.backspaceKey.isPressed)
        {
            yield return null;
        }
        backPanel.SetActive(false);
        if(nextPanel != null)
        {
            nextPanel.SetActive(true);
        }
    }
}
