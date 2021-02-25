using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerPrefsCommon : MonoBehaviour
{
    public static List<string[]> BOOKS_DATA = new List<string[]>();
    public static List<string[]> MATERIALS_DATA = new List<string[]>();


    private string bookPath = default;
    private string materialPath = default;
    private StreamWriter sw;
    private ReinforcementManager reinforcementManager;

    //素材の名前
    private string[] materialName = { "素材1", "素材2", "素材3", "素材4", "素材5", "素材6", "素材7", "素材8", "素材9", "素材10" };
    //素材ファイルのヘッダー
    private string[] materialHeader = { "","ATK", "MP", "TYPE" };
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
        for(int i = 1; i <= 10; i++)//Atkポイントの保存
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
        for(int m = 1; m <= 10; m++)//Mpの保存
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
        if(sw != null)
        {
            for (int i = 0; i < datas.Length; i += 3)
            {
                sw.WriteLine(string.Format("{0},{1},{2}", datas[i], datas[i + 1], datas[i + 2]));
            }
        }
       
        sw.Close();
    }
    //本のデータの読み込み
    public void BooksDataLoad()
    {
        BOOKS_DATA = new List<string[]>();//初期化しておく
        string bookdata = Path.Combine(Application.persistentDataPath, string.Format("book_{0}.csv",TitleManager.SELECT_DATA_NUMBER));
        using (var sw = new StreamReader(bookdata))
        {
            while (sw.Peek() != -1)
            {
                string line = sw.ReadLine();
                BOOKS_DATA.Add(line.Split(','));
            }
        }
    }
    /// <summary>
    /// 保存データの中身だけ見たい場合に呼び出す
    /// </summary>
    /// <param name="dataNumber"></param>
    /// <returns></returns>
    public List<string[]> BooksDataLoadTest(int dataNumber)
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
    /// <summary>
    /// 初回ゲームスタート時の保存ファイル作成
    /// </summary>
    /// <param name="firstStatus"></param>
    public void CreateSaveFile(string[] firstStatus, int filesNum)
    {
        bookPath = Path.Combine(Application.persistentDataPath, string.Format("book_{0}.csv", TitleManager.SELECT_DATA_NUMBER));
        materialPath = Path.Combine(Application.persistentDataPath, "Items.csv");
        if(!File.Exists(bookPath))//本のデータがなければ
        {
            //本のデータ1～3（book_{n}.csv）を作成
            for (int n = 0; n <= filesNum; n++)
            {
                string path = Path.Combine(Application.persistentDataPath, string.Format("book_{0}.csv", n));

                StreamWriter sw_0 = new StreamWriter(path, false);
                if (sw_0 != null)
                {
                    //1行ずつデータを入力
                    for (int i = 0; i < firstStatus.Length; i += 3)
                    {
                        sw_0.WriteLine(string.Format("{0},{1},{2}", firstStatus[i], firstStatus[i + 1], firstStatus[i + 2]));
                    }
                }
                sw_0.Close();
            }
        }
        if(!File.Exists(materialPath))//素材のデータが無い場合
        {
            StreamWriter sw_material = new StreamWriter(materialPath, false, Encoding.UTF8);
            if(sw_material != null)
            {
                MATERIALS_DATA.Add(materialHeader);
                //素材テンプレートを入れておく
                for (int i = 1; i <= materialName.Length; i++)//行 1から素材数まで
                {
                    
                    string zero = string.Format("{0}", materialName[i - 1]);
                    string[] mt = { zero, "", "", "" };
                    MATERIALS_DATA.Add(mt);
                    
                }

                //ファイルにテンプレートを書き込み
                for (int s = 0; s <= materialName.Length; s++)//行の数
                {
                    sw_material.WriteLine(string.Format("{0},{1},{2},{3}", MATERIALS_DATA[s][0], MATERIALS_DATA[s][1], MATERIALS_DATA[s][2], MATERIALS_DATA[s][3]));
                }
            }
            sw_material.Close();   
        }
    }
}
