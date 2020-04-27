﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public static bool IsInit;
    public static bool IsMute = false;
    static List<AudioSource> ASList;
    static GameObject MyAudioObject;
    static Dictionary<string, AudioSource> LoopAudioDic;
    AudioSource CurAS;

    void Awake()
    {
        if (!IsInit)
        {
            Init();
        }
    }
    void Init()
    {
        if (PlayerPrefs.GetInt("IsMute") == 0)
            Mute(false);
        else
            AudioPlayer.Mute(true);

        LoopAudioDic = new Dictionary<string, AudioSource>();
        ASList = new List<AudioSource>();
        MyAudioObject = new GameObject("AudioPlayer");
        DontDestroyOnLoad(MyAudioObject);
        MyAudioObject.AddComponent<AudioSource>();
        ASList.Add(MyAudioObject.GetComponent<AudioSource>());
        for (int i = 0; i < ASList.Count; i++)
        {
            if (!ASList[i].isPlaying)
                CurAS = ASList[i];
        }
        IsInit = true;
    }
    public static void Mute(bool _isMute)
    {
        if (_isMute == IsMute)
            return;
        IsMute = _isMute;
        if (IsMute)
        {
            PlayerPrefs.SetInt("IsMute", 1);
            if (LoopAudioDic != null)
                foreach (AudioSource value in LoopAudioDic.Values)
                {
                    value.volume = 0;
                }
        }
        else
        {
            PlayerPrefs.SetInt("IsMute", 0);
            if (LoopAudioDic != null)
                foreach (AudioSource value in LoopAudioDic.Values)
                {
                    value.volume = 1;
                }
        }

    }
    public void PlaySound(string _soundName)
    {
        if (IsMute)
            return;
        if (IsInit)
        {
            if (GetApplicableAudioSource() != null)
            {
                CurAS.loop = false;
                CurAS.clip = Resources.Load<AudioClip>(string.Format("Sounds/{0}", _soundName));
                CurAS.Play(0);
            }
            else
            {
                GetNewAudioSource();
                CurAS.loop = false;
                CurAS.clip = Resources.Load<AudioClip>(string.Format("Sounds/{0}", _soundName));
                CurAS.Play(0);
            }
        }
        else
        {
            Init();
            CurAS.loop = false;
            CurAS.Play(0);
        }
    }
    public void PlaySound(AudioClip _ac)
    {
        if (IsMute)
            return;
        if (IsInit)
        {
            if (GetApplicableAudioSource() != null)
            {
                CurAS.clip = _ac;
                CurAS.loop = false;
                CurAS.Play(0);
            }
            else
            {
                GetNewAudioSource();
                CurAS.loop = false;
                CurAS.clip = _ac;
                CurAS.Play(0);
            }
        }
        else
        {
            Init();
            CurAS.loop = false;
            CurAS.Play(0);
        }
    }
    public void StopLoopSound(string _key)
    {
        if (LoopAudioDic.ContainsKey(_key))
        {
            LoopAudioDic[_key].Stop();
            LoopAudioDic[_key].loop = false;
            LoopAudioDic.Remove(_key);
        }
        //else
        //Debug.LogWarning(string.Format("Key:{0}　不存在尋換播放音效清單中", _key));
    }
    public void PlayLoopSound(AudioClip _ac, string _key)
    {
        if (LoopAudioDic.ContainsKey(_key))
        {
            //Debug.LogWarning(string.Format("Key:{0} 循環播放音效索引重複", _key));
            return;
        }
        if (IsInit)
        {
            if (GetApplicableAudioSource() != null)
            {
                CurAS.loop = true;
                CurAS.clip = _ac;
                CurAS.Play();
            }
            else
            {
                GetNewAudioSource();
                CurAS.loop = true;
                CurAS.clip = _ac;
                CurAS.Play();
            }
        }
        else
        {
            Init();
            CurAS.loop = true;
            CurAS.Play();
        }
        if (IsMute)
            CurAS.volume = 0;
        else
            CurAS.volume = 1;
        LoopAudioDic.Add(_key, CurAS);
    }
    AudioSource GetApplicableAudioSource()
    {
        CurAS = null;
        for (int i = 0; i < ASList.Count; i++)
        {
            if (!ASList[i].isPlaying)
            {
                CurAS = ASList[i];
                return CurAS;
            }
        }
        return CurAS;
    }
    AudioSource GetNewAudioSource()
    {
        CurAS = MyAudioObject.AddComponent<AudioSource>();
        ASList.Add(CurAS);
        return CurAS;
    }
}
