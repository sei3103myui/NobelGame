using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;

public class PlayerPrefsCommon : MonoBehaviour
{
    public static List<string[]> BOOKS_DATA = new List<string[]>();
    public static List<string[]> MATERIALS_DATA = new List<string[]>();

    public static List<int[]> BooksPlayData = new List<int[]>();
    public static List<int[]> MaterialsPlayData = new List<int[]>();

    private string bookPath = default;//アイテムデータ保存先用
    private string materialPath = default;//素材データ保存先用
    private StreamWriter sw;
    private StreamWriter itemsw;
    private ReinforcementManager reinforcementManager;

    //素材の名前
    private string[] materialName = { "素材1", "素材2", "素材3", "素材4", "素材5", "素材6", "素材7", "素材8", "素材9", "素材10" };
    //素材ファイルのヘッダー
    private string[] materialHeader = { "","ATK", "MP", "TYPE" };

    public void SavePlayer()
    {
        //HPセーブ
        PlayerPrefs.SetFloat("PlayerHP", PlayerStatus.PLAYER_HP);
        //MPセーブ
        PlayerPrefs.SetFloat("PlayerMP", PlayerStatus.PLAYER_MP);
    }
    /// 素材データの保存（上書き）
    /// </summary>
    /// <param name="atk"></param>
    /// <param name="mp"></param>
    /// <param name="type"></param>
    public static void SaveItem(int atk,int mp , int type)
    {
        int[] newItemData = { atk, mp, type };
        for(int i = 0; i < MaterialsPlayData.Count; i++)
        {
            if(MaterialsPlayData[i][0] == 0)
            {
                MaterialsPlayData[i] = newItemData;
                break;
            }else if(i == MaterialsPlayData.Count - 1 && MaterialsPlayData[i][0] != 0)
            {
                //空いている領域が無いので上書き
                MaterialsPlayData[i] = newItemData;
            }
        }      
    }
    
    /// <summary>
    /// アイテムのセーブ
    /// </summary>
    /// <param name="datas"></param>
    public void SavebookFile()
    {        
        string path = Path.Combine(Application.persistentDataPath, string.Format("book_{0}.csv",TitleManager.SELECT_DATA_NUMBER));
        
        sw = new StreamWriter(path, false, Encoding.UTF8);
        if(sw != null)
        {
            for (int i = 0; i < BOOKS_DATA.Count; i++)
            {
                for (int n = 0; n < BOOKS_DATA[i].Length; n++)
                {
                    if (n != BOOKS_DATA[i].Length - 1)
                    {
                        sw.Write(string.Format("{0},", BOOKS_DATA[i][n]));
                    }
                    else
                    {
                        sw.Write(string.Format("{0}", BOOKS_DATA[i][n]));
                    }

                }
                sw.Write("\r\n");
            }
        }
       
        sw.Close();
    }

    /// <summary>
    /// 素材アイテムのセーブ
    /// </summary>
    public  void SaveItemFiles()
    {
        string itemPath = Path.Combine(Application.persistentDataPath, string.Format("Items_{0}.csv", TitleManager.SELECT_DATA_NUMBER));

        itemsw = new StreamWriter(itemPath, false,Encoding.UTF8);
        if (itemsw != null)
        {
            for (int i = 0; i < MATERIALS_DATA.Count; i++)
            {
                for(int n = 0; n < MATERIALS_DATA[i].Length; n++)
                {
                    if(n != MATERIALS_DATA[i].Length - 1)
                    {
                        itemsw.Write(string.Format("{0},", MATERIALS_DATA[i][n]));
                    }
                    else
                    {
                        itemsw.Write(string.Format("{0}", MATERIALS_DATA[i][n]));
                    }
                    
                }
                itemsw.Write("\r\n");
            }
        }
        itemsw.Close();
        Debug.Log("更新");
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
    /// <summary>
    /// ファイルの中身を取得
    /// </summary>
    /// <param name="saveNum"></param>
    /// <returns></returns>
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

    //セーブデータロード処理
    public static void SaveFilesLoad()
    {
        //リスト初期化
        BOOKS_DATA = new List<string[]>();
        MATERIALS_DATA = new List<string[]>();
        //パス取得
        string filepath = Path.Combine(Application.persistentDataPath, string.Format("Items_{0}.csv",TitleManager.SELECT_DATA_NUMBER));
        string bookpath = Path.Combine(Application.persistentDataPath, string.Format("book_{0}.csv", TitleManager.SELECT_DATA_NUMBER));

        string[] paths = { filepath, bookpath };

        foreach(string path in paths)
        {
            if (File.Exists(path))
            {
                using (var sw = new StreamReader(path))
                {
                    while (sw.Peek() != -1)
                    {
                        string line = sw.ReadLine();
                        if(path == filepath)
                        {
                            MATERIALS_DATA.Add(line.Split(','));
                        }
                        else if(path == bookpath)
                        {
                            BOOKS_DATA.Add(line.Split(','));
                        }
                        else
                        {
                            Debug.Log("パスワードが一致しません");
                        }
                        
                    }
                }
            }
        }
    }
    /// <summary>
    /// 書き換え用データの取得（元データコピー）
    /// </summary>
    public static void PlaydataNewLoad()
    {
        
        List<int[]> numdata = new List<int[]>();
        for (int i = 0; i < BOOKS_DATA.Count; i++)
        {
            int[] newdata = BOOKS_DATA[i].Select(int.Parse).ToArray();
            BooksPlayData.Add(newdata);
        }
        
        for(int i = 1; i < MATERIALS_DATA.Count; i++)
        {
            int[] itemsnum = new int[MATERIALS_DATA[i].Length - 1];
            for(int n = 1; n < MATERIALS_DATA[i].Length; n++)
            {
                
                if(MATERIALS_DATA[i][n] == "")
                {
                    itemsnum[n - 1] = 0;
                }
                else
                {
                    itemsnum[n - 1] = int.Parse(MATERIALS_DATA[i][n]);
                } 
            }
            MaterialsPlayData.Add(itemsnum);
        }
    }

    public static void PlaydataStringFormat()
    {
        for (int i = 0; i < MaterialsPlayData.Count; i++)
        {
            for(int n = 0; n < MaterialsPlayData[i].Length; n++)
            {
                MATERIALS_DATA[i + 1][n + 1] = MaterialsPlayData[i][n].ToString();
            }
        }

    }
    public static void BookPlaydataStringFormat()
    {

        for (int i = 0; i < BooksPlayData.Count; i++)
        {
            for (int n = 0; n < BooksPlayData[i].Length; n++)
            {
                BOOKS_DATA[i][n] = BooksPlayData[i][n].ToString();
            }
        }
    }

    public static void MaterialDataDelete(int materialNumber)
    {
        for(int i = 0; i < MaterialsPlayData[materialNumber].Length; i++)
        {
            MaterialsPlayData[materialNumber][i] = 0;
        }
        
    }
}
