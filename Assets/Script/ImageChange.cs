using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ImageChange : MonoBehaviour
{
    public Image TalkIcon;
    public Sprite[] newTalkIcon;
    public Sprite[] new2TalkIcon;

    private bool isbuttonDown = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.zKey.isPressed)
        {
            if (!isbuttonDown)
            {
                TalkIcon.sprite = newTalkIcon[0];
                isbuttonDown = true;
            }
            else
            {
                TalkIcon.sprite = new2TalkIcon[0];
                isbuttonDown = false;
            }
            
        }
    }

    
}
