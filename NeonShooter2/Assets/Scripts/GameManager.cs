using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{

    public static bool IsInit { get; protected set; }
    [SerializeField]
    Debugger DebuggerPrefab;
    [SerializeField]
    PopupUI PopUIPrefab;
    [SerializeField]
    ServerRequest SR;
    [SerializeField]
    protected AudioClip BGM;
    [SerializeField]
    protected AudioPlayer MyAudio;

    void Awake()
    {
        Screen.fullScreen = true;
    }
    void Start()
    {
        if (!Debugger.IsSpawn)
            DeployDebugger();
        if (!PopupUI.IsInit)
            DeployPopupUI();
        if (!GameDictionary.IsInit)
            GameDictionary.InitDic();
        if (IsInit)
            return;
        MyAudio.PlayLoopSound(BGM, "bgm");
        Player.Init();
        DontDestroyOnLoad(gameObject);
        SR.Init();
        Player.AutoLogin();
        IsInit = true;
    }
    void DeployDebugger()
    {
        GameObject go = Instantiate(DebuggerPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
        go.transform.position = Vector3.zero;
    }
    void DeployPopupUI()
    {
        GameObject go = Instantiate(PopUIPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
        go.transform.position = Vector3.zero;
        PopupUI ppui = go.GetComponent<PopupUI>();
        ppui.Init();
    }
}
