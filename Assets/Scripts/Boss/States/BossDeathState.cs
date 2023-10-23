using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BossDeathState : BaseBossState{
    [SerializeField] private int m_maxNumberOfExplosions;

    [SerializeField] private float m_explosionDuration;

    [SerializeField] private Transform m_explosionPositionsContainer;

    [SerializeField] private GameObject m_explosionVfx;

    [SerializeField]
    [Range(1f, 40f)]
    private float m_shakeSpeed;

    [SerializeField]
    [Range(0.1f, 2f)]
    private float m_shakeAmount;

    private List<Transform> explosionPositions = new List<Transform>();

    private void Start(){
        if (IsServer){
            // Add the explosions Positions 
            foreach (Transform transform in m_explosionPositionsContainer){
                explosionPositions.Add(transform);
            }
        }
    }

    private IEnumerator Shake(){
        var currentPositionx = transform.position.x;
        while (true){
            var shakeValue = Mathf.Sin(Time.time * m_shakeSpeed) * m_shakeAmount;

            transform.position = new Vector2(currentPositionx + shakeValue, transform.position.y);

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator RunDeath(){
        // Show various explosion vfx for some seconds
        var numberOfExplosions = 0;
        var stepDuration = m_explosionDuration / m_maxNumberOfExplosions;

        StartCoroutine(Shake());
        while (numberOfExplosions < m_maxNumberOfExplosions){
            var randPosition = explosionPositions[Random.Range(0, explosionPositions.Count)].position;

            NetworkObjectSpawner.SpawnNewNetworkObject(m_explosionVfx, randPosition);

            yield return new WaitForSeconds(stepDuration);

            numberOfExplosions++;
        }
        StopCoroutine(Shake());

        yield return new WaitForEndOfFrame();
        GameplayManager.Instance.BossDefeat();

        NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);
    }

    override public void RunState(){
        StartCoroutine(RunDeath());
    }
}