using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour {

    [SerializeField]
    private float _maxHp = 100;
    private float _hp;
    private NavMeshAgent agent;

    [SerializeField]
    private Image healthBarImg;

    [SerializeField]
    private Transform healthCanvasContainer;

    public delegate void DestroyedCallback(Enemy enemy);
    public event DestroyedCallback OnDestroyedCallback;

    public Transform playerTransform {get;set;}

    void Start()
    {
        _hp = _maxHp;
        if (!healthBarImg) healthBarImg = transform.Find("Canvas").transform.Find("HealthBarImage").GetComponent<Image>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() 
    {
        if (playerTransform) {
            agent.destination = playerTransform.position;
        }
    }

    public void Hit()
    {
        _hp -= 25;
        healthBarImg.fillAmount = _hp / _maxHp;
        if (_hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Water")
        {
           // Destroy(gameObject);
        }
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        if (OnDestroyedCallback != null) {OnDestroyedCallback(this);}
    }
}
