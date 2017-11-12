using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEncounter : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem _portalOrbParticle;
    [SerializeField]
    Enemy _enemy;
    [SerializeField]
    private Transform[] _enemySpawns;

    private int _enemiesDefeated;

    // Use this for initialization
    void Start()
    {
        TogglePortal(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        GetComponent<Collider>().enabled = false;
        foreach (Transform enemySpawn in _enemySpawns)
        {
            Enemy enemy = Instantiate(_enemy, enemySpawn);
            enemy.OnDestroyedCallback += EnemyDestroyedCallback;
        }
    }

    private void EnemyDestroyedCallback(Enemy enemy)
    {
        _enemiesDefeated++;
        if (_enemiesDefeated >= _enemySpawns.Length)
        {
            Debug.Log("Encounter Complete");
            TogglePortal(true);
        }
    }

    private void TogglePortal(bool enable)
    {
        ParticleSystem.EmissionModule emission = _portalOrbParticle.emission;
        emission.enabled = enable;
    }
}
