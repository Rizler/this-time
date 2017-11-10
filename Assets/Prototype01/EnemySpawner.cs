using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

	private class EnemyRef
	{
		public Vector3 loc;
		public Quaternion rot;
	}

	Dictionary<Enemy, EnemyRef> spawnLocations = new Dictionary<Enemy, EnemyRef>();

	[SerializeField]
	private Enemy enemyPrefab;

	void Start () {
		var enemies = FindObjectsOfType(typeof(Enemy));
		foreach(Enemy enemy in enemies) {
			spawnLocations.Add(enemy, new EnemyRef{loc = enemy.transform.position, rot = enemy.transform.rotation});
			enemy.OnDestroyedCallback += OnEnemyDestroyed;
		}
	}
	void OnEnemyDestroyed(Enemy enemy) {
		var enemyRef = spawnLocations[enemy];
		spawnLocations.Remove(enemy);
		var newEnemy = Instantiate(enemyPrefab, enemyRef.loc, enemyRef.rot);
		newEnemy.OnDestroyedCallback += OnEnemyDestroyed;
		spawnLocations.Add(newEnemy, enemyRef);

	}
	
}
