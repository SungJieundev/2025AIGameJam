using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitBase : PoolableMono
{
    public UnitSO unitSO;
    private UnitStat unitStat;
    public Transform firePoint;

    [HideInInspector] public Rigidbody2D rb;
    public Animator anim;

    public Vector2 dir;
    public float lastAttackTime = -999f;
    [HideInInspector] public Transform target;
    [HideInInspector] public float scanInterval = 0.05f;
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
        anim = GetComponent<Animator>();

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

        int level = GameManager.Instance.damageUpgradeLevel;
        if (level > 0 && gameObject.layer == LayerMask.NameToLayer("Tower"))
        {
            unitStat.TowerDamageUpgrade(level);
            unitStat.ApplyDamage();
        }

        GameManager.Instance.OnTowerDamageUpgrade += HandleTowerDamageUpgrade;

        rb.velocity = Vector2.zero;
        lastAttackTime = -999f;
        scanTimer = 0f;

        fsm.SetState(sSpawn);
    }

    protected virtual void OnDisable()
    {
        GameManager.Instance.OnTowerDamageUpgrade -= HandleTowerDamageUpgrade;
    }

    private void HandleTowerDamageUpgrade(int level)
    {
        if (gameObject.layer == LayerMask.NameToLayer("Tower"))
        {
            unitStat.TowerDamageUpgrade(level);
            unitStat.ApplyDamage();
        }      
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
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, GetUnitStat().unitRange, enemyMask);

        if (hit.collider && hit.collider.TryGetComponent<UnitBase>(out var enemy))
            target = enemy.transform;
        else if (hit.collider && hit.collider.TryGetComponent<GameTarget>(out var target))
            this.target = target.transform;
        else
            target = null;
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
        if(attack == null || !attack.CanExecute(this))
        {
            GoMarch();
            return;
        }
    }

    // 스캔 타이머 접근용
    public ref float ScanTimerRef() => ref scanTimer;

    public bool IsDead => unitStat.unitCurHp <= 0f;
    public void TakeDamage(float damage)
    {
        if (IsDead) return; ;
        unitStat.unitCurHp -= damage;
        if (unitStat.unitCurHp <= 0)
        {
            pendingDeath = true;
            GoDead();
        }
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
            Gizmos.DrawWireSphere(dir, unitSO ? unitStat.unitRange * .5f : 1f);
        }
        else
        {
            Vector2 c = (Vector2)transform.position + dir * (unitStat.unitRange * .5f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(c, unitStat.unitRange * .5f);
        }
    }
}
