using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

	private float _maxHp = 100;
    private float _hp = 100;

    [SerializeField]
    private Image healthBarImg;

    void Start()
    {
        if (!healthBarImg) healthBarImg = transform.Find("Canvas").transform.Find("HealthBarImage").GetComponent<Image>();
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
}
