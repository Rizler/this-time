using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerInRangeDetector : MonoBehaviour {

	[SerializeField]
	Enemy container;

	void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) {
			return;
		}
		container.playerTransform = other.transform;
	}

	void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag("Player")) {
			return;
		}
		container.playerTransform = null;
	}
}
