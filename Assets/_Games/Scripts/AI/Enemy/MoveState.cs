using SuperFight;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    SpiderHanging spider;
    float moveTime;
    Tween sequence;
    private Vector3 firstPos;
    private Vector3 endPos;
    public MoveState(SpiderHanging controller, string stateName) : base(controller, stateName)
    {
        spider = controller;
        moveTime = spider.moveTime;
        spider.animatorHandle.OnEventAnimation += CriticalEvent;
        firstPos = controller.transform.position;
    }
    ~MoveState()
    {
        spider.animatorHandle.OnEventAnimation -= CriticalEvent;
    }
    private void CriticalEvent(string eventName)
    {
        if (eventName.Equals("CriticalTrue"))
        {
            spider.CiticalVfx.Play();
        }
    }

    void MoveParrtern1()
    {
        sequence = controller.transform.DOMove(firstPos, moveTime / 3).SetDelay(0.5f).OnComplete(MoveParrtern2);
    }

    void MoveParrtern2()
    {
        sequence = controller.transform.DOMove(endPos, moveTime).SetDelay(1.5f).SetEase(Ease.InOutBack).OnComplete(MoveParrtern1);
        controller.animatorHandle.PlayAnimation("Spider_Attack_Shot", 0.1f, 1, true);
    }

    public override void EnterState()
    {
        RaycastHit2D raycast = Physics2D.Raycast(firstPos, Vector2.down, 100f, LayerMask.GetMask("Ground"));

        endPos = controller.transform.position - new Vector3(0, spider.moveRange, 0);
        if (raycast)
        {
            endPos = raycast.point + Vector2.up;
        }
        spider.silk.positionCount = 2;
        spider.silk.SetPosition(0, firstPos);
        sequence = controller.transform.DOMove(endPos, moveTime).SetDelay(spider.firstDelay).SetEase(Ease.InOutBack).OnComplete(MoveParrtern1);
    }

    public override void ExitState()
    {
        sequence.Kill(false);
    }

    public override void UpdateLogic()
    {
        spider.silk.SetPosition(1, controller.transform.position);
    }

    public override void UpdatePhysic()
    {

    }
}
