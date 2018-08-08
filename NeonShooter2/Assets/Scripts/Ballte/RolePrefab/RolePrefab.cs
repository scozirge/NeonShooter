﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class RolePrefab : MonoBehaviour
{
    [SerializeField]
    protected Transform Trans_Shield;
    [SerializeField]
    protected AudioPlayer MyAudio;
    [SerializeField]
    protected AudioClip DieAduio;
    public int ShieldAngle { get; protected set; }
    public bool IsAlive { get; protected set; }
    protected Camera MyCamera;
    private int health;
    public int Health
    {
        get { return health; }
        set
        {
            if (value < 0)
                value = 0;
            health = value;
        }
    }
    public int MaxHealth { get; protected set; }
    public float HealthRatio { get { return (float)Health / (float)MaxHealth; } }
    public virtual int Attack { get { return BaseAttack + ExtraAttack; } }
    public int ExtraAttack { get; protected set; }
    public int BaseAttack { get; protected set; }
    public int BaseAmmoNum { get; protected set; }
    public virtual int AmmoNum { get { return BaseAmmoNum; } }
    public int AmmoBounceTimes { get; protected set; }
    public Force MyForce { get; protected set; }



    public virtual void Init(Dictionary<string, object> _dataDic)
    {
        IsAlive = true;
        Health = (int)_dataDic["Health"];
        MyCamera = _dataDic["Camera"] as Camera;
        BaseAttack = (int)_dataDic["Attack"];
        AmmoBounceTimes = (int)_dataDic["AmmoBounceTimes"];
        MaxHealth = Health;
        StartConditionRefresh();
    }
    public virtual void StartConditionRefresh()
    {
    }

    protected virtual void Start()
    {
    }
    protected virtual void Update()
    {
    }
    public virtual void BeStruck(int _dmg)
    {
        EffectEmitter.EmitParticle("hitEffect", transform.position, Vector3.zero, null);
        CameraPrefab.DoAction("Shake2", 0);
        ReceiveDmg(_dmg);
    }
    public virtual void ShieldBeSruck(int _dmg)
    {
    }
    public virtual void ReceiveDmg(int _dmg)
    {
        if (!IsAlive)
            return;
        Health -= _dmg;
        DeathCheck();
    }
    public virtual void HealHP(int _heal)
    {
        if (!IsAlive)
            return;
        Health += _heal;
    }
    public virtual void IncreaseMaxHP(int _heal)
    {
        if (!IsAlive)
            return;
        MaxHealth += _heal;
        Health += _heal;
    }
    protected virtual bool DeathCheck()
    {
        if (Health <= 0)
        {
            IsAlive = false;
            EffectEmitter.EmitParticle("deathEffect", transform.position, Vector3.zero, null);
            BattleCanvas.ShowRole(MyForce, false);
            MyAudio.PlaySound(DieAduio);
            BattleManager.DestroyAllAmmo();
        }
        else IsAlive = true;
        return !IsAlive;
    }
    public virtual void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
