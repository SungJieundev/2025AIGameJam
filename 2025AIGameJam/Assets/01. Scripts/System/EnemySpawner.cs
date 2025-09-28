using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class EnemyWave
{
    public float spawnTimer;
    public GameObject enemy;
    public int count;
    public float interval;
}

public class EnemySpawner : MonoBehaviour
{
    public int nowWave = 0;
    public List<GameObject> spawnPoint;
    public List<EnemyWave> waves;
    public float waveTimers;
    private int nowSpawnPoint;
    private int nowSortingLayer = 6;


    private void Update()
    {
        waveTimers += Time.deltaTime;
        if (waveTimers >= waves[nowWave].spawnTimer)
        {
            StartCoroutine(SpawnWave(waves[nowWave]));
            nowWave += 1;
            if (nowWave >= waves.Count)
            {
                nowWave = 0;
                waveTimers = 0;
            }

        }
    }

    IEnumerator SpawnWave(EnemyWave wave)
    {
        Debug.Log("웨이브 시작");
        for (int i = 0; i < wave.count; i++ )
        {
            GameObject enemy = PoolManager.Instance.Pop(wave.enemy.name).gameObject;
            if (enemy.GetComponent<SpriteRenderer>())
            {
                enemy.GetComponent<SpriteRenderer>().sortingOrder = nowSortingLayer;
            }
            else
            {
                enemy.GetComponentInChildren<SpriteRenderer>().sortingOrder = nowSortingLayer;
            }    
            enemy.transform.position = spawnPoint[nowSpawnPoint].transform.position;
            nowSpawnPoint += 1;
            nowSortingLayer -= 1;
            if (nowSpawnPoint >= spawnPoint.Count) nowSpawnPoint = 0;
            if (nowSortingLayer <= 1) nowSortingLayer = 6;
            yield return new WaitForSeconds(wave.interval);
        }
    }    
}
