using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MapManager : MonoBehaviour
{
    public static string SELECT_EPISODE_NAME;
    public static string SELECT_BATTLE_MODE = "Easy";

    public GameObject select;
    public GameObject chapterSelect;
    public GameObject episodeSelect;
    public GameObject battleModeSelect;
    public GameObject firstChapter;
    public GameObject firstSelect;
    public GameObject firstEpisode;
    public GameObject firstMode;
    public GameObject battleStartButton;
    public Text chapterName;

    public AudioClip mapBgm;

    private string selectTextName;
    private bool isOption = false;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager2D.Instance.AudioBgm.clip = mapBgm;
        AudioManager2D.Instance.AudioBgm.Play();
        EventSystem.current.SetSelectedGameObject(firstSelect);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (Keyboard.current.oKey.isPressed && !isOption)
        {
            isOption = true;
        }
    }
    public void OnClickStory()
    {
        select.SetActive(false);
        chapterSelect.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstChapter);
    }

    public void OnClickChapter(Text SelectchapterName)
    {
        chapterSelect.SetActive(false);
        episodeSelect.SetActive(true);
        chapterName.text = SelectchapterName.text;
        EventSystem.current.SetSelectedGameObject(firstEpisode);
    }

    public void OnClickEpisode(Text buttonText)
    {
        selectTextName = buttonText.text;
        AudioManager2D.Instance.AudioBgm.Stop();
        SceneManager.LoadScene(selectTextName);
    }

    public void OnClickBack()
    {
        if (episodeSelect.activeInHierarchy)
        {
            episodeSelect.SetActive(false);
            chapterSelect.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstChapter);
        }
        else if (chapterSelect.activeInHierarchy)
        {
            chapterSelect.SetActive(false);
            select.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelect);
        }
        else if (battleModeSelect.activeInHierarchy)
        {
            battleModeSelect.SetActive(false);
            select.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelect);
        }
    }

    public void OnClickBattle()
    {
        battleModeSelect.SetActive(true);
        select.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstMode);
    }

    public void OnClickBattleMode(GameObject gameObject)
    {
        SELECT_BATTLE_MODE = gameObject.name;
        battleStartButton.SetActive(true);
    }

    public void OnClickStart()
    {
        AudioManager2D.Instance.AudioBgm.Stop();
        if (battleModeSelect.activeInHierarchy)
        {
            SceneManager.LoadScene("Battle");
        }
        
    }

    public void OnClickTitle()
    {
        AudioManager2D.Instance.AudioBgm.Stop();
        SceneManager.LoadScene("Title");
    }

    public void OnClickShop()
    {
        AudioManager2D.Instance.AudioBgm.Stop();
        SceneManager.LoadScene("Reinforcement");
    }
}
