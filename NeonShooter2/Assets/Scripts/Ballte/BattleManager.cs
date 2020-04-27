﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager : MonoBehaviour
{

    public delegate void PauseFunc();
    public static PauseFunc StartPause;
    public static PauseFunc EndPause;

    [SerializeField]
    BattleCanvas MyBattleCanvas;
    [SerializeField]
    Camera MyCamera;
    [SerializeField]
    EnemyRole EnemyPrefab;
    [SerializeField]
    PlayerRole PlayerPrefab;
    [SerializeField]
    Debugger DebuggerPrefab;
    [SerializeField]
    public Vector2 WindForce;
    [SerializeField]
    public int CriticalScore;
    [SerializeField]
    public int ShieldStrikeScore;

    public static BattleManager MySelf;
    public static PlayerRole MyPlayerRole;
    public static EnemyRole MyEnemyRole;
    public static bool IsPause { get; private set; }
    public static bool IsRevived { get; private set; }
    public static bool CallingAd { get; private set; }
    public static int StrikeTimes { get; protected set; }
    public static int WeaknessStrikeTimes { get; protected set; }
    public static int MaxComboStrikes { get; protected set; }
    public static int ShootTimes { get; protected set; }
    public static float Accuracy { get { return MyMath.Calculate_ReturnFloat(WeaknessStrikeTimes, ShootTimes, Operator.Divided); } }
    public static int Kill { get; protected set; }
    public static int Score { get; protected set; }
    public static int HighestScoring { get; protected set; }
    public static bool CanPressSettingBtn { get; protected set; }


    void Awake()
    {
        if (!Debugger.IsSpawn)
            DeployDebugger();
        if (!GameDictionary.IsInit)
            GameDictionary.InitDic();
        IsRevived = false;
        IsPause = false;
        CanPressSettingBtn = true;
        MySelf = transform.GetComponent<BattleManager>();
        HideBounceWall();
        SetNormanWall();
        StartGame();
    }

    static void ResetScore()
    {
        StrikeTimes = 0;
        WeaknessStrikeTimes = 0;
        MaxComboStrikes = 0;
        ShootTimes = 0;
        Kill = 0;
        Score = 0;
        HighestScoring = Player.BestScore;
        Level = 0;
    }
    public static void CallAD()
    {
        CallingAd = true;
        UnityAD.ShowRewardedVideo();
    }
    public static void FailToRevive()
    {
        if (!IsRevived)
            PopupUI.ShowWarning("CancelWatchAD", "FailToRevive");
    }
    public static void Revive()
    {
        if (!CallingAd)
            return;
        CallingAd = false;
        IsRevived = true;
        CanPressSettingBtn = true;
        DestroyAllAmmo();
        BattleCanvas.CallSettle(false);
        MyPlayerRole.Revive();
        PlayerRole.SetCanShoot(true);
        SetPause(false);
    }
    // Use this for initialization
    public static void StartGame()
    {
        ClearGame();
        MySelf.SpawnRoles();
        MySelf.MyBattleCanvas.Init(MyPlayerRole, MyEnemyRole);
        BattleCanvas.SetStage();
        SetPause(true);
        BattleCanvas.StartGame();
    }
    public static void ReStartGame()
    {
        ClearGame();
        MySelf.SpawnRoles();
        MySelf.MyBattleCanvas.Init(MyPlayerRole, MyEnemyRole);
        BattleCanvas.ReStartGame();
        MyPlayerRole.StartConditionRefresh();
        MyEnemyRole.StartConditionRefresh();
        SetPause(true);
        BattleCanvas.StartGame();
    }
    public static void ClearGame()
    {
        ResetScore();
        MySelf.ClearEnemyRole();
        MySelf.ClearPlayerRoles();
    }
    public static void NextGame()
    {
        MySelf.ClearEnemyRole();
        MySelf.SpanwEnemyRole();
        DestroyAllAmmo();
        MyPlayerRole.SetTarget(MyEnemyRole);
        MySelf.MyBattleCanvas.Init(MyPlayerRole, MyEnemyRole);
        BattleCanvas.ReStartGame();
        MyPlayerRole.StartConditionRefresh();
        MyEnemyRole.StartConditionRefresh();
        SetPause(false);
        CanPressSettingBtn = true;
    }
    void SpanwEnemyRole()
    {
        //Spawn Enemy
        GameObject enemyGo = Instantiate(EnemyPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
        MyEnemyRole = enemyGo.GetComponent<EnemyRole>();
        enemyGo.transform.SetParent(transform);
        //Init EnemyData
        Dictionary<string, object> enemyDataDic = new Dictionary<string, object>();
        enemyDataDic.Add("Health", 30 + Level * 4);
        enemyDataDic.Add("Attack", 20 + Level * 4);
        enemyDataDic.Add("Camera", MyCamera);
        int ammoNum = Mathf.RoundToInt(Level / 2) + 1;
        int ammoBounceTimes = 0;
        if (ammoNum > 6)
        {
            ammoNum = 6;
            ammoBounceTimes = 1;
        }
        enemyDataDic.Add("AmmoNum", ammoNum);
        enemyDataDic.Add("AmmoBounceTimes", ammoBounceTimes);
        MyEnemyRole.Init(enemyDataDic);
    }
    void SpawnRoles()
    {
        //Spawn Enemy
        GameObject enemyGo = Instantiate(EnemyPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
        MyEnemyRole = enemyGo.GetComponent<EnemyRole>();
        enemyGo.transform.SetParent(transform);
        //Spawn Player
        GameObject playerGo = Instantiate(PlayerPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
        MyPlayerRole = playerGo.GetComponent<PlayerRole>();
        playerGo.transform.SetParent(transform);

        //Init EnemyData
        Dictionary<string, object> enemyDataDic = new Dictionary<string, object>();
        enemyDataDic.Add("Health", 30 + Level * 4);
        enemyDataDic.Add("Attack", 20 + Level * 4);
        enemyDataDic.Add("Camera", MyCamera);
        int ammoNum = Mathf.RoundToInt(Level / 2) + 1;
        int ammoBounceTimes = 0;
        if (ammoNum > 6)
        {
            ammoNum = 6;
            ammoBounceTimes = 1;
        }
        enemyDataDic.Add("AmmoNum", ammoNum);
        enemyDataDic.Add("AmmoBounceTimes", ammoBounceTimes);
        MyEnemyRole.Init(enemyDataDic);
        //Init PlayerData
        Dictionary<string, object> playerDataDic = new Dictionary<string, object>();
        playerDataDic.Add("Health", 50);
        playerDataDic.Add("Attack", 25);
        playerDataDic.Add("AmmoBounceTimes", 2);
        playerDataDic.Add("AmmoBounceDamage", 0);
        playerDataDic.Add("DecreaseEnemyAmmo", 0);
        playerDataDic.Add("ShieldLevel", 0);
        playerDataDic.Add("Camera", MyCamera);
        playerDataDic.Add("Target", MyEnemyRole);
        MyPlayerRole.Init(playerDataDic);
    }
    void ClearPlayerRoles()
    {
        if (MyPlayerRole != null)
            MyPlayerRole.SelfDestroy();
    }
    void ClearEnemyRole()
    {
        if (MyEnemyRole != null)
            MyEnemyRole.SelfDestroy();
    }
    public static void DestroyAllAmmo()
    {
        if (MyPlayerRole != null)
            MyPlayerRole.MyAmmoSpawner.DestroyAllAmmos();
        if (MyEnemyRole != null)
            MyEnemyRole.MyAmmoSpawner.DestroyAllAmmos();
    }
    public static void AddToStartPauseFnc(PauseFunc _pf)
    {
        StartPause += _pf;
    }
    public static void RemoveFromStartPauseFnc(PauseFunc _pf)
    {
        if (StartPause != null)
            StartPause -= _pf;
    }
    public static void AddToEndPauseFnc(PauseFunc _pf)
    {
        EndPause += _pf;
    }
    public static void RemoveFromEndPauseFnc(PauseFunc _pf)
    {
        if (EndPause != null)
            EndPause -= _pf;
    }
    public static void SetPause(bool _pause)
    {
        if (IsPause == _pause)
            return;
        IsPause = _pause;
        if (IsPause)
        {
            if (StartPause != null)
                StartPause();
        }
        else
        {
            if (EndPause != null)
                EndPause();
        }
    }
    public static void SetRecord(string _type, int _value, Operator _operator)
    {
        switch (_type)
        {
            case "StrikeTimes":
                StrikeTimes = MyMath.Calculate_ReturnINT(StrikeTimes, _value, _operator);
                Score += MySelf.ShieldStrikeScore;

                break;
            case "WeaknessStrikeTimes":
                WeaknessStrikeTimes = MyMath.Calculate_ReturnINT(WeaknessStrikeTimes, _value, _operator);
                Score += MySelf.CriticalScore;
                break;
            case "MaxComboStrikes":
                MaxComboStrikes = MyMath.Calculate_ReturnINT(MaxComboStrikes, _value, _operator);
                Score += MaxComboStrikes;
                break;
            case "ShootTimes":
                ShootTimes = MyMath.Calculate_ReturnINT(ShootTimes, _value, _operator);
                break;
            case "Kill":
                Kill = MyMath.Calculate_ReturnINT(Kill, _value, _operator);
                break;
            case "Score":
                Score = MyMath.Calculate_ReturnINT(Score, _value, _operator);
                break;
            case "HighestScoring":
                HighestScoring = MyMath.Calculate_ReturnINT(HighestScoring, _value, _operator);
                break;
        }
    }
    void DeployDebugger()
    {
        GameObject debugGo = Instantiate(DebuggerPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
        debugGo.transform.position = Vector3.zero;
    }
    public static void CheckAliveAmmoToContinueShoot()
    {
        if (MyPlayerRole.MyAmmoSpawner.CheckAlifeAmmo())
            return;
        if (MyEnemyRole.MyAmmoSpawner.CheckAlifeAmmo())
            return;
        HideBounceWall();
        PlayerRole.SetCanShoot(true);
    }
}
