using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{

    [SerializeField]
    private float _maxHp = 100;
    private float _hp;
    private NavMeshAgent agent;

    [SerializeField]
    private Image healthBarImg;

    [SerializeField]
    private Transform healthCanvasContainer;

    private int hitCounter = 0;

    public delegate void DestroyedCallback(Enemy enemy);
    public event DestroyedCallback OnDestroyedCallback;

    public Transform playerTransform { get; set; }

    void Start()
    {
        _hp = _maxHp;
        if (!healthBarImg) healthBarImg = transform.Find("Canvas").transform.Find("HealthBarImage").GetComponent<Image>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (playerTransform && agent.enabled)
        {
            agent.destination = playerTransform.position;
        }
    }

    private IEnumerator KnockDown()
    {
        return null;
        /*agent.enabled = false;
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        Quaternion standingRotation = rigidBody.rotation;
        Quaternion newRotation = standingRotation * Quaternion.Euler(-90, 0, 0);
        rigidBody.MoveRotation(newRotation);
        transform.rotation = newRotation;
        Camera.main.GetComponent<FollowingCamera>().Shake(0.15f, 0.25f);
        yield return new WaitForSeconds(3);
        rigidBody.MoveRotation(standingRotation);
        agent.enabled = true;*/
    }

    public void Hit()
    {
        //_hp -= 25;
        healthBarImg.fillAmount = _hp / _maxHp;
        hitCounter++;
        if (hitCounter == 3)
        {
            StartCoroutine(KnockDown());
            hitCounter = 0;
        }
        if (_hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Water")
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        if (OnDestroyedCallback != null) { OnDestroyedCallback(this); }
    }
}
