using Unity.Netcode;
using UnityEngine;

public enum BossState{
    fire,
    misileBarrage,
    death,
    idle,
    enter
};

[RequireComponent(typeof(BossController))]
public class BaseBossState : NetworkBehaviour{
    protected BossController m_controller;

    private void Start(){
        m_controller = FindObjectOfType<BossController>();
    }

    public virtual void RunState(){ }

    public virtual void StopState(){
        StopAllCoroutines();
    }
}