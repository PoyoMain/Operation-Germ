using System.Collections;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class HeartBurn : MonoBehaviour
{
    [Header("General Boss Settings")]
    [SerializeField] private BossPart bossPart;
    [SerializeField] private float waitTime;
    [SerializeField] private float timeBetweenMoves;


    [Header("FireBall Settings")]
    [SerializeField] private float fireBallLaunchForce;
    [SerializeField] private Transform topFaceTransform;
    [SerializeField] private ProjectileSO fireBallData;
    [SerializeField] private float fireBallWaitTime;
    [SerializeField] private AudioSource smallSpit;
    [SerializeField] private AudioSource largeSpit;

    [Header("FireLog Settings")]
    [SerializeField] private Transform midFaceTransform;
    [SerializeField] private float logLaunchForce;
    [SerializeField] private ProjectileSO fireLogData;
    [SerializeField] private float fireLogWaitTime;

    [Header("FireMissile Settings")]
    [SerializeField] private Transform botFaceTransform;
    [SerializeField] private ProjectileSO fireMissileData;
    [SerializeField] private ProjectileSO fireMissileData1;
    [SerializeField] private ProjectileSO fireMissileData2;
    [SerializeField] private int numMissiles;
    [SerializeField] private float fireMissileWaitTime;


    private Animator animator;
    private BossHealth health;
    private float timerBetweenMoves;
    private bool isAttacking = false;
    private bool canAttack;

    private void Awake()
    {
        
        animator = GetComponent<Animator>();
        TryGetComponent(out health);
        timerBetweenMoves = timeBetweenMoves;
        StartCoroutine(AttackRoutine());

        //SpawnFireMissile();
        //SpawnFireBall();
        //SpawnFireLog();
    }
    private void OnEnable()
    {
        health.OnBossDefeated += BossDefeated;
    }

    private void OnDisable()
    {
        health.OnBossDefeated -= BossDefeated;
    }

    private void FixedUpdate()
    {
        if (timerBetweenMoves >= 0)
        {
            timerBetweenMoves -= Time.fixedDeltaTime;
            if(timerBetweenMoves < 0)
            {
                canAttack = true;
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        

        while (true)
        {
            if (!isAttacking && canAttack) 
            {
                
                int randomAttack = Random.Range(0, 3);
                animator.SetInteger("AttackNum", randomAttack);
                animator.SetBool("IsAttacking", true);
                isAttacking= true;
                canAttack= false;
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    /*IEnumerator FireBall()
    {
        isAtacking = true;
        animator.SetInteger("AttackNum", 0);
        SpawnFireBall();
        yield return new WaitForSeconds(fireBallWaitTime);
        isAtacking = false;
        animator.SetBool("IsAttacking", false);
    }

    IEnumerator FireLog()
    {
        isAtacking = true;
        animator.SetInteger("AttackNum", 1);
        SpawnFireLog();
        yield return new WaitForSeconds(fireLogWaitTime);  
        isAtacking = false;
        animator.SetBool("IsAttacking", false);
    }

    IEnumerator FireMissile() 
    {
        isAtacking = true;
        animator.SetInteger("AttackNum", 2);
        SpawnFireMissile();
        yield return new WaitForSeconds(fireMissileWaitTime);
        isAtacking = false;
        animator.SetBool("IsAttacking", false);
    }*/
    
    public void SpawnFireBall()
    {
        smallSpit.Play();
        IProjectile projectile = Instantiate(fireBallData.EnemyProjectile, topFaceTransform.position, Quaternion.identity).GetComponent<IProjectile>();
        Vector2 direction = Vector2.left;
        projectile.Initialize(direction, fireBallLaunchForce);

    }

    public void SpawnFireLog()
    {
        smallSpit.Play();
        IProjectile projectile = Instantiate(fireLogData.EnemyProjectile, midFaceTransform.position, Quaternion.identity).GetComponent<IProjectile>();
        Vector2 direction = Vector2.left;
        projectile.Initialize(direction, logLaunchForce);
    }

    public void SpawnFireMissile()
    {
        Vector2 direction = Vector2.left;
        for (int i = 0; i < numMissiles; i++) 
        {
            switch (i)
            {
                case 0:
                    largeSpit.Play();
                    IProjectile projectile0 = Instantiate(fireMissileData2.EnemyProjectile, topFaceTransform.position, Quaternion.identity).GetComponent<IProjectile>();
                    projectile0.Initialize(direction, 0);
                    break;
                case 1:
                    largeSpit.Play();
                    IProjectile projectile1 = Instantiate(fireMissileData.EnemyProjectile, midFaceTransform.position, Quaternion.identity).GetComponent<IProjectile>();
                    projectile1.Initialize(direction, 0);
                    break;
                case 2:
                    largeSpit.Play();
                    IProjectile projectile2 = Instantiate(fireMissileData1.EnemyProjectile, botFaceTransform.position, Quaternion.identity).GetComponent<IProjectile>();
                    projectile2.Initialize(direction, 0);
                    break;
            }
        }
    }

    public void OnAttackFinish()
    {
        animator.SetBool("IsAttacking", false);
        animator.SetInteger("AttackNum", -1);
        isAttacking = false;
        timerBetweenMoves = timeBetweenMoves;
    }

    private void BossDefeated()
    {
        BossManager.Instance.DefeatBoss(bossPart);
    }
}
