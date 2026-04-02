using System.Collections;
using UnityEngine;


public class Gun : MonoBehaviour
{
    public Transform firePoint;
    private LineRenderer bulletLineEffect;
    private AudioSource gunAudioPlayer;

    public LayerMask targetMask;
    float fireDistance = 100f;
    public float fireRate = 0.2f;
    private float lastFireTime;
    public ParticleSystem gunParticle;

    private void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineEffect = firePoint.GetComponent<LineRenderer>();

        bulletLineEffect.positionCount = 2;
        bulletLineEffect.enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
    {
        if (Time.time - lastFireTime > fireRate)
        {
            lastFireTime = Time.time;
            Shot();
        }
    }
        
    }

    public void Shot()
    {

        Ray ray = new Ray(firePoint.position, firePoint.forward);
        RaycastHit hit;

        Vector3 targetPoint;


        if (Physics.Raycast(ray, out hit, fireDistance, targetMask))
        {
            

            var living = hit.collider.GetComponentInParent<Livingentity>();
            if (living != null)
            {
                living.OnDamage(10f, hit.point, hit.normal);
                
            }
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 100f;
        }

        
        Vector3 direction = (targetPoint - firePoint.position).normalized;
        Vector3 endPoint = firePoint.position + direction * 100f;

        if (hit.collider != null)
        {
            endPoint = targetPoint;
        }

        StartCoroutine(CoshotEffect(endPoint));
    }
    private IEnumerator CoshotEffect(Vector3 hitPosition)
    {

        gunParticle.Play();

        bulletLineEffect.SetPosition(0, firePoint.position);
        bulletLineEffect.SetPosition(1, hitPosition);
        bulletLineEffect.enabled = true;
        yield return new WaitForSeconds(0.03f);

        bulletLineEffect.enabled = false;
    }
}
