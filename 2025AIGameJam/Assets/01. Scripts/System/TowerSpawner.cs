using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    public List<GameObject> spawnPoint;
    public List<GameObject> towerList;
    private int nowSpawnPoint;

    public void SpawnTower(int index)
    {
        GameObject tower = PoolManager.Instance.Pop(towerList[index].name).gameObject;
        tower.transform.position = spawnPoint[nowSpawnPoint].transform.position;
        nowSpawnPoint += 1;
        if (nowSpawnPoint >= spawnPoint.Count) nowSpawnPoint = 0;
    }
}
