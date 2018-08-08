﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AmmoPrefab : MonoBehaviour
{
    [SerializeField]
    protected AudioPlayer MyAudio;
    [SerializeField]
    protected AudioClip HitShieldAduio;
    [SerializeField]
    protected AudioClip HitAduio;
    [SerializeField]
    protected AudioClip HitWallAduio;
    [SerializeField]
    protected AudioClip HitHardWallAduio;
    [SerializeField]
    protected AudioClip FlyingAudio;
    [SerializeField]
    protected AudioClip SpeedyFlyingAudio;

    public int MaxBounceTimes { get; protected set; }
    public int CurBounceTimes { get; protected set; }
    public bool IsLaunching { get; protected set; }
    protected Vector3 Force;
    protected Rigidbody2D MyRigi;
    public virtual int BaseDamage { get; protected set; }
    public virtual int Damage { get { return BaseDamage; } }
    public bool IsDavestated = false;
    protected Vector2 SavedVelocity;
    protected float SavedAngularVelocity;
    public virtual void Init(Dictionary<string, object> _dic)
    {
        IsLaunching = false;
        MyRigi = transform.GetComponent<Rigidbody2D>();
        BaseDamage = (int)_dic["Damage"];
        MaxBounceTimes = (int)_dic["AmmoBounceTimes"];
        BattleManager.AddToStartPauseFnc(PauseGame);
        BattleManager.AddToEndPauseFnc(ResumeGame);
    }
    protected virtual void Update()
    {
        MyRigi.velocity += BattleManager.MySelf.WindForce * Time.deltaTime;
    }
    public virtual void Launch()
    {
        MyRigi.AddForce(Force);
        IsLaunching = true;
        if (FlyingAudio != null)
            MyAudio.PlayLoopSound(FlyingAudio, string.Format("{0}_{1}", name.ToString(), "FlyingAudio"));
    }
    protected virtual void OnTriggerEnter2D(Collider2D _col)
    {
    }
    public virtual void SelfDestroy()
    {
        if (FlyingAudio != null)
            MyAudio.StopLoopSound(string.Format("{0}_{1}", name.ToString(), "FlyingAudio"));
        if (SpeedyFlyingAudio != null)
            MyAudio.StopLoopSound(string.Format("{0}_{1}", name.ToString(), "SpeedyFlyingAudio"));
        IsDavestated = true;
        BattleManager.RemoveFromStartPauseFnc(PauseGame);
        BattleManager.RemoveFromEndPauseFnc(ResumeGame);
        BattleManager.CheckAliveAmmoToContinueShoot();
        Destroy(gameObject);
    }

    public virtual void PowerUp()
    {
        if (SpeedyFlyingAudio != null)
            MyAudio.PlayLoopSound(SpeedyFlyingAudio, string.Format("{0}_{1}", name.ToString(), "SpeedyFlyingAudio"));
        EffectEmitter.EmitParticle("trail_star", Vector3.zero, Vector3.zero, transform);
    }



    protected virtual void PauseGame()
    {
        SavedVelocity = MyRigi.velocity;
        SavedAngularVelocity = MyRigi.angularVelocity;
        MyRigi.velocity = Vector2.zero;
        MyRigi.angularVelocity = 0;
    }

    protected virtual void ResumeGame()
    {
        MyRigi.velocity = SavedVelocity;
        MyRigi.angularVelocity = SavedAngularVelocity;
    }
}
