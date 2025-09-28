using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public interface IAttackBehaviour
{
    bool CanExecute(UnitBase u); // 사거리/쿨타임 충족?
    void Begin(UnitBase u);      // 애니 트리거/사전 준비
    void OnHit(UnitBase u);      // 실제 피해/투사체/AOE
    void End(UnitBase u);
}

public static class AttackFactory
{
    public static IAttackBehaviour Create(UnitSO so) => so.attackKind switch
    {
        Enums.AttackKind.MeleeSingle => new MeleeSingleBehaviour(so),
        Enums.AttackKind.RangeSingle => new RangedSingleBehaviour(so),
        Enums.AttackKind.MeleeAOE => new MeleeAOEBehaviour(so),
        Enums.AttackKind.RangeAOE => new RangedAOEBehaviour(so),
        Enums.AttackKind.Protocol => new ProtocolBehaviour(so),
        _ => null
    };
}

class MeleeSingleBehaviour : IAttackBehaviour
{
    readonly UnitSO so;
    public MeleeSingleBehaviour(UnitSO so) { this.so = so; }

    public bool CanExecute(UnitBase u)
    {
        u.ScanTarget();
        bool inRange = u.target && u.InAttackRange(u.target, so.unitRange);
        float cd = 1f / u.GetUnitStat().unitAttackSpeed;
        bool cooldown = Time.time - u.lastAttackTime >= cd;
        return inRange && cooldown;
    }

    public void Begin(UnitBase u)
    {
        Debug.Log("공격 실행");
        u.isAttacking = true;
        u.rb.velocity = Vector2.zero;
        u.lastAttackTime = Time.time;
        u.StartCoroutine(Hit(u));
        u.anim.SetTrigger("Attack");
        // 공격 애니메이션 재생

    }

    IEnumerator Hit(UnitBase u)
    {
        
        yield return new WaitForSeconds(Mathf.Max(0f, so.meleeHitDelay));
        if (u.effect != null)
        {
            u.StartCoroutine(EffectOn(u));
        }
        OnHit(u);
        End(u);
    }

    IEnumerator EffectOn(UnitBase u)
    {
        
        u.effect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        u.effect.SetActive(false);
    }

    public void OnHit(UnitBase u)
    {
        if (u.target && u.InAttackRange(u.target, so.unitRange) &&
            u.target.TryGetComponent<UnitBase>(out var d))
            d.TakeDamage(u.GetUnitStat().unitTotalDamage);

        if (u.target && u.InAttackRange(u.target, so.unitRange) &&
            u.target.TryGetComponent<GameTarget>(out var t))
            t.TakeDamage(u.GetUnitStat().unitTotalDamage);
    }

    public void End(UnitBase u)
    {
        u.isAttacking = false;
        u.ScanTarget();
    }
}

class MeleeAOEBehaviour : IAttackBehaviour
{
    readonly UnitSO so;
    static readonly Collider2D[] hits = new Collider2D[20];
    public MeleeAOEBehaviour(UnitSO so) { this.so = so; }

    public bool CanExecute(UnitBase u)
    {
        u.ScanTarget();
        Vector2 center = (Vector2)u.transform.position + u.dir * (so.unitRange * 0.5f);
        int count = Physics2D.OverlapCircleNonAlloc(center, so.unitRange * 0.5f, hits, u.enemyMask);
        float cd = 1f / u.GetUnitStat().unitAttackSpeed;
        return count > 0 && (Time.time - u.lastAttackTime >= cd);
    }

    public void Begin(UnitBase u)
    {
        Debug.Log("광역 공격 실행");
        u.isAttacking = true;
        u.rb.velocity = Vector2.zero;
        u.lastAttackTime = Time.time;
        u.StartCoroutine(Hit(u));
        u.anim.SetTrigger("Attack");
        // 공격 애니메이션 재생
    }

    IEnumerator Hit(UnitBase u)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, so.meleeHitDelay)); 
        if (u.effect != null)
        {
            u.StartCoroutine(EffectOn(u));
        }
        OnHit(u);
        End(u);
    }

    IEnumerator EffectOn(UnitBase u)
    {

        u.effect.SetActive(true);
        yield return new WaitForSeconds(so.attackEndDelay);
        u.effect.SetActive(false);
    }

    public void OnHit(UnitBase u)
    {
        Vector2 center = (Vector2)u.transform.position + u.dir * (so.unitRange * 0.5f);
        Debug.DrawLine(u.transform.position, center, Color.green, 0.5f);              // 공격 중심점까지 선
        DrawCircle(center, so.aoeRadius, Color.green, 0.5f);
        int count = Physics2D.OverlapCircleNonAlloc(center, so.aoeRadius, hits, u.enemyMask);
        for (int i = 0; i < count; i++)
        {
            if (hits[i] && hits[i].TryGetComponent<UnitBase>(out var d))
                d.TakeDamage(u.GetUnitStat().unitTotalDamage);    

            if (hits[i] && hits[i].TryGetComponent<GameTarget>(out var t))
                t.TakeDamage(u.GetUnitStat().unitTotalDamage);
        }
    }

    public void End(UnitBase u)
    {
        u.isAttacking = false;
        u.ScanTarget();
    }

    void DrawCircle(Vector2 center, float radius, Color color, float duration)
    {
        int segments = 32;
        float angle = 0f;
        Vector3 prevPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            angle = i * 2 * Mathf.PI / segments;
            Vector3 newPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Debug.DrawLine(prevPoint, newPoint, color, duration);
            prevPoint = newPoint;
        }
    }
}

class RangedSingleBehaviour : IAttackBehaviour
{
    readonly UnitSO so;
    public RangedSingleBehaviour(UnitSO so) { this.so = so; }

    public bool CanExecute(UnitBase u)
    {
        u.ScanTarget();
        bool inRange = u.target && u.InAttackRange(u.target, so.unitRange);
        float cd = 1f / u.GetUnitStat().unitAttackSpeed;
        bool cooldown = Time.time - u.lastAttackTime >= cd;
        return inRange && cooldown;
    }

    public void Begin(UnitBase u)
    {
        Debug.Log("원거리 공격 실행");
        u.isAttacking = true;
        u.rb.velocity = Vector2.zero;
        u.lastAttackTime = Time.time;
        u.StartCoroutine(Hit(u));
        u.anim.SetTrigger("Attack");
    }

    IEnumerator Hit(UnitBase u)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, so.rangeHitDelay));
        OnHit(u);
        End(u);
    }

    public void OnHit(UnitBase u)
    {
        if (!u.target || !so.projectile) return;
        var proj = PoolManager.Instance.Pop(so.projectile.name);
        proj.gameObject.GetComponent<Projectile>().Init(u);
        proj.transform.position = u.firePoint.position;
    }

    public void End(UnitBase u)
    {
        u.isAttacking = false;
        u.ScanTarget();
    }
}

class RangedAOEBehaviour : IAttackBehaviour
{
    readonly UnitSO so;
    static readonly Collider2D[] hits = new Collider2D[20];
    public RangedAOEBehaviour(UnitSO so) { this.so = so; }
    public bool CanExecute(UnitBase u)
    {
        u.ScanTarget();
        bool inRange = u.target && u.InAttackRange(u.target, so.unitRange);
        float cd = 1f / u.GetUnitStat().unitAttackSpeed;
        bool cooldown = Time.time - u.lastAttackTime >= cd;
        return inRange && cooldown;
    }
    public void Begin(UnitBase u)
    {
        Debug.Log("광역 원거리 공격 실행");
        u.isAttacking = true;
        u.rb.velocity = Vector2.zero;
        u.lastAttackTime = Time.time;
        u.StartCoroutine(Hit(u));
        u.anim.SetTrigger("Attack");
        // 공격 애니메이션 재생
    }

    IEnumerator Hit(UnitBase u)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, so.rangeHitDelay));
        OnHit(u);
        End(u);
    }
    public void OnHit(UnitBase u)
    {
        Vector2 center = (Vector2)u.transform.position + u.dir * so.aoeRange;
        Debug.DrawLine(u.transform.position, center, Color.red, 0.5f);
        DrawCircle(center, so.aoeRadius, Color.red, 0.5f);
        int count = Physics2D.OverlapCircleNonAlloc(center, so.aoeRadius, hits, u.enemyMask);
        for (int i = 0; i < count; i++)
        {
            if (hits[i] && hits[i].TryGetComponent<UnitBase>(out var d))
                d.TakeDamage(u.GetUnitStat().unitTotalDamage);

            if (hits[i] && hits[i].TryGetComponent<GameTarget>(out var t))
                t.TakeDamage(u.GetUnitStat().unitTotalDamage);
        }
            
        
    }
    public void End(UnitBase u)
    {
        u.isAttacking = false;
    }

    void DrawCircle(Vector2 center, float radius, Color color, float duration)
    {
        int segments = 32;
        float angle = 0f;
        Vector3 prevPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            angle = i * 2 * Mathf.PI / segments;
            Vector3 newPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Debug.DrawLine(prevPoint, newPoint, color, duration);
            prevPoint = newPoint;
        }
    }
}

class ProtocolBehaviour : IAttackBehaviour
{
    readonly UnitSO so;
    static readonly Collider2D[] hits = new Collider2D[20];
    public ProtocolBehaviour(UnitSO so) { this.so = so; }
    public bool CanExecute(UnitBase u)
    {
        u.ScanTarget();
        Vector2 center = (Vector2)u.transform.position + u.dir * (so.unitRange * 0.5f);
        int count = Physics2D.OverlapCircleNonAlloc(center, so.unitRange * 0.5f, hits, u.enemyMask);
        float cd = 1f / u.GetUnitStat().unitAttackSpeed;
        return count > 0 && (Time.time - u.lastAttackTime >= cd);
    }

    public void Begin(UnitBase u)
    {
        Debug.Log("광역 공격 실행");
        u.isAttacking = true;
        u.rb.velocity = Vector2.zero;
        u.lastAttackTime = Time.time;
        u.StartCoroutine(Hit(u));
        u.anim.SetTrigger("Attack");
        // 공격 애니메이션 재생
    }

    IEnumerator Hit(UnitBase u)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, so.Hit1Deley));
        OnHit(u);
        yield return new WaitForSeconds(Mathf.Max(0f, so.Hit2Deley));
        OnHit(u);
        yield return new WaitForSeconds(Mathf.Max(0f, so.Hit3Deley));
        OnHit2(u);
        yield return new WaitForSeconds(Mathf.Max(0f, so.Hit4Deley));
        End(u);
    }

    public void OnHit(UnitBase u)
    {
        Vector2 center = (Vector2)u.transform.position + u.dir * (so.unitRange * 0.5f);
        int count = Physics2D.OverlapCircleNonAlloc(center, so.unitRange * 0.5f, hits, u.enemyMask);
        for (int i = 0; i < count; i++)
        {
            if (hits[i] && hits[i].TryGetComponent<UnitBase>(out var d))
                d.TakeDamage(u.GetUnitStat().unitTotalDamage);

            if (hits[i] && hits[i].TryGetComponent<GameTarget>(out var t))
                t.TakeDamage(u.GetUnitStat().unitTotalDamage);
        }
        Debug.Log("Protocol Attack");
    }

    public void OnHit2(UnitBase u)
    {
        Vector2 center = (Vector2)u.transform.position + u.dir * so.aoeRange;
        int count = Physics2D.OverlapCircleNonAlloc(center, so.aoeRadius, hits, u.enemyMask);
        // 프로토콜 마지막 공격 소리
        for (int i = 0; i < count; i++)
        {
            if (hits[i] && hits[i].TryGetComponent<UnitBase>(out var d))
                d.TakeDamage(u.GetUnitStat().unitTotalDamage);

            if (hits[i] && hits[i].TryGetComponent<GameTarget>(out var t))
                t.TakeDamage(u.GetUnitStat().unitTotalDamage);
        }
        Debug.Log("Protocol Attack");

    }

    public void End(UnitBase u) { u.isAttacking = false;}
}
