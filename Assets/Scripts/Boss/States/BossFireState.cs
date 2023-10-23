using System.Collections;
using UnityEngine;

public class BossFireState : BaseBossState{

    [SerializeField] private Transform[] _fireCannonSpawningArea;
    [SerializeField] private GameObject _trioBulletPrefab;
    [SerializeField] private GameObject _circularBulletPrefab;
    [SerializeField] private float _normalShootRateOfFire;
    [SerializeField] private float _idleSpeed;

    override public void RunState(){
        StartCoroutine(FireState());
    }

    private IEnumerator FireState(){
        var shootTimer = 0f;
        var normalStateTimer = 0f;
        var normalStateExitTime = Random.Range(7f, 21f);

        while (normalStateTimer <= normalStateExitTime){
            // Small movement on the boss
            transform.position = new Vector2(
                transform.position.x,
                Mathf.Sin(Time.time) * _idleSpeed);

            shootTimer += Time.deltaTime;
            if (shootTimer >= _normalShootRateOfFire){
                var nextBulletPrefabToShoot = GetNextBulletPrefabToShoot();

                FireBulletPrefab(nextBulletPrefabToShoot);

                shootTimer = 0f;
            }
            yield return new WaitForEndOfFrame();
            normalStateTimer += Time.deltaTime;
        }
        m_controller.SetState(BossState.misileBarrage);
    }

    private GameObject GetNextBulletPrefabToShoot(){
        var randomBulletChoice = Random.Range(0, 10);
        return randomBulletChoice < 7 ? _trioBulletPrefab : _circularBulletPrefab;
    }

    private void FireBulletPrefab(GameObject bulletPrefab){
        // Because the cannon positions are lower on the sprite with increase the rotation up
        var randomZrotation = Random.Range(-25f, 45f);

        foreach (var laserCannon in _fireCannonSpawningArea){
            NetworkObjectSpawner.SpawnNewNetworkObject(
                bulletPrefab,
                laserCannon.position,
                Quaternion.Euler(0f, 0f, randomZrotation)
            );
        }
    }
}