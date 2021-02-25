using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager2D : MonoBehaviour
{
    private static AudioManager2D instance;
    public static AudioManager2D Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject(typeof(AudioManager2D).Name);
                instance = obj.AddComponent<AudioManager2D>();
            }
            return instance;
        }
    }

    private const string KEY_AUDIO_VOLUME_MASTER = "key_audio_volume_master";
    private const string KEY_AUDIO_VOLUME_BGM = "key_audio_volume_bgm";
    private const string KEY_AUDIO_VOLUME_SE = "key_audio_volume_se";

    private const string PARAM_VOLUME_MASTER = "VolumeMaster";
    private const string PARAM_VOLUME_BGM = "VolumeBgm";
    private const string PARAM_VOLUME_SE = "VolumeSE";

    public AudioSource AudioBgm { get; private set; }
    public AudioSource AudioSE { get; private set; }

    public float VolumeMaster
    {
        get
        {
            audioMixer.GetFloat(PARAM_VOLUME_MASTER, out float currentVolume);
            return currentVolume;
        }
        set 
        { audioMixer.SetFloat(PARAM_VOLUME_MASTER, value);}
    }

    public float VolumeBgm
    {
        get
        {
            audioMixer.GetFloat(PARAM_VOLUME_BGM, out float currentVolume);
            return currentVolume;
        }
        set 
        {　audioMixer.SetFloat(PARAM_VOLUME_BGM, value);}
    }

    public float VolumeSE
    {
        get
        {
            audioMixer.GetFloat(PARAM_VOLUME_SE, out float currentVolume);
            return currentVolume;
        }
        set 
        { audioMixer.SetFloat(PARAM_VOLUME_SE, value);}
    }

    private AudioMixer audioMixer;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        AudioBgm = gameObject.AddComponent<AudioSource>();
        AudioSE = gameObject.AddComponent<AudioSource>();
        AudioBgm.playOnAwake = false;
        AudioBgm.loop = true;
        AudioSE.playOnAwake = false;

        //AudioSourceにAudioMixerをセット
        audioMixer = Resources.Load<AudioMixer>("AudioMixer");
        if (audioMixer)
        {
            AudioBgm.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];
            AudioSE.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SE")[0];

            //AudioMixerが見つかった場合のみ設定値の読み込み
            LoadParam();
        }
        else
        {
            Debug.LogError("Resources/AudioMixer が見つかりません");
        }


    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadParam()
    {
        //PlayerPrefsの保存値取得（保存されていない場合はAudioMixerの値を初期値として使用）
        var volumeMaster = PlayerPrefs.GetFloat(KEY_AUDIO_VOLUME_MASTER, VolumeMaster);
        var volumeBgm = PlayerPrefs.GetFloat(KEY_AUDIO_VOLUME_BGM, VolumeBgm);
        var volumeSE = PlayerPrefs.GetFloat(KEY_AUDIO_VOLUME_SE, VolumeSE);

        VolumeMaster = volumeMaster;
        VolumeBgm = volumeBgm;
        VolumeSE = volumeSE;
    }

    public void SaveParam()
    {
        PlayerPrefs.SetFloat(KEY_AUDIO_VOLUME_MASTER,VolumeMaster);
        PlayerPrefs.SetFloat(KEY_AUDIO_VOLUME_BGM,VolumeBgm);
        PlayerPrefs.SetFloat(KEY_AUDIO_VOLUME_SE,VolumeSE);
        PlayerPrefs.Save();
    }

   
}
