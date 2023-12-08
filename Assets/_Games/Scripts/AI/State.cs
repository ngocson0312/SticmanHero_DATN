using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperFight;
public abstract class State
{
    public string stateName;
    protected Controller controller;
    protected Core core;
    protected Transform transform;
    public State(Controller controller, string stateName)
    {
        this.controller = controller;
        this.stateName = stateName;
        this.core = controller.core;
        this.transform = controller.transform;
    }
    public abstract void EnterState();
    public abstract void UpdateLogic();
    public abstract void UpdatePhysic();
    public abstract void ExitState();
}
