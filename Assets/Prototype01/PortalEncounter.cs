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

    public delegate void OnEncounterComplete(PortalEncounter encounter);
    public event OnEncounterComplete onEncounterCompleteCallback;

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
            //TODO: Figure out why this doesn't really spawn the enemy at enemy enemySpawn.position
            //Enemy enemy = Instantiate(_enemy, enemySpawn.position, Quaternion.identity);
            Enemy enemy = Instantiate(_enemy, enemySpawn);
            enemy.OnDestroyedCallback += EnemyDestroyedCallback;
        }
    }

    private void EnemyDestroyedCallback(Enemy enemy)
    {
        _enemiesDefeated++;
        if (_enemiesDefeated >= _enemySpawns.Length)
        {
            TogglePortal(true);
            onEncounterCompleteCallback.Invoke(this);
        }
    }

    private void TogglePortal(bool enable)
    {
        ParticleSystem.EmissionModule emission = _portalOrbParticle.emission;
        emission.enabled = enable;
    }
}
