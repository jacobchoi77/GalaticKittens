using System;
using Unity.Netcode;
using UnityEngine;

/*
    Script that controls how the boss is going to work,
    the different behaviours are set on different scripts.
    Here you can add new states
*/

public class BossController : NetworkBehaviour{
    [SerializeField] private int m_damage;

    [Header("States for the boss")]
    [SerializeField] private BossEnterState m_enterState;
    [SerializeField] private BaseBossState m_fireState;
    [SerializeField] private BaseBossState m_misileBarrageState;
    [SerializeField] private BaseBossState m_idleState;
    [SerializeField] private BaseBossState m_deathState;
    [Header("For testing the boss states -> false for production")]
    [SerializeField] private bool m_isTesting;
    [SerializeField] private BossState m_testState;

    private BossUI bossUI;

    private void OnTriggerEnter2D(Collider2D collider){
        if (collider.TryGetComponent(out PlayerShipController playerShip)){
            playerShip.Hit(m_damage);
        }
    }

    public void OnHit(int currentHealth){
        bossUI.UpdateUI(currentHealth);
    }

    public void StartBoss(Vector3 initialPositionForEnterState){
        m_enterState.initialPosition = initialPositionForEnterState;
        SetState(BossState.enter);
    }

    public void SetState(BossState state){
        if (!IsServer) return;
        switch (state){
            case BossState.enter:
                m_enterState.RunState();
                break;

            case BossState.fire:
                m_fireState.RunState();
                break;

            case BossState.misileBarrage:
                m_misileBarrageState.RunState();
                break;

            case BossState.idle:
                m_idleState.RunState();
                break;

            case BossState.death:
                m_enterState.StopState();
                m_fireState.StopState();
                m_misileBarrageState.StopState();
                m_idleState.StopState();
                m_deathState.RunState();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void SetUI(BossUI bossUI){
        if (!IsServer) return;
        var bossHealth = GetComponentInChildren<BossHealth>();
        this.bossUI = bossUI;
        bossUI.SetHealth(bossHealth.Health);
    }

    override public void OnNetworkSpawn(){
        if (IsServer && m_isTesting){
            SetState(m_testState);
        }
    }
}