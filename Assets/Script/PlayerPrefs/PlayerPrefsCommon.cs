using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerPrefsCommon : MonoBehaviour
{
    public static List<string[]> ITEMS_DATA = new List<string[]>();

    private StreamWriter sw;
    private ReinforcementManager reinforcementManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SavePlayer()
    {
        //HPセーブ
        PlayerPrefs.SetFloat("PlayerHP", PlayerStatus.PLAYER_HP);
        //MPセーブ
        PlayerPrefs.SetFloat("PlayerMP", PlayerStatus.PLAYER_MP);
    }
    //アイテムの保存
    public void SaveItem(float atk,float mp)
    {
        for(int i = 1; i <= 10; i++)
        {
            if (PlayerPrefs.HasKey(string.Format("MaterialATK{0}_{1}", TitleManager.SELECT_DATA_NUMBER, i)))
            {
                if(i == 10)
                {
                    PlayerPrefs.SetFloat(string.Format("MaterialATK{0}_{1}", TitleManager.SELECT_DATA_NUMBER, i), atk);
                }
            }
            else
            {
                PlayerPrefs.SetFloat(string.Format("MaterialATK{0}_{1}", TitleManager.SELECT_DATA_NUMBER, i), atk);
                break;
            }
           
        }
        for(int m = 1; m <= 10; m++)
        {
            if (PlayerPrefs.HasKey(string.Format("MaterialMP{0}_{1}", TitleManager.SELECT_DATA_NUMBER, m)))
            {
                if (m == 10)
                {
                    PlayerPrefs.SetFloat(string.Format("MaterialMP{0}_{1}", TitleManager.SELECT_DATA_NUMBER, m), mp);
                }
            }
            else
            {
                PlayerPrefs.SetFloat(string.Format("MaterialMP{0}_{1}", TitleManager.SELECT_DATA_NUMBER, m), mp);
                break;
            }
        }
    }
    //アイテムのデータ書き換え
    public void SavebookFile(string[] datas)
    {        
        string path = Path.Combine(Application.persistentDataPath, string.Format("book_{0}.csv",TitleManager.SELECT_DATA_NUMBER));
        
        sw = new StreamWriter(path, false);
        if(sw == null)
        {
           Debug.Log("nullです");
        }
        else
        {
            for (int i = 0; i < datas.Length; i += 3)
            {
                sw.WriteLine(string.Format("{0},{1},{2}", datas[i], datas[i + 1], datas[i + 2]));
            }
        }
        sw.Close();
    }
    //アイテムデータの読み込み
    public void ItemsDataLoad()
    {
        ITEMS_DATA = new List<string[]>();//初期化しておく
        string bookdata = Path.Combine(Application.persistentDataPath, string.Format("book_{0}.csv",TitleManager.SELECT_DATA_NUMBER));
        using (var sw = new StreamReader(bookdata))
        {
            while (sw.Peek() != -1)
            {
                string line = sw.ReadLine();
                ITEMS_DATA.Add(line.Split(','));
            }
        }
    }
    /// <summary>
    /// 保存データの中身だけ見たい場合に呼び出す
    /// </summary>
    /// <param name="dataNumber"></param>
    /// <returns></returns>
    public List<string[]> ItemsDataLoadTest(int dataNumber)
    {
        List<string[]> datas = new List<string[]>();//初期化しておく
        string bookdata = Path.Combine(Application.persistentDataPath, string.Format("book_{0}.csv", dataNumber));
        using (var sw = new StreamReader(bookdata))
        {
            while (sw.Peek() != -1)
            {
                string line = sw.ReadLine();
                datas.Add(line.Split(','));
            }
        }
        return datas;
    }

    public void CreateSaveFile(string[] firstStatus)
    {
        for(int n = 0; n <= 3; n++)
        {
            string path = Path.Combine(Application.persistentDataPath, string.Format("book_{0}.csv", n));

            StreamWriter sw_0 = new StreamWriter(path, false);
            if (sw_0 == null)
            {
                Debug.Log("nullです");
            }
            else
            {
                for (int i = 0; i < firstStatus.Length; i += 3)
                {
                    sw_0.WriteLine(string.Format("{0},{1},{2}", firstStatus[i], firstStatus[i + 1], firstStatus[i + 2]));
                }
            }
            sw_0.Close();
        }
        
    }
}
