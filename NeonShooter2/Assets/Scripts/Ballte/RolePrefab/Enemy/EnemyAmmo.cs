﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAmmo : AmmoPrefab
{
    [SerializeField]
    float LifeTime;
    [SerializeField]
    protected float AngularVlocity = 3f;
    [SerializeField]
    SpriteRenderer AmmoSR;

    protected float Radius { get; set; }
    protected float StartRadian;
    protected float CurRadian;
    protected Vector3 ShootPos;
    protected float AmmoSpeed;
    ParticleSystem TrailEffect;

    public override void Init(Dictionary<string, object> _dic)
    {
        base.Init(_dic);
        ShootPos = (Vector3)_dic["ShooterPos"];
        AmmoSpeed = (int)_dic["AmmoSpeed"];
    }
    public void SetCircularMotion(float _radius, float _startAngle)
    {
        Radius = _radius;
        StartRadian = _startAngle;
        CurRadian = StartRadian;
        CircularMotion();
    }
    protected override void Update()
    {
        if (BattleManager.IsPause)
            return;
        base.Update();
        CircularMotion();
        LifeTimeCountDown();
    }
    protected override void OnTriggerEnter2D(Collider2D _col)
    {
        if (!IsLaunching)
            return;
        base.OnTriggerEnter2D(_col);
        switch (_col.gameObject.tag)
        {
            case "Player":
                MyAudio.PlaySound(HitAduio);
                EffectEmitter.EmitParticle("bloodEffect", transform.position, new Vector3(0, 0, 180 - MyMath.GetAngerFormTowPoint2D(BattleManager.MyPlayerRole.transform.position, transform.position)), null);
                BattleManager.MyPlayerRole.BeStruck(Damage);
                SelfDestroy();
                break;
            case "PlayerShield":
                MyAudio.PlaySound(HitShieldAduio);
                EffectEmitter.EmitParticle("shieldhit", transform.position, new Vector3(0, 0, 180 - MyMath.GetAngerFormTowPoint2D(BattleManager.MyPlayerRole.transform.position, transform.position)), null);
                BattleManager.MyPlayerRole.ShieldBeSruck(Damage);
                SelfDestroy();
                break;


            case "LeftCol":
                MyAudio.PlaySound(HitWallAduio);
                EffectEmitter.EmitParticle("bounceEffect", transform.position, new Vector3(0, 0, 180), null);
                CameraPrefab.DoAction("Shake", 0);
                if (CurBounceTimes == 0)
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetDragForce(MyRigi.velocity, 0);
                if (Bounce())
                {
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetVelocity(MyRigi.velocity);
                    Vector2 dir = MyRigi.velocity;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
                    AmmoSR.transform.rotation = Quaternion.Euler(AmmoSR.transform.localRotation.x, AmmoSR.transform.localRotation.y, angle);
                    TrailEffect.transform.rotation = Quaternion.Euler(AmmoSR.transform.localRotation.x, AmmoSR.transform.localRotation.y, angle);
                }

                break;
            case "RightCol":
                MyAudio.PlaySound(HitWallAduio);
                EffectEmitter.EmitParticle("bounceEffect", transform.position, Vector3.zero, null);
                CameraPrefab.DoAction("Shake", 0);
                if (CurBounceTimes == 0)
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetDragForce(MyRigi.velocity, 0);
                if (Bounce())
                {
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetVelocity(MyRigi.velocity);
                    Vector2 dir = MyRigi.velocity;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
                    AmmoSR.transform.rotation = Quaternion.Euler(AmmoSR.transform.localRotation.x, AmmoSR.transform.localRotation.y, angle);
                    TrailEffect.transform.rotation = Quaternion.Euler(AmmoSR.transform.localRotation.x, AmmoSR.transform.localRotation.y, angle);
                }
                break;
            case "TopCol":
                MyAudio.PlaySound(HitWallAduio);
                EffectEmitter.EmitParticle("bounceEffect", transform.position, new Vector3(0, 0, 90), null);
                CameraPrefab.DoAction("Shake", 0);
                if (CurBounceTimes == 0)
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetDragForce(MyRigi.velocity, 0);
                if (Bounce())
                {
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetVelocity(MyRigi.velocity);
                    Vector2 dir = MyRigi.velocity;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
                    AmmoSR.transform.rotation = Quaternion.Euler(AmmoSR.transform.localRotation.x, AmmoSR.transform.localRotation.y, angle);
                    TrailEffect.transform.rotation = Quaternion.Euler(AmmoSR.transform.localRotation.x, AmmoSR.transform.localRotation.y, angle);
                }

                break;
            case "BotCol":
                MyAudio.PlaySound(HitWallAduio);
                EffectEmitter.EmitParticle("bounceEffect", transform.position, new Vector3(0, 0, 270), null);
                CameraPrefab.DoAction("Shake", 0);
                if (CurBounceTimes == 0)
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetDragForce(MyRigi.velocity, 0);
                if (Bounce())
                {
                    MyRigi.velocity = _col.GetComponent<NormalWallObj>().GetVelocity(MyRigi.velocity);
                    Vector2 dir = MyRigi.velocity;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
                    AmmoSR.transform.rotation = Quaternion.Euler(AmmoSR.transform.localRotation.x, AmmoSR.transform.localRotation.y, angle);
                    TrailEffect.transform.rotation= Quaternion.Euler(AmmoSR.transform.localRotation.x, AmmoSR.transform.localRotation.y, angle);
                }
                break;
            default:
                break;
        }
    }

    void LifeTimeCountDown()
    {
        if (!IsLaunching)
            return;
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0)
            SelfDestroy();
    }
    void CircularMotion()
    {
        if (IsLaunching)
            return;
        CurRadian += AngularVlocity * Time.deltaTime;
        float x = Radius * Mathf.Cos(CurRadian) + ShootPos.x;
        float y = Radius * Mathf.Sin(CurRadian) + ShootPos.y;
        transform.position = new Vector2(x, y);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 - MyMath.GetAngerFormTowPoint2D(ShootPos, transform.position)));
    }
    public override void Launch()
    {
        base.Launch();
        TrailEffect=EffectEmitter.EmitParticle("trail_arrow", Vector3.zero, Vector3.zero, transform);
        Force = (transform.position - ShootPos).normalized * AmmoSpeed;
        MyRigi.AddForce(Force);
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
}
