using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    //どのセーブデータを開いているか
    public static int SELECT_DATA_NUMBER = 0;

    public AudioClip titleBgm;
    public GameObject DefaultObj;
    public GameObject okbutton;
    public GameObject startbutton;
    public GameObject DeleatOption;
    public GameObject ContinueButton;
    public GameObject DataDreateButton;
    [Header("SaveData選択画面")]
    public GameObject saveData;
    public GameObject savedatafirst;
    public GameObject saveDataOkbutton;
    public Text savedataText;

    public PlayerPrefsCommon playerPrefsCommon;
    [Header("作成するデータ数")]
    [SerializeField]private int createFileNum = 0;
    private int selectNumber = 0;
    private bool isCreate = false;
    private bool isFiles = false;
    private string[] firstfiles = new string[] { "1", "5", "10", "2", "5", "10", "3", "5", "10" };
    private string path = default;
    //private List<string[]> Itemsdata = new List<string[]>();
    // Start is called before the first frame update
    void Start()
    {
        path = Path.Combine(Application.persistentDataPath, string.Format("book_{0}.csv", TitleManager.SELECT_DATA_NUMBER));
        AudioManager2D.Instance.AudioBgm.clip = titleBgm;
        AudioManager2D.Instance.AudioBgm.Play();
        savedataText.text = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!File.Exists(path) && !isCreate)
        {
            ContinueButton.SetActive(false);
            DataDreateButton.SetActive(false);
            isCreate = true;
        }
        else if(File.Exists(path) && !isFiles)
        {
            ContinueButton.SetActive(true);
            DataDreateButton.SetActive(true);
            isFiles = true;
        }
       
    }
    /// <summary>
    /// スタートボタン選択
    /// </summary>
    public void OnStartClick()
    {
        //セーブファイルが作成されているか確認
        if (File.Exists(path))
        {
            //保存されているフラグON
            PlayerPrefs.SetInt("CreateFile", 1);
        }
        else
        {
            //セーブファイル作成
            playerPrefsCommon.CreateSaveFile(firstfiles,createFileNum);
            PlayerPrefs.SetInt("CreateFile", 1);
        }
        SELECT_DATA_NUMBER = 0;
        AudioManager2D.Instance.AudioBgm.Stop();
        SceneManager.LoadScene("Prologue");
    }
    /// <summary>
    /// 続きから始める選択
    /// </summary>
    public void OnContinueClick()
    {
        DefaultObj.SetActive(false);
        saveData.SetActive(true);
        EventSystem.current.SetSelectedGameObject(savedatafirst);
           
    }
    //セーブデータ選択ボタンを選択した場合
    public void OnClickSavedata(GameObject gameObject)
    {
        saveDataOkbutton.SetActive(true);
        for (int i = 1; i <= 3; i++)
        {
            if (gameObject.name == "Save" + i)
            {
                selectNumber = i;
                List<string[]> booksdata = playerPrefsCommon.BooksDataLoadTest(selectNumber);
                savedataText.text =
                    string.Format("Data：{0}\n\nITEM1＜ATK:{1} MP：{2}＞\n\nITEM2＜ATK:{3} MP：{4}＞\n\nITEM3＜ATK:{5} MP：{6}＞\n\n",
                    gameObject.name, booksdata[0][1], booksdata[0][2], booksdata[1][1], booksdata[1][2], booksdata[2][1], booksdata[2][2]);
                break;
            }
        }
        //savedataText.text = string.Format("セーブデータ{0}を開きますか？", SELECT_DATA_NUMBER);
        
    }
    //データ選択後の開始ボタンが押されたら
    public void OnClickDataStart()
    {
        SELECT_DATA_NUMBER = selectNumber;
        AudioManager2D.Instance.AudioBgm.Stop();
        SceneManager.LoadScene("Map");
    }
    //データ選択後の戻るボタンが選択されたら
    public void OnClickdefaultBack()
    {
        if (saveData.activeInHierarchy)
        {
            SELECT_DATA_NUMBER = 0;
            saveData.SetActive(false);
            saveDataOkbutton.SetActive(false);
            savedataText.text = null;
            DefaultObj.SetActive(true);
            EventSystem.current.SetSelectedGameObject(startbutton);
        }
    }
    //ゲーム終了ボタンが選択されたら
    public void OnOptionClick()
    {
        //ゲーム終了
        Application.Quit();
    }

    /// <summary>
    /// データ削除ボタン押下時
    /// </summary>
    public void OnClickDelete()
    {
        if (!DeleatOption.activeInHierarchy)
        {
            DefaultObj.SetActive(false);
            DeleatOption.SetActive(true);
            EventSystem.current.SetSelectedGameObject(okbutton);
        }
        
    }
    //確認画面でOK押下
    public void OnClickOkDeleat()
    {
        
        PlayerPrefs.DeleteAll();
        //データ初期化
        playerPrefsCommon.CreateSaveFile(firstfiles,createFileNum);
        DeleatOption.SetActive(false);
        DefaultObj.SetActive(true);
        EventSystem.current.SetSelectedGameObject(startbutton);
    }
    //確認画面でNO押下
    public void OnClickNoButton()
    {
        DeleatOption.SetActive(false);
        DefaultObj.SetActive(true);
        EventSystem.current.SetSelectedGameObject(startbutton);
    }
}
