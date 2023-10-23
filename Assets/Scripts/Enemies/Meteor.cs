using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Meteor : NetworkBehaviour, IDamagable{

    [SerializeField] private int m_damage = 1;
    [SerializeField] private int m_health = 1;
    [SerializeField] private GameObject m_vfxExplosion;
    [SerializeField] private Sprite[] m_meteors;
    [SerializeField] private GameObject m_meteorSprite;
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    [SerializeField] private float m_hitEffectDuration = 0.2f;

    [Header("Range for random scale value")]
    [SerializeField] private float m_scaleMin = 0.8f;
    [SerializeField] private float m_scaleMax = 1.5f;
    private readonly static int Hit1 = Shader.PropertyToID("_Hit");

    private void Start(){
        // Randomly select the sprite to use 
        m_spriteRenderer.GetComponent<SpriteRenderer>().sprite =
            m_meteors[Random.Range(0, m_meteors.Length)];

        // Randomly scale the meteor
        var randomScale = Random.Range(m_scaleMin, m_scaleMax);
        transform.localScale = new Vector3(randomScale, randomScale, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collider){
        if (!IsServer)
            return;

        if (collider.TryGetComponent(out IDamagable damageable)){
            // Hit the object that collide with me
            damageable.Hit(m_damage);

            // Hit me too!
            Hit(m_damage);
        }
    }

    public IEnumerator HitEffect(){
        var active = false;
        var timer = 0f;
        while (timer < m_hitEffectDuration){
            active = !active;
            m_spriteRenderer.material.SetInt(Hit1, active ? 1 : 0);

            yield return new WaitForEndOfFrame();

            timer += Time.deltaTime;
        }

        m_spriteRenderer.material.SetInt(Hit1, 0);
    }

    public void Hit(int damage){
        m_health -= damage;
        if (m_health <= 0){
            PowerUpSpawnController.Instance.OnPowerUpSpawn(transform.position);
            NetworkObjectSpawner.SpawnNewNetworkObject(m_vfxExplosion, transform.position);
            NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);
        }

        StopCoroutine(HitEffect());
        StartCoroutine(HitEffect());
    }
}