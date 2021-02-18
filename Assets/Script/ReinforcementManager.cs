using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.IO;

public class ReinforcementManager : MonoBehaviour
{
    //public static List<string[]> newbookdatas = new List<string[]>();

    [Header("各ウィンドウ")]
    public GameObject ItemSelect;//所持アイテム選択ウィンドウの親
    public GameObject StoneSelect;//素材選択ウィンドウの親
    public GameObject resultItem;//強化結果ウィンドウの親
    public GameObject SelectSaveData;//セーブデータ選択ウィンドウの親

    [Header("変更するテキスト")]
    public Text selectText;
    public Text selectItemText;
    public Text previewText;
    public Text resultMaterialText;
    public Text saveSelectText;

    [Header("最初に選択されるボタン")]
    public GameObject itemfirstObj;
    public GameObject itemOkObj;
    public GameObject stonefirstObj;
    public GameObject materialfirstObj;
    public GameObject resultfirstObj;
    public GameObject saveSelectFirst;
    public GameObject saveSelectOk;

    public AudioClip audioBgm;
    public PlayerPrefsCommon playerPrefsCommon;
    private int selectNumber = 0;
    private int MaterialNumber = 0;
    private float selectAtk = 0;
    private float selectMp = 0;
    private float selectItemType = 0; 
    private float selectItemAtk = 0;
    private float selectItemMp = 0;
    private float nextAtk = 0;
    private float nextMp = 0;
    private string typename = "";
    //private List<string[]> bookdatas = new List<string[]>();
    
    private void Awake()
    {
        if (TitleManager.SELECT_DATA_NUMBER == 0)
        {
            ItemSelect.SetActive(false);
            saveSelectText.text = null;
            SelectSaveData.SetActive(true);//ウィンドウ開く
            saveSelectOk.SetActive(false);//選択ボタンは見えないように
            EventSystem.current.SetSelectedGameObject(saveSelectFirst);
        }
        else
        {
            ItemSelect.SetActive(true);
            EventSystem.current.SetSelectedGameObject(itemfirstObj);
            playerPrefsCommon.ItemsDataLoad();
        }
        
        //string bookdataPath = Path.Combine(Application.persistentDataPath, "book_1.csv");
        //using (var sw = new StreamReader(bookdataPath))
        //{
        //    while (sw.Peek() != -1)
        //    {
        //        string line = sw.ReadLine();
        //        bookdatas.Add(line.Split(','));
        //    }
        //} 
    }
    // Start is called before the first frame update
    void Start()
    {
        //初期化処理
        selectItemText.text = null;
        selectText.text = null;
        previewText.text = null;
        resultMaterialText.text = null;
        //Audioの設定
        AudioManager2D.Instance.AudioBgm.clip = audioBgm;
        AudioManager2D.Instance.AudioBgm.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 強化する所持アイテムの選択
    /// </summary>
    /// <param name="gameObject"></param>
    public void OnClickItemSelect(GameObject gameObject)
    {
        itemOkObj.SetActive(true);
        for(int num = 1; num <= 3; num++)
        {
            if(gameObject.name == "Item"+ num)
            {
                selectNumber = num - 1;
                selectText.text = string.Format("アイテム{0}でいいですか?", num);
                ItemStatusGet(num);

            }
        }
        
    }
    //
    public void OnClickPreview(GameObject gameObject)
    {
        
        for(int i = 1; i <= 10; i++)
        {
            if(gameObject.name == "Material" + i)
            {
                if (PlayerPrefs.HasKey(string.Format("MaterialATK{0}_{1}", TitleManager.SELECT_DATA_NUMBER, i)))
                {
                    MaterialNumber = i;
                    selectAtk = PlayerPrefs.GetFloat(string.Format("MaterialATK{0}_{1}", TitleManager.SELECT_DATA_NUMBER, i));
                    selectMp = PlayerPrefs.GetFloat(string.Format("MaterialMP{0}_{1}", TitleManager.SELECT_DATA_NUMBER, i));
                    previewText.text = string.Format("ATK：{0}\nMP：{1}", selectAtk, selectMp);
                    stonefirstObj.SetActive(true);
                }
                else
                {
                    if (stonefirstObj.activeInHierarchy)
                    {
                        stonefirstObj.SetActive(false);
                    }
                    previewText.text = "素材がありません";
                }
            }
        }
        
    }
    /// <summary>
    /// 各ウィンドウの確認画面処理
    /// </summary>
    public void OnClickOk()
    {
        //所持アイテム選択画面が動的なら
        if (ItemSelect.activeInHierarchy)
        {
            ItemSelect.SetActive(false);
            StoneSelect.SetActive(true);
            EventSystem.current.SetSelectedGameObject(materialfirstObj);
            switch (selectItemType)
            {
                case 1:
                    typename = "Sky";
                    break;
                case 2:
                    typename = "MARINE";
                    break;
                case 3:
                    typename = "Earth";
                    break;
            }
            selectItemText.text = string.Format("TYPE：{0}\nATK：{1}\nMP：{2}",typename,selectItemAtk,selectItemMp);
        }
        else if (StoneSelect.activeInHierarchy)
        {
            nextAtk = selectItemAtk + selectAtk;
            nextMp = selectItemMp + selectMp;
            StoneSelect.SetActive(false);
            resultItem.SetActive(true);
            EventSystem.current.SetSelectedGameObject(resultfirstObj);
            resultMaterialText.text = string.Format("TYPE：{0}\nATK：{1}\nMP：{2}",typename, nextAtk, nextMp);
        }
        else if (resultItem.activeInHierarchy)
        {
            //データ保存処理
            string[] nextBooksStatus = NextItemStatus(nextAtk,nextMp);
            
            playerPrefsCommon.SavebookFile(nextBooksStatus);
            //素材データ削除
            PlayerPrefs.DeleteKey(string.Format("MaterialATK{0}_{1}", TitleManager.SELECT_DATA_NUMBER, MaterialNumber));
            PlayerPrefs.DeleteKey(string.Format("MaterialMP{0}_{1}", TitleManager.SELECT_DATA_NUMBER, MaterialNumber));
            resultItem.SetActive(false);
            ItemSelect.SetActive(true);
            itemOkObj.SetActive(false);
            selectText.text = null;
            EventSystem.current.SetSelectedGameObject(itemfirstObj);
        }
    }
    //Backボタン押下時
    public void OnClickBack()
    {
        if (SelectSaveData.activeInHierarchy)
        {
            saveSelectOk.SetActive(false);
            SelectSaveData.SetActive(false);
            AudioManager2D.Instance.AudioBgm.Stop();
            SceneManager.LoadScene("Map");
        }
        else if (ItemSelect.activeInHierarchy)
        {
            AudioManager2D.Instance.AudioBgm.Stop();
            SceneManager.LoadScene("Map");
        }
        else if (StoneSelect.activeInHierarchy)
        {
            StoneSelect.SetActive(false);
            ItemSelect.SetActive(true);
            EventSystem.current.SetSelectedGameObject(itemfirstObj);
        }
        else if (resultItem.activeInHierarchy)
        {
            resultItem.SetActive(false);
            StoneSelect.SetActive(true);
            EventSystem.current.SetSelectedGameObject(materialfirstObj);
        }
    }

    public void ItemStatusGet(int number)
    {
        selectItemType = float.Parse(PlayerPrefsCommon.ITEMS_DATA[number - 1][0]);
        selectItemAtk = float.Parse(PlayerPrefsCommon.ITEMS_DATA[number - 1][1]);
        selectItemMp = float.Parse(PlayerPrefsCommon.ITEMS_DATA[number - 1][2]);
        
    }

    public string[] NextItemStatus(float atk ,float mp)
    {
        string[] data = new string[9];
        int number = 0;
        for (int v = 0; v < 3; v++)
        {
            for(int h = 0; h < 3; h++)
            {
                if(v == selectNumber)
                {
                    data[number] = PlayerPrefsCommon.ITEMS_DATA[v][h];//タイプはそのまま
                    number++;
                    data[number] = string.Format("{0}",atk);
                    number++;
                    data[number] = string.Format("{0}",mp);
                    number++;
                    break;
                }
                data[number] = PlayerPrefsCommon.ITEMS_DATA[v][h];
                number++;
            }
        }
        return data;
    }
    /// <summary>
    /// 保存先を選択したとき
    /// </summary>
    /// <param name="gameObject"></param>
    public void OnClickSaveData(GameObject gameObject)
    {
        for(int s = 1; s <= 3; s++)
        {
            if(gameObject.name == string.Format("Save{0}",s))
            {
                selectNumber = s;
                List<string[]> selectdatas = playerPrefsCommon.ItemsDataLoadTest(selectNumber);
                saveSelectText.text =
                    string.Format("Data：{0}\n\nITEM1＜ATK:{1} MP：{2}＞\n\nITEM2＜ATK:{3} MP：{4}＞\n\nITEM3＜ATK:{5} MP：{6}＞\n\n",
                    gameObject.name,selectdatas[0][1],selectdatas[0][2],selectdatas[1][1], selectdatas[1][2], selectdatas[2][1], selectdatas[2][2]);
                break;
            }
            
        }
        saveSelectOk.SetActive(true);
    }
    //保存先を決定する
    public void OnClickSavedataSelect()
    {
        TitleManager.SELECT_DATA_NUMBER = selectNumber;
        selectNumber = 0;//初期化しておく
        saveSelectOk.SetActive(false);
        SelectSaveData.SetActive(false);
        ItemSelect.SetActive(true);
        EventSystem.current.SetSelectedGameObject(itemfirstObj);
        playerPrefsCommon.ItemsDataLoad();
    }
}
