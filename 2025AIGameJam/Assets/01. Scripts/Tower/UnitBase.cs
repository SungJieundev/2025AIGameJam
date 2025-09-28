using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitBase : PoolableMono
{
    public UnitSO unitSO;
    private UnitStat unitStat;
    public Transform firePoint;
    public GameObject effect;

    [HideInInspector] public Rigidbody2D rb;
    public Animator anim;

    public Vector2 dir;
    public float lastAttackTime = -999f;
    [HideInInspector] public Transform target;
    public float scanInterval = 0.01f;
    public float scanTimer;
    public float dathKnockDist = 0.7f;
    public float deathKnockTime = 0.25f;
    [HideInInspector]public bool pendingDeath;

    int _missCount = 0;
    const int MissToClear = 2;

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

    private void Update()
    {
        fsm.Tick();

        // 스캔 주기 처리
        scanTimer += Time.deltaTime;
        if (scanTimer >= scanInterval)  // scanInterval = 0.05~0.1f 권장
        {
            scanTimer = 0f;

            Transform before = target;
            ScanTarget();

            if (!target)
            {
                _missCount++;
                if (_missCount < MissToClear)
                    target = before; // 한두 번 놓쳐도 이전 타깃 유지
            }
            else _missCount = 0;
        }
    }
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
    static readonly Collider2D[] scanBuf = new Collider2D[20];

    public void ScanTarget()
    {
        target = null;

        float range = GetUnitStat().unitRange;
        // 전방 중앙점을 기준으로 탐색 (근접/원거리 모두 안정적)
        Vector2 center = (Vector2)transform.position + dir * (range * 0.5f);

        int count = Physics2D.OverlapCircleNonAlloc(center, range * 0.6f, scanBuf, enemyMask);

        float best = float.MaxValue;
        Transform bestT = null;

        for (int i = 0; i < count; i++)
        {
            var col = scanBuf[i];
            if (!col) continue;

            // 자기 자신 필터
            if (col.transform == transform) continue;

            // 정면성(시야각) 필터: 완전 원형이 부담되면 아래 주석 해제해서 전방만 남기기
            // Vector2 to = (Vector2)col.transform.position - (Vector2)transform.position;
            // if (Vector2.Dot(to.normalized, dir) < 0f) continue; // 후방 제외

            // 유효 타깃만 인정
            if (!col.TryGetComponent<UnitBase>(out var ub) && !col.TryGetComponent<GameTarget>(out var gt))
                continue;

            float dx = Mathf.Abs(col.transform.position.x - transform.position.x);
            if (dx < best)
            {
                best = dx;
                bestT = col.transform;
            }
        }

        target = bestT;
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

        if (damage >= unitStat.unitCurHp)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
                GameManager.Instance.GainProtocolPower(unitStat.unitCurHp);
            unitStat.unitCurHp -= damage;
            if (unitStat.unitCurHp <= 0)
            {
                pendingDeath = true;
                GoDead();
            }
        }
        else
        {
            if(gameObject.layer == LayerMask.NameToLayer("Enemy"))
                GameManager.Instance.GainProtocolPower(damage);
            unitStat.unitCurHp -= damage;
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
