using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour {

	[SerializeField]
	Transform playerTransform = null;
	Vector3 playerStartPos;
	Quaternion playerStartRot;
	Collider restartCollider = null;

	// Use this for initialization
	void Start () {
		restartCollider = GetComponent<Collider>();
		playerStartPos = playerTransform.position;
		playerStartRot = playerTransform.rotation;
		
	}

	void Reset()
	{
		if (!playerTransform) playerTransform = FindObjectOfType<PlayerController>().transform;
	}

	void OnTriggerEnter(Collider other)
	{
		playerTransform.position = playerStartPos;
		playerTransform.rotation = playerStartRot;
	}
}

