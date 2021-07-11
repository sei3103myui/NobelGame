using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum EpisodeMode
{
    Story,
    Choices,
    End,
    Skip
}

public enum EpisodePlayMode
{
    Wait,
    Play,
    Stop,
    End
}

public enum EpisodeKeyConfig
{
    Auto,
    Button,
    Skip
}
public class EpisodeManager : MonoBehaviour
{
    public EpisodePlayMode playMode = EpisodePlayMode.Wait;
    public Text nametxt;//キャラクターの名前を格納するテキスト
    public Text storytxt;//ストーリーを格納するテキスト

    [Header("文字送りの速さ")]
    public float talkSpeed = 0.1f;
    public EpisodeKeyConfig episodeKeyConfig = EpisodeKeyConfig.Button;
    protected EpisodeMode episodeMode = EpisodeMode.Story;

    protected TextAsset csvfile;
    protected List<string[]> episodefileList = new List<string[]>();

    protected int lineNum = 1;//行
    protected int ColumNum = 0;//列

    private GameObject[] ButtonsObj;
    protected Canvas mainCanvas;
    [Header("拡張子なしCSVファイルの名前")]
    [SerializeField] private string filePath;
    [Header("再生終了後のシーン名")]
    [SerializeField] private string nextSceneName; 
    [SerializeField] private GameObject nextButton;
    private GameObject choicePanel;
    private int choiceNum = 0;
    private int selectChoiceNum = 0;

    private void Awake()
    {
        
    }

    private void Start()
    {
        mainCanvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        storytxt.text = null;

        //初期値Playなら再生
        if(playMode == EpisodePlayMode.Play)
        {
            StartCoroutine(StoryCoroutine());
        }
    }

    public IEnumerator LoadEpisode()
    {
        episodefileList = new List<string[]>();
        //csvファイルの読み込み
        csvfile = Resources.Load("CSV/Talk/" + filePath) as TextAsset;

        StringReader reader = new StringReader(csvfile.text);

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            episodefileList.Add(line.Split(','));
        }
        yield return null;
    }

    public IEnumerator StoryCoroutine()
    {
        choiceNum = 0;
        lineNum = 1;
        //ファイルの読み込み
        yield return StartCoroutine(LoadEpisode());

        for(int i = 1; i < episodefileList.Count; i++)
        {
            if(episodefileList[i][0] == "")
            {
                episodeMode = EpisodeMode.Story;
                //ストーリー再生処理
                yield return StartCoroutine(talkingCoroutine(i));
                
                lineNum++;

            }else if(episodefileList[i][0] == "Choices")
            {
                episodeMode = EpisodeMode.Choices;

                //分岐数チェック
                if (episodefileList[i][1] != "")
                {
                    choiceNum = int.Parse(episodefileList[i][1]);
                    LoadUI();
                    
                }
                yield return new WaitForSeconds(1);
                //選択肢が押されるまで待つ
                selectChoiceNum = 0;
                
                while (selectChoiceNum == 0) yield return null;
                
                lineNum += selectChoiceNum - 1;//選択肢と同じ行番号を取得
                string choicetitle = episodefileList[lineNum][2];//分岐先名を取得
                for(int num = lineNum; num < episodefileList.Count; num++)
                {
                    
                    if(episodefileList[num][0] == choicetitle)
                    {
                        lineNum = num;
                        break;
                    }
                }
                
                yield return StartCoroutine(talkingCoroutine(lineNum));
                //進んだ行番号へ更新
                i = lineNum;
                lineNum++;
            }
            else
            {
                Debug.LogError("コマンドエラーです");
                
            }
            //分岐ストーリー終了後なら
            if(episodeMode == EpisodeMode.End)
            {
                for (int num = lineNum; num < episodefileList.Count; num++)
                {
                    if (episodefileList[num][0] == "end")
                    {
                        lineNum = num;
                        break;
                    }
                }
                
                yield return StartCoroutine(talkingCoroutine(lineNum));
                episodeMode = EpisodeMode.Story;
                //進んだ行番号へ更新
                i = lineNum;
                lineNum++;
            }
          
            yield return null;
        }
        playMode = EpisodePlayMode.End;
        NextSceneLoad();
    }
    /// <summary>
    /// 1行再生処理
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public IEnumerator talkingCoroutine(int line)
    {
        //キャラクターの名前があればテキストに(無ければ"")
        if(episodefileList[line][3] != "")
        {
            nametxt.text = episodefileList[line][3];
        }
        else
        {
            nametxt.text = null;
        }
        
        
        //会話再生
        for (int i = 4; i < episodefileList[line].Length; i++)
        {
            switch (episodefileList[line][i])
            {
                case "":
                    break;
                case "end":
                    break;
                default:
                    break;
            }
            if(episodefileList[line][i] == "")
            {
                storytxt.text = null;
                break;
            }else if (episodefileList[line][i] == "end")
            {
                storytxt.text = null;
                episodeMode = EpisodeMode.End;
                break;
            }
            else
            {
                //会話文が続いていれば表示する
                string word = episodefileList[line][i];
                for (int n = 0; n < word.Length; n++)
                {
                    if(word[n] == '"')
                    {
                        continue;
                    }
                    //空白文字で改行する
                    char nullWord = '　';
                    if (word[n] == nullWord)
                    {
                        storytxt.text += "\n";
                        continue;

                    }
                    storytxt.text += word[n];
                    if (episodeKeyConfig == EpisodeKeyConfig.Skip)
                    {
                        yield return new WaitForSeconds(0.01f);

                    }
                    else
                    {
                        yield return new WaitForSeconds(talkSpeed);
                    }
                    
                }
            }
            nextButton.SetActive(true);
            switch (episodeKeyConfig)
            {
                case EpisodeKeyConfig.Button:
                    while (!Keyboard.current.spaceKey.wasPressedThisFrame)
                    {
                        if (episodeKeyConfig == EpisodeKeyConfig.Auto)
                        {
                            yield return new WaitForSeconds(0.5f);
                            break;
                        }else if(episodeKeyConfig == EpisodeKeyConfig.Skip)
                        {
                            yield return new WaitForSeconds(0.01f);
                            break;
                        }
                        yield return null;
                    }
                    break;
                case EpisodeKeyConfig.Auto:

                    yield return new WaitForSeconds(0.5f);
                    break;
                case EpisodeKeyConfig.Skip:
                    yield return new WaitForSeconds(0.01f);
                    break;
            }
            storytxt.text = null;
            nextButton.SetActive(false);
        }
        yield return null;
    }

    /// <summary>
    /// 分岐選択UI表示処理
    /// </summary>
    public void LoadUI()
    {
        //選択肢の数に合うButtonが入ったパネルを生成（Prefab）
        choicePanel = Instantiate(Resources.Load<GameObject>(string.Format("UI/ChoicesPanel_{0}", choiceNum)));
        //MainCanvasの子オブジェクトに登録
        choicePanel.transform.SetParent(mainCanvas.transform, false);
        ButtonsObj = new GameObject[choiceNum];
        GameObject[] buttonTxt = new GameObject[choiceNum];
        
        for (int num = 0; num < choicePanel.transform.childCount; num++)
        {
            int number = num + 1;
            ButtonsObj[num] = choicePanel.transform.GetChild(num).gameObject;
            ButtonsObj[num].GetComponent<Button>().onClick.AddListener(() => { ChoicesOnClick(number); });
            
            
            buttonTxt[num] = ButtonsObj[num].transform.GetChild(0).gameObject;
            buttonTxt[num].GetComponent<Text>().text = episodefileList[lineNum + num][2];
        }
        EventSystem.current.SetSelectedGameObject(ButtonsObj[0]);
    }
    /// <summary>
    /// 選択肢が何番目か取得する（初期値0から）
    /// </summary>
    /// <param name="buttonNum"></param>
    public void ChoicesOnClick(int buttonNum)
    {
        //選択したボタンの番号を返す
        selectChoiceNum = buttonNum;
        StartCoroutine(choicePanelDestroy());
    }

    public IEnumerator choicePanelDestroy()
    {
        yield return null;
        Destroy(choicePanel);
    }
    /// <summary>
    /// オート再生モード選択時
    /// </summary>
    public void OnClickAuto()
    {
        if(episodeKeyConfig == EpisodeKeyConfig.Auto)
        {
            episodeKeyConfig = EpisodeKeyConfig.Button;
        }
        else
        {
            episodeKeyConfig = EpisodeKeyConfig.Auto;
        }
        
    }
    /// <summary>
    /// スキップ選択
    /// </summary>
    public void OnClicSkip()
    {
        if (episodeKeyConfig == EpisodeKeyConfig.Skip)
        {
            episodeKeyConfig = EpisodeKeyConfig.Button;
        }
        else
        {
            episodeKeyConfig = EpisodeKeyConfig.Skip;
        }
        
    }


    public void NextSceneLoad()
    {
        //遷移先が入力されていたらシーン遷移
        if(nextSceneName != null)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
