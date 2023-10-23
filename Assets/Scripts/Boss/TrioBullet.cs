using Unity.Netcode;
using UnityEngine;

public class TrioBullet : NetworkBehaviour{
    [SerializeField] private GameObject _smallBulletPrefab;

    [SerializeField] private Transform[] _firePositions;

    private void SpawnBullets(){
        foreach (var firePosition in _firePositions){
            var newBullet = Instantiate(
                _smallBulletPrefab,
                firePosition.position,
                firePosition.rotation);

            newBullet.GetComponent<NetworkObject>().Spawn();
        }

        NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);
    }

    override public void OnNetworkSpawn(){
        if (IsServer) SpawnBullets();
    }
}