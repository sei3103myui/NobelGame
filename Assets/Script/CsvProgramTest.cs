using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CsvProgramTest : MonoBehaviour
{
    private StreamWriter sw;
    string[] words = new string[] { "1", "10", "10", "3", "10", "15", "4", "10", "15"};

    
    public Text text;
    //private List<string[]> csvBook1 = new List<string[]>();
    private bool isPush = false;
    // Start is called before the first frame update
    void Start()
    {
        List<string[]> csvBook1 = new List<string[]>();
        TextAsset bookdata = Resources.Load("CSV/Book/book_1") as TextAsset;
        StringReader bookreader = new StringReader(bookdata.text);
        while (bookreader.Peek() != -1)
        {
            string line = bookreader.ReadLine();
            csvBook1.Add(line.Split(','));
        }
        for (int i = 0; i < csvBook1.Count; i++)
        {
            for (int n = 0; n < csvBook1[i].Length; n++)
            {
                text.text += csvBook1[i][n];
                text.text += "　";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void FixedUpdate()
    {
        if (Keyboard.current.spaceKey.isPressed && !isPush)
        {
            isPush = true;
            StartCoroutine(SaveWait());
        }
    }

    
    public IEnumerator SaveWait()
    {
        string path = Application.dataPath + "/Resources/CSV/Book/book_1.csv";
        if (File.Exists(path))
        {
            Debug.Log("存在します");
        }
        else
        {
            Debug.Log("存在しません");
        }
        sw = new StreamWriter(path, false);
        if (sw != null)
        {
            for (int i = 0; i < words.Length; i += 3)
            {
                sw.WriteLine(string.Format("{0},{1},{2}", words[i], words[i + 1], words[i + 2]));
            }
            sw.Close();
        }
        Debug.Log("OK");
        while (!Keyboard.current.spaceKey.isPressed) yield return null;

        TextAsset bookdata = Resources.Load("CSV/Book/book_1") as TextAsset;
        StringReader bookreader = new StringReader(bookdata.text);
        List<string[]> csvBook2 = new List<string[]>();
        while (bookreader.Peek() != -1)
        {
            string line = bookreader.ReadLine();
            csvBook2.Add(line.Split(','));
        }
        text.text = "";
        for (int i = 0; i < csvBook2.Count; i++)
        {
            for (int n = 0; n < csvBook2[i].Length; n++)
            {
                text.text += csvBook2[i][n];
                text.text += "　";
                
            }
        }
    }
}
