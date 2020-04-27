using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyRoleUI : RoleUI
{
    [SerializeField]
    GameObject MyScoreObj;
    [SerializeField]
    Text ScoreTitle_Text;
    [SerializeField]
    Text ScoreValue_Text;
    [SerializeField]
    Animator ScoreAni;
    [SerializeField]
    Image HeadImg;
    [SerializeField]
    Image BodyImg;
    [SerializeField]
    Image BackImg;

    static Sprite[] Heads;
    static Sprite[] Bodys;
    static Sprite[] Backs;
    static bool IsLoadedSprites = false;

    private void OnEnable()
    {
        if (!IsLoadedSprites)
        {
            Heads = Resources.LoadAll<Sprite>("Images/Role/Head");
            Bodys = Resources.LoadAll<Sprite>("Images/Role/Body");
            Backs = Resources.LoadAll<Sprite>("Images/Role/Back");
            IsLoadedSprites = true;
        }
        int rand = Random.Range(0, Heads.Length);
        HeadImg.sprite = Heads[rand];
        rand = Random.Range(0, Bodys.Length);
        BodyImg.sprite = Bodys[rand];
        rand = Random.Range(0, Backs.Length);
        BackImg.sprite = Backs[rand];
    }
    public void ShowScore(string _str, string _score)
    {
        if (!BattleManager.MyEnemyRole.IsAlive)
            return;
        ScoreTitle_Text.text = _str;
        ScoreValue_Text.text = _score;
        PlayMotion("Play", 0);
    }
    public void PlayMotion(string _motion, float _normalizedTime)
    {
        switch (_motion)
        {
            case "Default":
                if (Animator.StringToHash(string.Format("Base Layer.{0}", _motion)) != ScoreAni.GetCurrentAnimatorStateInfo(0).fullPathHash)
                    ScoreAni.Play(_motion, 0, _normalizedTime);
                break;
            case "Play":
                if (Animator.StringToHash(string.Format("Base Layer.{0}", _motion)) != ScoreAni.GetCurrentAnimatorStateInfo(0).fullPathHash)
                    ScoreAni.Play(_motion, 0, _normalizedTime);
                else
                    ScoreAni.StopPlayback();//重播
                break;
            default:
                break;
        }
    }

}
