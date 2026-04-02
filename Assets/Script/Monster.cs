using UnityEngine;
using UnityEngine.AI;

public class Monster : Livingentity
{

    public enum State
    {
        Idle,
        Trace,
        Attack,
        Die
    }

    public Collider monsterCollider;
    public Transform target;
    public AudioClip deathClip;
    public AudioClip hitClip;
    private AudioSource zombieAudioSource;
    private Animator zombieAnimator;
    private NavMeshAgent agent;
    public ParticleSystem hittedEffect;
    private State currentstate;
    public float traceDistance = 200f;
    public float attackDistance = 1f;
    private float lastAttackTime;
    public float attackDelay = 0.5f;
    public Hitbox hitbox;
    public LayerMask targetLayer;
    

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        zombieAudioSource = GetComponent<AudioSource>();



    }

    void Update()
    {
        if (IsDead || target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackDistance)
        {
            currentstate = State.Attack;
        }
        else if (distance <= traceDistance)
        {
            currentstate = State.Trace;
        }
        else
        {
            currentstate = State.Idle;
        }
        switch (currentstate)
        {
            case State.Idle:
                agent.isStopped = true;
                zombieAnimator.SetBool("Targeting", false);
                break;

            case State.Trace:
                Trace();
                break;

            case State.Attack:
                Attack();
                break;

            case State.Die:
                break;
        }
    }

    public void Trace()
    {
        if (!agent.isOnNavMesh) return;

        agent.isStopped = false;
        agent.SetDestination(target.position);

        zombieAnimator.SetBool("Targeting", true);
    }

    public void Attack()
    {
        agent.isStopped = true;
        zombieAnimator.SetBool("Targeting", false);

        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

        
        if (Time.time - lastAttackTime >= attackDelay)
        {
            lastAttackTime = Time.time;

            
            hitbox.gameObject.SetActive(true);
            Invoke("DisableHitbox", 0.2f);
        }
    }

    void DisableHitbox()
    {
        hitbox.gameObject.SetActive(false);
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        agent.enabled = true;
        agent.isStopped = false;
        

        monsterCollider.enabled = true;
        currentstate = State.Idle;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead) return; 

        zombieAudioSource.PlayOneShot(hitClip);

        base.OnDamage(damage, hitPoint, hitNormal);

        if (hittedEffect != null)
        {
            hittedEffect.transform.position = hitPoint;
            hittedEffect.transform.forward = hitNormal;
            hittedEffect.Play();
        }
    }
}

