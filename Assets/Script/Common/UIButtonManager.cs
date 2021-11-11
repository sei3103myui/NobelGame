using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum ActiveButtonMode
{
    Play,
    Pose
}
public class UIButtonManager : MonoBehaviour
{
    delegate void MoveButton();
    MoveButton moveBtn;

    public static ActiveButtonMode actBtnMode = ActiveButtonMode.Play;

    public List<UIButtonController> btControllers = new List<UIButtonController>();

    [Header("Buttonの親オブジェクト")]
    public GameObject ButtonPanel;

    public UnityEvent setClickEvent;

    [HideInInspector]public int setId = 0;
   
    [Header("素材の設定")]
    public GameObject buttonPrefab;
    public List<Sprite> buttonSprites = new List<Sprite>();
    public List<bool> usetext = new List<bool>();
    public List<string> texts = new List<string>();
    [Header("ボタンのスケール変更時サイズ")]
    public Vector2 btnDefaultScale = new Vector2(160,160);
    public Vector2 btnScale = new Vector2(200,200);
    [Header("ボタンの間隔")]
    public Vector2 spaceRt = new Vector2(-250,-100);
    [Header("Buttonの数")]
    [SerializeField] private int buttonsNum = 0;
    
    
    void Start()
    {
        for(int i = 0; i < buttonsNum; i++)
        {
            //ボタンの生成
            GameObject button = Instantiate(buttonPrefab);
            //Canvasの親オブジェクトを設定
            button.transform.SetParent(ButtonPanel.transform);
            //初期位置の設定
            RectTransform rpos = button.GetComponent<RectTransform>();
            rpos.localScale = new Vector3(1, 1, 1);
            //コントローラーの取得
            button.AddComponent<UIButtonController>();
            UIButtonController btController = button.GetComponent<UIButtonController>();
            btControllers.Add(btController);//リストに登録
            btController.id = i;//id設定
            btController.manager = gameObject.GetComponent<UIButtonManager>();//manager（自分）を渡す
            //ボタンデザインの設定
            if (buttonSprites[i] != null)btController.btImage.sprite = buttonSprites[i];
            if(usetext[i])btController.childTxt.text = texts[i];
            //メソッド登録
            moveBtn += btController.MoveOn;  
        }
        //初期位置移動
        moveBtn.Invoke();
        btControllers[setId].SelectedButton();
        setClickEvent.Invoke();
        
    }

    private void Update()
    {
        if(actBtnMode == ActiveButtonMode.Play)
        {
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                if (setId > 0)
                {
                    setId--;
                }
                else
                {
                    setId = buttonsNum - 1;
                }

                moveBtn.Invoke();
                btControllers[setId].SelectedButton();
            }
            else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                if (setId < buttonsNum - 1)
                {
                    setId++;

                }
                else
                {
                    setId = 0;
                }

                moveBtn.Invoke();
                btControllers[setId].SelectedButton();
            }
        }
        
    }
    /// <summary>
    /// ボタン操作で変える場合
    /// </summary>
    /// <param name="on"></param>
    public void OnClickChangeButton(bool on)
    {
        if(actBtnMode == ActiveButtonMode.Play)
        {
            if (on)
            {
                if (setId < buttonsNum - 1)
                {
                    setId++;

                }
                else
                {
                    setId = 0;
                }

            }
            else
            {
                if (setId > 0)
                {
                    setId--;
                }
                else
                {
                    setId = buttonsNum - 1;
                }
            }
            moveBtn.Invoke();
            btControllers[setId].SelectedButton();
        }     
    }
}
