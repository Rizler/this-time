using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

	private float _maxHp = 100;
    private float _hp = 100;

    [SerializeField]
    private Image healthBarImg;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (!healthBarImg) healthBarImg = transform.Find("Canvas").transform.Find("HealthBarImage").GetComponent<Image>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _hp -= 25;
            healthBarImg.fillAmount = _hp / _maxHp;
            if (_hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
