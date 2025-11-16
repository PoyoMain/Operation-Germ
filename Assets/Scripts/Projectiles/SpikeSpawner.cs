using UnityEngine;

public class SpikeSpawner : MonoBehaviour
{
    [SerializeField] private ProjectileSO spikeProjectileData;
    [SerializeField] private GameObject rumblePrefab;
    [SerializeField] private Transform[] spikeSpawnPoints;
    [SerializeField] private Transform[] rumbleSpawnPoints;
    [SerializeField] private float launchSpeed;

    public void StartSpikeSpawn()
    {
        if (spikeSpawnPoints.Length == 0) return;

        int index = Random.Range(0, spikeSpawnPoints.Length);
        Transform spawnPoint = spikeSpawnPoints[index];

        IProjectile projectile = Instantiate(spikeProjectileData.EnemyProjectile, spawnPoint.position, Quaternion.identity, spawnPoint).GetComponent<IProjectile>();
        projectile.Initialize(transform.up, launchSpeed);

        Transform rumbleSpawn = rumbleSpawnPoints[index];
        Instantiate(rumblePrefab, rumbleSpawn.position, Quaternion.identity);
    }
}
