using System.Collections;
using UnityEngine;

public class BossMisileBarrageState : BaseBossState{
    [SerializeField] private Transform[] m_misileSpawningArea;

    [SerializeField] private GameObject m_misilePrefab;

    [SerializeField]
    [Range(0f, 1f)]
    private float m_misileDelayBetweenSpawns;

    private IEnumerator RunMisileBarrageState(){
        // Spawn the missiles
        foreach (var spawnPosition in m_misileSpawningArea){
            FireMisiles(spawnPosition.position);
            yield return new WaitForSeconds(m_misileDelayBetweenSpawns);
        }

        // Go idle from a moment
        m_controller.SetState(BossState.idle);
    }

    // Spawn the missile prefab
    private void FireMisiles(Vector3 position){
        NetworkObjectSpawner.SpawnNewNetworkObject(
            m_misilePrefab,
            position,
            m_misilePrefab.transform.rotation);
    }

    // Run state
    override public void RunState(){
        StartCoroutine(RunMisileBarrageState());
    }
}