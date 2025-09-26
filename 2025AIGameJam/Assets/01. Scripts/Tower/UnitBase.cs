using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public UnitSO unitSO;
    private UnitStat unitStat;
    public Transform firePoint;

    private Rigidbody2D rb;

    public Vector2 dir;
    public float lastAttackTime = -999f;
    public float scanInterval = 0.1f;
    public float scanTimer;

    public LayerMask enemyMask;
    public LayerMask allyMask;

    StateMachine fsm;

    IState sSpawn, sMarch, sAttack, sKnock, sDead;

    public IAttackBehaviour attack;

    #region Unity Events
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        bool isEnemy = (enemyMask.value & (1 << enemyLayer)) != 0;
        dir = isEnemy ? Vector2.right : Vector2.left;

        fsm = new StateMachine();


    }

    protected virtual void OnEnable()
    {
        if (unitStat == null)
            unitStat = new UnitStat(unitSO);

        unitStat.UnitReset(unitSO);
        rb.velocity = Vector2.zero;
        lastAttackTime = -999f;
        scanTimer = 0f;

        fsm.SetState(sSpawn);
    }

    private void Update() => fsm.Tick();
    private void FixedUpdate() => fsm.FixedTick();
    #endregion

    public UnitStat GetUnitStat()
    {
        return unitStat;
    }

    

    #region Unit Base Function
    public void TakeDamage(int damage)
    {
        unitStat.unitHp -= damage;
        if (unitStat.unitHp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Å¸¿ö »ç¸Á");
    }
    #endregion
}
