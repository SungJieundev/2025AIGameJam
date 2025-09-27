using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTarget : MonoBehaviour
{
    public int maxHp;
    public int curHp;
    public LayerMask layer;


    private void Start()
    {
        if (layer == LayerMask.GetMask("Enemy"))
            maxHp = GameManager.Instance.buildingMaxHp;
        else if (layer == LayerMask.GetMask("Tower"))
            maxHp = GameManager.Instance.playerMaxHp;

        curHp = maxHp;
    }
    public void TakeDamage(float damage)
    {
        curHp -= (int)damage;
        Debug.Log(curHp);
        if (curHp <= 0)
        {
            if (layer == LayerMask.GetMask("Enemy"))
                GameManager.Instance.GameClear();
            else if (layer == LayerMask.GetMask("Tower"))
                GameManager.Instance.GameOver();
        }
    }
}
