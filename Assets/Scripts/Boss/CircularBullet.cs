using Unity.Netcode;
using UnityEngine;

public class CircularBullet : NetworkBehaviour{

    [SerializeField] private GameObject _smallBulletPrefab;
    [SerializeField] private Transform[] _firePositions;

    private void SpawnBullets(){
        foreach (var firePosition in _firePositions){
            var go = Instantiate(
                _smallBulletPrefab,
                firePosition.position,
                firePosition.rotation);

            go.GetComponent<NetworkObject>().Spawn();
        }

        NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);
    }

    override public void OnNetworkSpawn(){
        if (!IsServer) return;
        var randomSpawn = Random.Range(1.5f, 3f);
        Invoke(nameof(SpawnBullets), randomSpawn);
    }
}