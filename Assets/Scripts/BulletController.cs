using Unity.Netcode;
using UnityEngine;

public class BulletController : NetworkBehaviour{
    private enum BulletOwner{
        Enemy,
        Player
    }

    public int damage = 1;

    [HideInInspector]
    public CharacterDataSO characterData;

    [SerializeField]
    private BulletOwner m_owner;

    public GameObject m_Owner{ get; set; }

    private void Start(){
        if (m_owner == BulletOwner.Player && IsServer){
            ChangeBulletColorClientRpc(characterData.color);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider){
        if (!IsServer) return;
        if (collider.TryGetComponent(out IDamagable damageable)){
            if (m_owner == BulletOwner.Player){
                // For the final score
                characterData.enemiesDestroyed++;
            }
            damageable.Hit(damage);
            NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);
        }
    }

    [ClientRpc]
    private void ChangeBulletColorClientRpc(Color newColor){
        GetComponent<SpriteRenderer>().color = newColor;
    }
}