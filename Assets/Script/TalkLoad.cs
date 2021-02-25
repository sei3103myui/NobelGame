using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using System;

public class TalkLoad : MonoBehaviour
{
    public enum TalkMode
    {
        NoName,
        OnName
    }
    public TalkMode talkmode;
    public enum TalkingMode
    {
        Auto,
        Button
    }
    public TalkingMode talkingMode = TalkingMode.Button;

    [Header("拡張子なしCSVファイルの名前")]
    public string csvFileName;
    [Header("会話文表示用Text")]
    public Text talkText;
    public Text nameText;
    public GameObject buttonImage;
    public bool isTalk = false;
    
    private string characterName;
    private string word;
    
    TextAsset csvFile;
    List<string[]> csvDatas = new List<string[]>();

    // Start is called before the first frame update
    void Start()
    {
        //csvFile = Resources.Load("CSV/Talk/" + csvFileName) as TextAsset;
        //StringReader reader = new StringReader(csvFile.text);

        //while (reader.Peek() != -1)
        //{
        //    string line = reader.ReadLine();
        //    csvDatas.Add(line.Split(','));
        //}

        talkText.text = null;//テキストの初期化
        if(talkmode == TalkMode.OnName)
        {
            nameText.text = null;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        //if (Keyboard.current.spaceKey.isPressed && !isTalk)
        //{
        //    isTalk = true;
        //    StartCoroutine(Talking());
        //}
    }

    public void CSVLoad()
    {
        csvFile = Resources.Load("CSV/Talk/" + csvFileName) as TextAsset;
        StringReader reader = new StringReader(csvFile.text);

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            csvDatas.Add(line.Split(','));
        }
       
    }
    public IEnumerator Talking()
    {
        if (talkmode == TalkMode.OnName)
        {
            for (int i = 0; i <= csvDatas.Count; i++)//行
            {
                //会話を全て表示した場合
                if (i == csvDatas.Count)
                {
                    //ボタンが押されたら終了処理
                    while (!Keyboard.current.spaceKey.isPressed) yield return null;
                    talkText.text = null;
                    if (talkmode == TalkMode.OnName)
                    {
                        nameText.text = null;
                    }
                    break;
                }
                //一行目（キャラクターの名前）があるか
                if (csvDatas[i][0] != "")
                {
                    characterName = csvDatas[i][0];//会話主の名前は1列目
                    nameText.text = characterName;
                }
                else
                {
                    //無ければ空白
                    nameText.text = null;
                }


                for (int s = 1; s < csvDatas[i].Length; s++)//列
                {
                    //会話文がない（空白）だった場合
                    if (csvDatas[i][s] == "")
                    {
                        talkText.text = null;
                        break;
                    }
                    else
                    {
                        //会話文が続いていれば表示する
                        word = csvDatas[i][s];
                        for (int n = 0; n < word.Length; n++)
                        {
                            //空白文字で改行する
                            char nullWord = '　';
                            if (word[n] == nullWord)
                            {
                                talkText.text += "\n";
                                continue;

                            }
                            talkText.text += word[n];
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                    buttonImage.SetActive(true);
                    if (talkingMode == TalkingMode.Button)
                    {
                        while (!Keyboard.current.spaceKey.isPressed && talkingMode == TalkingMode.Button) yield return null;
                    }
                    else if (talkingMode == TalkingMode.Auto)
                    {
                        yield return new WaitForSeconds(0.5f);
                    }
                    talkText.text = null;
                    buttonImage.SetActive(false);
                }

            }
            yield return new WaitForSeconds(2);
            isTalk = false;//会話終了
        }
        else if (talkmode == TalkMode.NoName)//キャラ名無しここから
        {
            for (int i = 0; i <= csvDatas.Count; i++)//行
            {
                //会話を全て表示した場合
                if (i == csvDatas.Count)
                {
                    //ボタンが押されたら終了処理
                    while (!Keyboard.current.spaceKey.isPressed) yield return null;
                    //talkText.text = null;
                    
                    break;
                }

                for (int s = 0; s < csvDatas[i].Length; s++)//列
                {
                    //会話文がない（空白）だった場合
                    if (csvDatas[i][s] == "")
                    {
                        talkText.text = null;
                        break;
                    }
                    else
                    {
                        //会話文が続いていれば表示する
                        word = csvDatas[i][s];
                        for (int n = 0; n < word.Length; n++)
                        {
                            
                            talkText.text += word[n];
                            yield return new WaitForSeconds(0.06f);
                        }
                    }
                    buttonImage.SetActive(true);
                    if(talkingMode == TalkingMode.Button)
                    {
                        while (!Keyboard.current.spaceKey.isPressed && talkingMode == TalkingMode.Button) yield return null;
                    }
                    else if(talkingMode == TalkingMode.Auto)
                    {
                        yield return new WaitForSeconds(1);
                    }
                    talkText.text += "\n";
                    buttonImage.SetActive(false);
                }
            }
            yield return new WaitForSeconds(2);
            isTalk = false;//会話終了
        }
    }

}
