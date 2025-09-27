using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PoolableMono
{
    public float damage;
    public float range;
    public UnitBase owner;
    public string targetTag;

    private Vector2 prodir;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void Reset()
    {
        

    }

    public void Init(UnitBase u)
    {
        owner = u;
        damage = owner.GetUnitStat().unitTotalDamage;
        range = owner.GetUnitStat().unitRange;
        prodir = owner.dir;;
        rb.velocity = new Vector2(10 * prodir.x, 0);
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, owner.transform.position) >= range)
        {
            rb.velocity = Vector2.zero;
            PoolManager.Instance.Push(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            if (collision.TryGetComponent<UnitBase>(out var d))
            {
                d.TakeDamage(damage);
            }
            else if (collision.TryGetComponent<GameTarget>(out var t))
            {
                t.TakeDamage(damage);
            }
            rb.velocity = Vector2.zero;
            PoolManager.Instance.Push(this);
        }
    }
}
