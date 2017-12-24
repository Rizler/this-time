using Prototype02;
using UnityEngine;

public class Lava : MonoBehaviour
{

    [SerializeField]
    private Transform playerTransform = null;

    private Vector3 playerStartPos;
    private Quaternion playerStartRot;
    private Collider restartCollider = null;

    // Use this for initialization
    private void Start()
    {
        restartCollider = GetComponent<Collider>();
        playerStartPos = playerTransform.position;
        playerStartRot = playerTransform.rotation;

    }

    private void Reset()
    {
        if (!playerTransform) playerTransform = FindObjectOfType<PlayerController>().transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyAI>())
        {
            Destroy(other);
            //todo:add score ?
        }
        else
        {

            playerTransform.position = playerStartPos;
            playerTransform.rotation = playerStartRot;
        }
    }
}