using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TalkManager : MonoBehaviour
{
    public enum TalkingMode
    {
        Auto,
        Button,
        Skip
    }
    public TalkingMode[] talkingModes;
    public string[] csvfilesName;
    public string nextSceneName;

    public TalkLoad talkLoad;
    public AudioClip prologueBgm;

    [Header("オプション等UIの設定ここから")]
    public GameObject skipMenu;
    public GameObject firstSkip;

    private bool isAuto;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager2D.Instance.AudioBgm.clip = prologueBgm;
        AudioManager2D.Instance.AudioBgm.Play();
        StartCoroutine(TalkingCommon());
    }

    // Update is called once per frame
    void Update()
    {
        if (isAuto && Keyboard.current.zKey.isPressed)
        {
            isAuto = false;
            talkLoad.talkingMode = TalkLoad.TalkingMode.Button;
        }
    }
    //エピソード開始
    public IEnumerator TalkingCommon()
    {
        //設定したモードの数だけ会話文をつなげる
        for (int i = 0; i < talkingModes.Length; i++)
        {
            talkLoad.csvFileName = csvfilesName[i];
            talkLoad.CSVLoad();//csvファイル読み込みしてもらう
            if (talkingModes[i] == TalkingMode.Auto)
            {
                //自動で会話文を表示
                talkLoad.isTalk = true;
                StartCoroutine(talkLoad.Talking());
            }
            else if (talkingModes[i] == TalkingMode.Button)
            {
                //ボタンが押されてからスタート
                while (!Keyboard.current.anyKey.isPressed) yield return null;
                talkLoad.isTalk = true;
                StartCoroutine(talkLoad.Talking());
            }

            //会話が終了するのを待つ
            while (talkLoad.isTalk) yield return null;

            //もし次のシーン名があれば
            if (nextSceneName != "")
            {
                AudioManager2D.Instance.AudioBgm.Stop();
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    public void OnClickSkip()
    {
        skipMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSkip);
    }

    public void OnClickOK()
    {
        if (skipMenu.activeInHierarchy)
        {
            AudioManager2D.Instance.AudioBgm.Stop();
            SceneManager.LoadScene(nextSceneName);
        }
    }
    public void OnClickCancel()
    {
        if (skipMenu.activeInHierarchy)
        {
            skipMenu.SetActive(false);
        }
    }

    public void OnClickAuto()
    {
        talkLoad.talkingMode = TalkLoad.TalkingMode.Auto;
        isAuto = true;
    }
}
