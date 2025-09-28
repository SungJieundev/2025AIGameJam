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
                return;
            }       
        }
        GameManager.Instance.UseMoney(so.unitPrice);
        SpawnTower(index);
        lastSpawnTime[index] = Time.time;

        FindObjectOfType<InGameUI>().StartTowerCooldown(index, so.cooldown);
    }

    public void ProtocolMecha(int index)
    {
        UnitBase unit = towerList[index].GetComponent<UnitBase>();
        UnitSO so = unit.unitSO;
        if (GameManager.Instance.curProtocolPower < so.unitPrice) return;

        if (lastSpawnTime.TryGetValue(index, out float lastTime))
        {
            float elapsed = Time.time - lastTime;
            if (elapsed < so.cooldown)
            {
                return;
            }
        }
        GameManager.Instance.UseProtocolPower(so.unitPrice);
        SpawnTower(index);
        lastSpawnTime[index] = Time.time;

        FindObjectOfType<InGameUI>().StartTowerCooldown(index, so.cooldown);
    }
}
