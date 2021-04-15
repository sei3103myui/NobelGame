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


    private string bookPath = default;//アイテムデータ保存先用
    private string materialPath = default;//素材データ保存先用
    private StreamWriter sw;
    private ReinforcementManager reinforcementManager;

    //素材の名前
    private string[] materialName = { "素材1", "素材2", "素材3", "素材4", "素材5", "素材6", "素材7", "素材8", "素材9", "素材10" };
    //素材ファイルのヘッダー
    private string[] materialHeader = { "","ATK", "MP", "TYPE" };
    
    void Start()
    {
        
    }

    
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
    //素材データの保存
    public void SaveItem(float atk,float mp)
    {
        materialPath = Path.Combine(Application.persistentDataPath, string.Format("Items_{0}", TitleManager.SELECT_DATA_NUMBER));
        //ファイルが見つかったら
        if (File.Exists(materialPath))
        {
            //csvファイルの内容を読み込み
            MATERIALS_DATA = ItemsLoad(TitleManager.SELECT_DATA_NUMBER);

            //
        }
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
        materialPath = Path.Combine(Application.persistentDataPath, "Items_0.csv");
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
            for(int m = 0; m <= filesNum; m++)
            {
                string path = Path.Combine(Application.persistentDataPath, string.Format("Items_{0}.csv",m));

                StreamWriter sw_material = new StreamWriter(path, false, Encoding.UTF8);
                if (sw_material != null)
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

    public List<string[]> ItemsLoad(int saveNum)
    {
        List<string[]> items = new List<string[]>();
        
        //パス取得
        string filepath = Path.Combine(Application.persistentDataPath, string.Format("Items_{0}.csv", saveNum));
        if (File.Exists(filepath))
        {
            using (var sw = new StreamReader(filepath))
            {
                while (sw.Peek() != -1)
                {
                    string line = sw.ReadLine();
                    items.Add(line.Split(','));
                }
            }
        }
        else
        {
            
        }

        return items;
    }

    public List<int[]> ItemsFileChange(List<string[]> stringItems)
    {
        //数値データを保存するリスト
        List<int[]> intItems = new List<int[]>();
        
        //行
        for(int h = 1; h <= stringItems.Count; h++)
        {
            
            for(int v = 1; v <= stringItems[h].Length; v++)
            {
                //データがあれば
                if(stringItems[h][v] == "")
                {
                    
                }
            }
            
        }
        return intItems;
    }
}
