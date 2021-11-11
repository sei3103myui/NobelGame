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

    [Header("Button�̐e�I�u�W�F�N�g")]
    public GameObject ButtonPanel;

    public UnityEvent setClickEvent;

    [HideInInspector]public int setId = 0;
   
    [Header("�f�ނ̐ݒ�")]
    public GameObject buttonPrefab;
    public List<Sprite> buttonSprites = new List<Sprite>();
    public List<bool> usetext = new List<bool>();
    public List<string> texts = new List<string>();
    [Header("�{�^���̃X�P�[���ύX���T�C�Y")]
    public Vector2 btnDefaultScale = new Vector2(160,160);
    public Vector2 btnScale = new Vector2(200,200);
    [Header("�{�^���̊Ԋu")]
    public Vector2 spaceRt = new Vector2(-250,-100);
    [Header("Button�̐�")]
    [SerializeField] private int buttonsNum = 0;
    
    
    void Start()
    {
        for(int i = 0; i < buttonsNum; i++)
        {
            //�{�^���̐���
            GameObject button = Instantiate(buttonPrefab);
            //Canvas�̐e�I�u�W�F�N�g��ݒ�
            button.transform.SetParent(ButtonPanel.transform);
            //�����ʒu�̐ݒ�
            RectTransform rpos = button.GetComponent<RectTransform>();
            rpos.localScale = new Vector3(1, 1, 1);
            //�R���g���[���[�̎擾
            button.AddComponent<UIButtonController>();
            UIButtonController btController = button.GetComponent<UIButtonController>();
            btControllers.Add(btController);//���X�g�ɓo�^
            btController.id = i;//id�ݒ�
            btController.manager = gameObject.GetComponent<UIButtonManager>();//manager�i�����j��n��
            //�{�^���f�U�C���̐ݒ�
            if (buttonSprites[i] != null)btController.btImage.sprite = buttonSprites[i];
            if(usetext[i])btController.childTxt.text = texts[i];
            //���\�b�h�o�^
            moveBtn += btController.MoveOn;  
        }
        //�����ʒu�ړ�
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
    /// �{�^������ŕς���ꍇ
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
