using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitBase : PoolableMono
{
    public UnitSO unitSO;
    private UnitStat unitStat;
    public Transform firePoint;

    public Rigidbody2D rb;

    public Vector2 dir;
    public float lastAttackTime = -999f;
    [HideInInspector] public Transform target;
    [HideInInspector] public float scanInterval = 0.1f;
    public float scanTimer;
    public float dathKnockDist = 0.7f;
    public float deathKnockTime = 0.25f;
    [HideInInspector]public bool pendingDeath;

    public LayerMask enemyMask;
    public LayerMask allyMask;

    StateMachine fsm;

    IState sSpawn, sMarch, sAttack, sDead;

    public bool isAttacking;
    public IAttackBehaviour attack;

    #region Unity Events
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        bool isEnemy = (enemyMask.value & (1 << enemyLayer)) != 0;
        dir = isEnemy ? Vector2.left : Vector2.right;

        attack = AttackFactory.Create(unitSO);

        fsm = new StateMachine();
        sSpawn = new State_Spawn(this, fsm);
        sMarch = new State_March(this, fsm);
        sAttack = new State_Attack(this, fsm);
        sDead = new State_Dead(this, fsm);

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

    public void GoMarch() => fsm.SetState(sMarch);
    public void GoAttack() => fsm.SetState(sAttack);
    public void GoDead() => fsm.SetState(sDead);

    #region Unit Base Function
    public void ScanTarget()
    {
        target = null;
        Vector2 center = (Vector2)transform.position + dir * (unitStat.unitRange * 0.5f);
        Collider2D[] results = new Collider2D[8];

        int count = Physics2D.OverlapCircleNonAlloc(center, unitStat.unitRange, results, enemyMask);
        
        float best = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            var c = results[i];
            if (!c) continue;

            float dx = Mathf.Abs(c.transform.position.x - transform.position.x);

            if (dx < best)
            {
                best = dx;
                target = c.transform;
            }
        }
    }

    public bool InAttackRange(Transform t, float rangeOverride = -1f)
    {
        if (!t) return false;

        float range = (rangeOverride > 0f) ? rangeOverride : unitStat.unitRange;

        float dx = Mathf.Abs(t.position.x - transform.position.x);

        return dx <= range + 0.01f;
    }

    public void AttackEnterFromFSM()
    {
        if (attack == null || !attack.CanExecute(this)) { GoMarch(); return; }
        attack.Begin(this);
    }

    // 스캔 타이머 접근용
    public ref float ScanTimerRef() => ref scanTimer;

    public bool IsDead => unitStat.unitCurHp <= 0f;
    public void TakeDamage(float damage)
    {
        if (IsDead) return; ;
        unitStat.unitCurHp -= damage;
        Debug.Log(gameObject.name + " 피격됨 : " + damage + "피해 입음 / 남은 체력 : " + GetUnitStat().unitCurHp);
        if (unitStat.unitCurHp <= 0)
        {
            pendingDeath = true;
            GoDead();
        }
    }

    public void Die()
    {
        Debug.Log("타워 사망");
    }
    #endregion

    public override void Reset()
    {
        
    }
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(dir, unitSO ? unitSO.unitRange : 1f);
        }
        else
        {
            Vector2 c = (Vector2)transform.position + dir * (unitStat.unitRange * .5f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(c, unitStat.unitRange);
        }
    }
}
