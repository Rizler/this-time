using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

	private float _maxHp = 100;
    private float _hp = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _hp -= 25;
            Image healthBarImg = transform.Find("Canvas").transform.Find("HealthBarImage").GetComponent<Image>();
            healthBarImg.fillAmount = _hp / _maxHp;
            if (_hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
