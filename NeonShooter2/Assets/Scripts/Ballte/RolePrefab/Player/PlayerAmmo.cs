﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAmmo : AmmoPrefab
{
    [SerializeField]
    SpriteRenderer MyAmmoStar;
    [SerializeField]
    List<Sprite> AmmoLevelSprites;


    public int AmmoBounceDamage { get; protected set; }
    public float DamageFactor { get; protected set; }
    public override int Damage { get { return (int)((BaseDamage + CurBounceTimes * AmmoBounceDamage) * DamageFactor); } }
    public HitCondition AmmoHitCondition { get; protected set; }
    public bool IsHitTarget = false;
    public float Dragproportion { get; protected set; }
    public int AmmoLevel { get; protected set; }


    public override void Init(Dictionary<string, object> _dic)
    {
        base.Init(_dic);
        AmmoBounceDamage = (int)_dic["AmmoBounceDamage"];
        Dragproportion = (float)_dic["DragProportion"];
        Force = (Vector3)_dic["Force"];
        CurBounceTimes = 0;
        DamageFactor = 1;
        AmmoHitCondition = HitCondition.NoHit;
        AmmoLevel = 0;
    }
    public override void Launch()
    {
        base.Launch();
        EffectEmitter.EmitParticle("trail_shiny", Vector3.zero, Vector3.zero, transform);
        BattleManager.SetRecord("ShootTimes", 1, Operator.Plus);
        MyAudio.PlaySound("sfx_shoot");
    }
    protected override void OnTriggerEnter2D(Collider2D _col)
    {
        base.OnTriggerEnter2D(_col);
        switch (_col.gameObject.tag)
        {
            case "BounceWall":
                MyAudio.PlaySound(HitHardWallAduio);
                PowerUp();
                MyRigi.velocity = _col.GetComponent<BounceWallObj>().GetVelocity(MyRigi.velocity);
                break;
            case "LeftCol":
                MyAudio.PlaySound(HitWallAduio);
                EffectEmitter.EmitParticle("bounceEffect", transform.position, new Vector3(0, 0, 180), null);
                CameraPrefab.DoAction("Shake", 0);
                if (CurBounceTimes == 0)
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetDragForce(MyRigi.velocity, Dragproportion);
                if (Bounce())
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetVelocity(MyRigi.velocity);
                break;
            case "RightCol":
                MyAudio.PlaySound(HitWallAduio);
                EffectEmitter.EmitParticle("bounceEffect", transform.position, Vector3.zero, null);
                CameraPrefab.DoAction("Shake", 0);
                if (CurBounceTimes == 0)
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetDragForce(MyRigi.velocity, Dragproportion);
                if (Bounce())
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetVelocity(MyRigi.velocity);
                break;
            case "TopCol":
                MyAudio.PlaySound(HitWallAduio);
                EffectEmitter.EmitParticle("bounceEffect", transform.position, new Vector3(0, 0, 90), null);
                CameraPrefab.DoAction("Shake", 0);
                if (CurBounceTimes == 0)
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetDragForce(MyRigi.velocity, Dragproportion);
                if (Bounce())
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetVelocity(MyRigi.velocity);
                break;
            case "BotCol":
                MyAudio.PlaySound(HitWallAduio);
                EffectEmitter.EmitParticle("bounceEffect", transform.position, new Vector3(0, 0, 270), null);
                CameraPrefab.DoAction("Shake", 0);
                if (CurBounceTimes == 0)
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetDragForce(MyRigi.velocity, Dragproportion);
                if (Bounce())
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetVelocity(MyRigi.velocity);
                break;
            case "EnemyShield":
                if (IsHitTarget)
                    return;
                MyAudio.PlaySound(HitShieldAduio);
                Vector2 effectPos = Vector2.Lerp(BattleManager.MyEnemyRole.transform.position, transform.position, 0.8f);
                EffectEmitter.EmitParticle("shieldhit", effectPos, new Vector3(0, 0, 180 - MyMath.GetAngerFormTowPoint2D(BattleManager.MyEnemyRole.transform.position, transform.position)), null);
                BattleManager.MyEnemyRole.ShieldBeSruck(Damage);

                AmmoHitCondition = HitCondition.HitShell;
                SelfDestroy();
                IsHitTarget = true;
                break;
            case "Monster":
                if (IsHitTarget)
                    return;
                MyAudio.PlaySound(HitAduio);
                EffectEmitter.EmitParticle("bloodEffect", transform.position, new Vector3(0, 0, 180 - MyMath.GetAngerFormTowPoint2D(BattleManager.MyEnemyRole.transform.position, transform.position)), null);
                BattleManager.MyEnemyRole.BeStruck(Damage);

                AmmoHitCondition = HitCondition.Hit;
                SelfDestroy();
                IsHitTarget = true;
                break;
            default:
                break;
        }
    }
    public override void PowerUp()
    {
        base.PowerUp();
        DamageFactor += 1f;
        MaxBounceTimes++;
        AmmoLevel++;
        if (AmmoLevel<AmmoLevelSprites.Count)
            MyAmmoStar.sprite = AmmoLevelSprites[AmmoLevel];
    }
    bool Bounce()
    {
        CurBounceTimes++;
        if (CurBounceTimes > MaxBounceTimes)
        {
            SelfDestroy();
            return false;
        }
        return true;
    }
    void Rotate()
    {
        MyRigi.AddTorque(300);
    }
    public override void SelfDestroy()
    {
        //EnemyRole.SetAllMonsterUnarm();
        switch(AmmoHitCondition)
        {
            case HitCondition.Hit:
                BattleManager.SetRecord("StrikeTimes", 1, Operator.Plus);
                BattleManager.SetRecord("MaxComboStrikes", 1, Operator.Plus);
                BattleManager.SetRecord("WeaknessStrikeTimes", 1, Operator.Plus);
                if (BattleManager.MaxComboStrikes > 1)
                    BattleCanvas.ShowScoreOnEnemy(string.Format("{0}x{1}", GameDictionary.String_UIDic["CriticalCombo"].GetString(Player.UseLanguage), BattleManager.MaxComboStrikes), string.Format("{0}{1}","+",BattleManager.MySelf.CriticalScore + BattleManager.MaxComboStrikes));
                else
                    BattleCanvas.ShowScoreOnEnemy(GameDictionary.String_UIDic["CriticalHit"].GetString(Player.UseLanguage), string.Format("{0}{1}", "+", BattleManager.MySelf.CriticalScore));
                break;
            case HitCondition.HitShell:
                BattleManager.SetRecord("MaxComboStrikes", 0, Operator.Equal);
                BattleCanvas.ShowScoreOnEnemy(GameDictionary.String_UIDic["ShieldHit"].GetString(Player.UseLanguage), string.Format("{0}{1}","+",BattleManager.MySelf.ShieldStrikeScore));
                break;
            case HitCondition.NoHit:
                BattleManager.SetRecord("MaxComboStrikes", 0, Operator.Equal);
                break;
        }
            
        base.SelfDestroy();
    }
}
