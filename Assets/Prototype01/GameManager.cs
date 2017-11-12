using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private PortalEncounter _portalEncounter1;
    [SerializeField]
    private PortalEncounter _portalEncounter2;
    [SerializeField]
    private GameObject _gateBars;
    [SerializeField]
    private Enemy _boss;


    private int _completedEvents;

    // Use this for initialization
    void Start()
    {
        _portalEncounter1.onEncounterCompleteCallback += EncounterCompleteCallback;
        _portalEncounter2.onEncounterCompleteCallback += EncounterCompleteCallback;
        _boss.OnDestroyedCallback += OnBossDestroyedCallback;
    }

    private void OnBossDestroyedCallback(Enemy enemy)
    {
        StartCoroutine(Victory());
    }

    private IEnumerator Victory()
    {
        Text victoryText = transform.Find("Canvas").transform.Find("Victory Text").GetComponent<Text>();
        victoryText.enabled = true;
        yield return new WaitForSecondsRealtime(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void EncounterCompleteCallback(PortalEncounter encounter)
    {
        _completedEvents++;
        if (_completedEvents >= 2)
        {
            _gateBars.transform.position += new Vector3(0, 12, 0);
        }
    }
}
