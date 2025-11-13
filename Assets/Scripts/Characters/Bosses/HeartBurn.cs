using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HeartBurn : MonoBehaviour
{
    [Header("General Boss Settings")]
    [SerializeField] private BossPart bossPart;
    [SerializeField] private float waitTime;


    [Header("FireBall Settings")]
    [SerializeField] private float fireBallLaunchForce;
    [SerializeField] private Transform topFaceTransform;
    [SerializeField] private ProjectileSO fireBallData;
    [SerializeField] private float fireBallWaitTime;

    [Header("FireLog Settings")]
    [SerializeField] private Transform midFaceTransform;
    [SerializeField] private float logLaunchForce;
    [SerializeField] private ProjectileSO fireLogData;
    [SerializeField] private float fireLogWaitTime;

    [Header("FireMissile Settings")]
    [SerializeField] private Transform botFaceTransform;
    [SerializeField] private ProjectileSO fireMissileData;
    [SerializeField] private int numMissiles;
    [SerializeField] private float fireMissileWaitTime;


    private Animator animator;
    private bool isAtacking = false;

    private void Awake()
    {
        
        animator = GetComponent<Animator>();
        //StartCoroutine(AttackRoutine());
        SpawnFireMissile();
        //SpawnFireBall();
        //SpawnFireLog();
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            if (!isAtacking) 
            {
                int randomAttack = Random.Range(0, 3);
                switch (randomAttack)
                {
                    case 0:
                        StartCoroutine(FireBall()); break;
                    case 1:
                        StartCoroutine(FireLog()); break;
                    case 2:
                        StartCoroutine(FireMissile()); break;
                }
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator FireBall()
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
    }
    
    public void SpawnFireBall()
    {
        IProjectile projectile = Instantiate(fireBallData.EnemyProjectile, topFaceTransform.position, Quaternion.identity).GetComponent<IProjectile>();
        Vector2 direction = Vector2.left;
        projectile.Initialize(direction, fireBallLaunchForce);

    }

    public void SpawnFireLog()
    {
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
                    IProjectile projectile0 = Instantiate(fireMissileData.EnemyProjectile, topFaceTransform.position, Quaternion.identity).GetComponent<IProjectile>();
                    projectile0.Initialize(direction, 0);
                    break;
                case 1:
                    IProjectile projectile1 = Instantiate(fireMissileData.EnemyProjectile, midFaceTransform.position, Quaternion.identity).GetComponent<IProjectile>();
                    projectile1.Initialize(direction, 0);
                    break;
                case 2:
                    IProjectile projectile2 = Instantiate(fireMissileData.EnemyProjectile, botFaceTransform.position, Quaternion.identity).GetComponent<IProjectile>();
                    projectile2.Initialize(direction, 0);
                    break;
            }
        }
    }

}
