using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class HomingMisile : NetworkBehaviour{

    [SerializeField] private int m_damage = 1;
    [SerializeField] private float m_startingSpeed = 4f;
    [SerializeField] private float m_followSpeed = 8f;
    [SerializeField] private float m_startingTime = 0.5f;
    [SerializeField] private float m_followTime = 2f;

    [Header("Set in runtime")]
    [HideInInspector]
    [SerializeField] private Transform m_targetToHit;

    private IEnumerator MissileHoming(){
        var timer = 0f;

        // Important: the axis we are using for the direction of move is the positive X, take this into account were using another prefab

        // Starting -> Going up.
        while (true){
            yield return new WaitForEndOfFrame();
            transform.Translate(Vector2.right * (m_startingSpeed * Time.deltaTime));
            timer += Time.deltaTime;
            if (timer > m_startingTime){
                break;
            }
        }

        timer = 0f;

        // Following -> Move towards the target
        while (true){
            yield return new WaitForEndOfFrame();

            // Safety check because maybe the target dies before i hit
            if (m_targetToHit != null){
                var transform1 = transform;
                var position = transform1.position;
                var rotation = transform1.rotation;
                var targetPosition = m_targetToHit.position;

                Vector2 dir = targetPosition - position;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                position = Vector2.MoveTowards(position, targetPosition, Time.deltaTime * m_followSpeed);
                rotation = Quaternion.Slerp(rotation, Quaternion.Euler(0f, 0f, angle), Time.deltaTime * 5f);

                transform1.position = position;
                transform1.rotation = rotation;
            }
            else{
                break;
            }

            timer += Time.deltaTime;
            if (timer > m_followTime){
                break;
            }
        }

        // Breaking -> stop following the target and just continue on the same direction
        while (true){
            yield return new WaitForEndOfFrame();
            transform.Translate(Vector2.right * (m_followSpeed * Time.deltaTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D collider){
        if (IsServer){
            if (collider.TryGetComponent(out IDamagable damageable)){
                collider.GetComponent<IDamagable>().Hit(m_damage);
                StopAllCoroutines();
                NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);
            }
        }
    }

    override public void OnNetworkSpawn(){
        if (IsServer){
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            m_targetToHit = players[Random.Range(0, players.Length)].transform;
            StartCoroutine(MissileHoming());
        }
    }
}