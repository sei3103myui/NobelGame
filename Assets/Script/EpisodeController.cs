using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EpisodeController : EpisodeManager
{
    
    void Start()
    {
        mainCanvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        storytxt.text = null;
        //èâä˙ílPlayÇ»ÇÁçƒê∂
        if (playMode == EpisodePlayMode.Play)
        {
            StartCoroutine(StoryCoroutine());
           
        }
    }

    public  void StoryFileLoad(string path)
    {
        csvfile = Resources.Load(path) as TextAsset;

        StringReader reader = new StringReader(csvfile.text);

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            episodefileList.Add(line.Split(','));
        }
    }

    
}
