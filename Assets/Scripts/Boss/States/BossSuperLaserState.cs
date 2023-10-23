using System.Collections;
using UnityEngine;

public class BossSuperLaserState : BaseBossState{
    [SerializeField] private GameObject m_superLaserPrefab;

    [SerializeField] private Transform m_superLaserPosition;

    private IEnumerator FireSuperLaser(){
        var randomRotation = Random.Range(-40f, 10f);

        var superLaser = NetworkObjectSpawner.SpawnNewNetworkObject(
            m_superLaserPrefab,
            m_superLaserPosition.localPosition,
            Quaternion.Euler(0f, 0f, randomRotation)
        );

        // TODO: Wait the time the vfx last
        yield return new WaitForSeconds(5f);
        m_controller.SetState(BossState.idle);
    }

    override public void RunState(){
        StartCoroutine(FireSuperLaser());
    }

    override public void StopState(){
        StopCoroutine(FireSuperLaser());
    }

}