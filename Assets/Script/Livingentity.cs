using UnityEngine;
using UnityEngine.Events;
public class Livingentity : MonoBehaviour, IDamageable
{
    
    public float startingHealth = 100f;

    protected float damage;
    public float Health { get; private set; }

    public bool IsDead { get; private set; }

    public UnityEvent OnDead;

    protected virtual void OnEnable()
    {
        IsDead = false;
        Health = startingHealth;
    }
    public void SetHealth(float value)
    {
        Health = value;
    }
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if(IsDead)
        {
            return;
        }
        Health -= damage;
        if(Health <= 0)
        {
            Health = 0;
            Die();
        }
    }

    public virtual void Die()
    {

        if (IsDead) return;

        IsDead = true;
        OnDead?.Invoke();
    }

    
}
