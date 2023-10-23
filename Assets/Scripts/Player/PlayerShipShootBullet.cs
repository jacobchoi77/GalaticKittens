using UnityEngine;
using Unity.Netcode;

public class PlayerShipShootBullet : NetworkBehaviour{

    [SerializeField] private int m_fireDamage;
    [SerializeField] private GameObject m_bulletPrefab;
    [SerializeField] private Transform m_cannonPosition;
    [SerializeField] private CharacterDataSO m_characterData;
    [SerializeField] private GameObject m_shootVfx;
    [SerializeField] private AudioClip m_shootClip;

    // Update is called once per frame
    private void Update(){
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.Space)){
            FireNewBulletServerRpc();
        }
    }

    [ServerRpc]
    private void FireNewBulletServerRpc(){
        SpawnNewBulletVfx();
        var newBullet = GetNewBullet();
        PrepareNewlySpawnedBullet(newBullet);
        PlayShootBulletSoundClientRpc();
    }

    private void SpawnNewBulletVfx(){
        NetworkObjectSpawner.SpawnNewNetworkObject(m_shootVfx, m_cannonPosition.position);
    }

    private GameObject GetNewBullet(){
        return NetworkObjectSpawner.SpawnNewNetworkObject(
            m_bulletPrefab,
            m_cannonPosition.position);
    }

    private void PrepareNewlySpawnedBullet(GameObject newBullet){
        var bulletController = newBullet.GetComponent<BulletController>();
        bulletController.damage = m_fireDamage;
        bulletController.characterData = m_characterData;
    }

    [ClientRpc]
    private void PlayShootBulletSoundClientRpc(){
        AudioManager.Instance.PlaySoundEffect(m_shootClip);
    }
}