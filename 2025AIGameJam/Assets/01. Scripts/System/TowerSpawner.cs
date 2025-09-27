using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    public List<GameObject> spawnPoint;
    public List<GameObject> towerList;
    private Dictionary<int, float> lastSpawnTime = new Dictionary<int, float>();
    private int nowSpawnPoint;
    private int nowSortingLayer = 6;


    public void SpawnTower(int index)
    {
        GameObject tower = PoolManager.Instance.Pop(towerList[index].name).gameObject;
        tower.GetComponent<SpriteRenderer>().sortingOrder = nowSortingLayer;
        tower.transform.position = spawnPoint[nowSpawnPoint].transform.position;
        nowSpawnPoint += 1;
        nowSortingLayer -= 1;
        if (nowSpawnPoint >= spawnPoint.Count) nowSpawnPoint = 0;
        if (nowSortingLayer <= 1) nowSortingLayer = 6;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) BuyTower(0);
        if (Input.GetKeyDown(KeyCode.F2)) BuyTower(1);
        if (Input.GetKeyDown(KeyCode.F3)) BuyTower(2);
        if (Input.GetKeyDown(KeyCode.F4)) BuyTower(3);
        if (Input.GetKeyDown(KeyCode.F5)) BuyTower(4);
        if (Input.GetKeyDown(KeyCode.F6)) BuyTower(5);
        if (Input.GetKeyDown(KeyCode.F7)) BuyTower(6);
        if (Input.GetKeyDown(KeyCode.F8)) BuyTower(7);
        if (Input.GetKeyDown(KeyCode.F9)) SpawnTower(8);
    }

    public void BuyTower(int index)
    {
        UnitBase unit = towerList[index].GetComponent<UnitBase>();
        UnitSO so = unit.unitSO;
        if (GameManager.Instance.playerCurMoney < so.unitPrice) return;

        if (lastSpawnTime.TryGetValue(index, out float lastTime))
        {
            float elapsed = Time.time - lastTime;
            if (elapsed < so.cooldown)
            {
                Debug.Log("ÄðÅ¸ÀÓ");
                return;
            }       
        }
        GameManager.Instance.UseMoney(towerList[index].GetComponent<UnitBase>().unitSO.unitPrice);
        SpawnTower(index);

        lastSpawnTime[index] = Time.time;
    }
}
