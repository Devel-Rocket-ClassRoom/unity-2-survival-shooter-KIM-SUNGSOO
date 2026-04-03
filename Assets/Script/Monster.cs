using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Monster : Livingentity
{
    public enum State { Idle, Trace, Attack, Die }
    public MonsterData data;

    public Collider monsterCollider;
    public Hitbox hitbox;
    public ParticleSystem hittedEffect;

    private NavMeshAgent agent;
    private Animator zombieAnimator;
    private AudioSource zombieAudioSource;

    
    public Transform target;
    public float traceDistance = 20f;
    public float attackDistance = 2f;
    public float attackDelay = 0.5f;
    public LayerMask targetLayer;
    public AudioClip hitClip;
    public AudioClip deathClip;

    private State currentState;
    private float lastAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        zombieAudioSource = GetComponent<AudioSource>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        currentState = State.Idle;
        monsterCollider.enabled = true;
        agent.enabled = true;
        agent.isStopped = false;

        // NavMesh 위치 초기화
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }

    private void Update()
    {
        if (IsDead) return;

        // target이 없으면 탐색
        if (target == null)
        {
            target = FindTarget(traceDistance);
        }


        // target이 있으면 거리 체크 후 상태 전환
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= attackDistance)
                currentState = State.Attack;
            else if (distance <= traceDistance)
                currentState = State.Trace;
            else
            {
                currentState = State.Idle;
                target = null;
            }
        }
        else
        {
            currentState = State.Idle;
        }

        // 상태별 처리
        switch (currentState)
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
                agent.isStopped = true;
                break;
        }
    }

    private void Trace()
    {
        if (!agent.isOnNavMesh) return;

        agent.isStopped = false;
        agent.SetDestination(target.position);
        zombieAnimator.SetBool("Targeting", true);

        // 몬스터가 플레이어 바라보게
        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    private void Attack()
    {
        agent.isStopped = true;
        zombieAnimator.SetBool("Targeting", false);

        if (Time.time - lastAttackTime >= attackDelay)
        {
            lastAttackTime = Time.time;

            // hitbox 활성화
            hitbox.gameObject.SetActive(true);
            Invoke(nameof(DisableHitbox), 0.2f);
        }

        // 공격 방향
        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    private void DisableHitbox()
    {
        hitbox.gameObject.SetActive(false);
    }

    private Transform FindTarget(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetLayer);
        if (colliders.Length == 0) return null;

        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var col in colliders)
        {
            var living = col.GetComponent<Livingentity>();
            if (living != null && !living.IsDead)
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = col.transform;
                }
            }
        }

        return closest;
    }
    public void Setup(MonsterData data)
    {
        this.data = data;

        startingHealth = data.maxHP;
        SetHealth(startingHealth); 

        damage = data.damage;
        agent.speed = data.speed;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        
        if (!IsDead)
        {
            if (zombieAudioSource != null && hitClip != null)
                zombieAudioSource.PlayOneShot(hitClip);

            base.OnDamage(damage, hitPoint, hitNormal);
        }

        if (hittedEffect != null)
        {
            hittedEffect.transform.position = hitPoint;
            hittedEffect.transform.forward = hitNormal;

            if (!hittedEffect.gameObject.activeInHierarchy)
                hittedEffect.gameObject.SetActive(true);
            hittedEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            hittedEffect.Play();
            
        }
    }

    public override void Die()
    {
        if (IsDead) return;

        base.Die();
        zombieAudioSource.PlayOneShot(deathClip);
        zombieAnimator.SetTrigger("Die");

        currentState = State.Die;
        agent.isStopped = true;
        agent.enabled = false;
        monsterCollider.enabled = false;
        hitbox.gameObject.SetActive(false);
    }

    public void StartSinking()
    {
        // 시체 가라앉는 애니메이션 이벤트가 호출되면 실행됨
        // 예: Rigidbody 없이 단순히 아래로 이동
        StartCoroutine(SinkAndDestroy());
    }

    private IEnumerator SinkAndDestroy()
    {
        float sinkSpeed = 2f;
        float duration = 2f; // 2초 동안 가라앉음
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.Translate(Vector3.down * sinkSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject,1f); 
    }
}