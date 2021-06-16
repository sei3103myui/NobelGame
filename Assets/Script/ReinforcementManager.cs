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
    private int selectAtk = 0;
    private int selectMp = 0;
    private int selectItemType = 0; 
    private int selectItemAtk = 0;
    private int selectItemMp = 0;
    private int nextAtk = 0;
    private int nextMp = 0;
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
            playerPrefsCommon.BooksDataLoad();
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
    public void OnClickItemSelect(int ItemNumber)
    {
        itemOkObj.SetActive(true);

        selectNumber = ItemNumber;
        selectText.text = string.Format("アイテム{0}でいいですか?", ItemNumber + 1);
       

    }
    public void OnItemPreview(int itemNumber)
    {
        if(PlayerPrefsCommon.MaterialsPlayData[itemNumber][0] == 0)
        {
            if (stonefirstObj.activeInHierarchy)
            {
                stonefirstObj.SetActive(false);
            }
            previewText.text = "素材がありません";   
        }
        else
        {
            selectAtk = PlayerPrefsCommon.MaterialsPlayData[itemNumber][0];
            selectMp = PlayerPrefsCommon.MaterialsPlayData[itemNumber][1];
            string type = "none";
            switch (PlayerPrefsCommon.MaterialsPlayData[itemNumber][2])
            {
                case 1:
                    type = "Sky";
                    break;
                case 2:
                    type = "Sea";
                    break;
                case 3:
                    type = "Earth";
                    break;
            }
            previewText.text = string.Format("ATK：{0}\nMP：{1}\nTYPE：{2}", selectAtk, selectMp, type);
            stonefirstObj.SetActive(true);//合成ボタン
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
            selectItemType = PlayerPrefsCommon.BooksPlayData[selectNumber][0];
            selectItemAtk = PlayerPrefsCommon.BooksPlayData[selectNumber][1];
            selectItemMp = PlayerPrefsCommon.BooksPlayData[selectNumber][2];
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
            PlayerPrefsCommon.BooksPlayData[selectNumber][1] = nextAtk;
            PlayerPrefsCommon.BooksPlayData[selectNumber][2] = nextMp;
            PlayerPrefsCommon.BookPlaydataStringFormat();

            //素材削除
            PlayerPrefsCommon.MaterialDataDelete(selectNumber);
            PlayerPrefsCommon.PlaydataStringFormat();

            resultItem.SetActive(false);
            itemOkObj.SetActive(false);
            ItemSelect.SetActive(true);
            
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


    public string[] NextItemStatus(float atk, float mp)
    {
        string[] data = new string[9];
        int number = 0;
        for (int v = 0; v < 3; v++)
        {
            for (int h = 0; h < 3; h++)
            {
                if (v == selectNumber)
                {
                    data[number] = PlayerPrefsCommon.BOOKS_DATA[v][h];//タイプはそのまま
                    number++;
                    data[number] = string.Format("{0}", atk);
                    number++;
                    data[number] = string.Format("{0}", mp);
                    number++;
                    break;
                }
                data[number] = PlayerPrefsCommon.BOOKS_DATA[v][h];
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
                List<string[]> selectdatas = playerPrefsCommon.BooksDataLoadTest(selectNumber);
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
        playerPrefsCommon.BooksDataLoad();
    }
}
