using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonController : MonoBehaviour
{
    public UIButtonManager manager;
    public Button button;
    public Image btImage = default;
    public Text childTxt = default;
    public int id = 0;    
    public int oldId = 0;
    
    private RectTransform myTransform;
    private float speed = 4f;
 
    private Vector3 movePos;
    
   
    private void Awake()
    {
        button = gameObject.GetComponent<Button>();
        myTransform = gameObject.GetComponent<RectTransform>();
        childTxt = gameObject.transform.Find("Text").GetComponent<Text>();
        btImage = gameObject.transform.Find("Image").GetComponent<Image>();
    }
    void Start()
    {
        //childTxt.text = null;
    }
  
    void Update()
    {
        
        myTransform.anchoredPosition = Vector3.Lerp(myTransform.localPosition, movePos, Time.deltaTime * speed);
    }

    public void MoveOn()
    {
        //差分チェック       
        int diff = Difference();
        movePos = new Vector3(diff * manager.spaceRt.x, Mathf.Abs(diff) * manager.spaceRt.y, 0);
        ActiveChange(diff);

        if (manager.setId != id)
        {
            button.enabled = false;
            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, manager.btnDefaultScale.x);
            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, manager.btnDefaultScale.y);
        }
        else
        {
            button.enabled = true;
            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, manager.btnScale.x);
            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, manager.btnScale.y);
        }
    }

    public int Difference()
    {
        var diff = id - manager.setId;
        if (Mathf.Abs(diff) >= manager.btControllers.Count - 1)
        {
            diff = Mathf.Sign(diff) == -1 ? 1 : -1;
            
        }
        return diff;
    }

    public void ActiveChange(int diff)
    {
        if (Mathf.Abs(diff) >= 2)
        {
            btImage.enabled = false;
            childTxt.enabled = false;
        }
        else if (btImage.enabled != true || childTxt.enabled != true)
        {
            btImage.enabled = true;
            childTxt.enabled = true;
        }
    }

    public void SelectedButton()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

   
}
