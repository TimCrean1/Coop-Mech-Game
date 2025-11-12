using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienWaveManager : MonoBehaviour
{
    [SerializeField] private List<float> waveIntervals = new List<float>();
    [SerializeField] private List<float> spawnIntervals = new List<float>();
    [SerializeField] private List<Transform> waveSpawns = new List<Transform>();
    [SerializeField] private Transform spawnPos;

    [SerializeField] private List<GameObject> wave1Enemies = new List<GameObject>();
    [SerializeField] private List<GameObject> wave2Enemies = new List<GameObject>();
    [SerializeField] private List<GameObject> wave3Enemies = new List<GameObject>();
    [SerializeField] private AlienMothership mothership;

    private bool waveActive = true;

    void Start()
    {
        mothership.gameObject.SetActive(false);
        GameState.Instance.OnFinalWaveStart.AddListener(StartWave);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartWave();
        }
    }

    private void StartWave()
    {
        mothership.gameObject.SetActive(true); //TODO: Lerp routine for polish
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        yield return new WaitForSeconds(5f);
        while(waveActive)
        {
            for(int i = 0; i<wave1Enemies.Count; i++)
            {
                Instantiate(wave1Enemies[i], spawnPos.transform.position, Quaternion.identity);

                if (wave1Enemies[i].GetComponent<SentryAlien>() == true)
                {
                    SentryAlien alien = wave1Enemies[i].GetComponent<SentryAlien>();
                    alien.waveTarget = waveSpawns[i];
                }

                yield return new WaitForSeconds(spawnIntervals[0]);
            }
            yield return new WaitForSeconds(waveIntervals[0]);

            for (int i = 0; i < wave2Enemies.Count; i++)
            {
                Instantiate(wave2Enemies[i], spawnPos.transform.position, Quaternion.identity);

                if (wave1Enemies[i].GetComponent<SentryAlien>() == true)
                {
                    SentryAlien alien = wave1Enemies[i].GetComponent<SentryAlien>();
                    alien.waveTarget = waveSpawns[i];
                }

                yield return new WaitForSeconds(spawnIntervals[1]);
            }
            yield return new WaitForSeconds(waveIntervals[1]);

            for (int i = 0; i < wave3Enemies.Count; i++)
            {
                Instantiate(wave3Enemies[i], spawnPos.transform.position, Quaternion.identity);

                if (wave1Enemies[i].GetComponent<SentryAlien>() == true)
                {
                    SentryAlien alien = wave1Enemies[i].GetComponent<SentryAlien>();
                    alien.waveTarget = waveSpawns[i];
                }

                yield return new WaitForSeconds(spawnIntervals[2]);
            }
            yield return new WaitForSeconds(waveIntervals[2]);

            waveActive = false;
            mothership.OnWaveEnd();

            yield return null;
        }
        yield return null;
    }
    
}
