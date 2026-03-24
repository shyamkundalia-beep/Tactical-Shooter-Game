using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAI : MonoBehaviour
{
    [Header ("Player Health and Damage")]
    public float PlayerHealth = 120f;
    public float presentHealth;
    public float giveDamage = 5f;
    public float PlayerSpeed;


    [Header ("Player Things")]
    public NavMeshAgent PlayerAgent;
    public Transform LookPoint;
    public GameObject ShootingRaycastArea;
    public Transform enemyBody;
    public LayerMask EnemyLayer;
    public Transform Spawn;
    public Transform PlayerCharacter;


    [Header("Player Animation and Spark Effect")]
    public Animator anim;
    public ParticleSystem muzzleSpark;


    [Header("Player Shooting var")]
    public float timebtwShoot;
    bool previouslyShoot;


    [Header ("Player State")]
    public float visionRadius;
    public float shootingRadius;
    public bool enemyInvisionRadius;
    public bool enemyInshootingRadius;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip shootingSound;

    public ScoreManager scoreManager;


    private void Awake()
    {
        PlayerAgent = GetComponent<NavMeshAgent>();
        presentHealth = PlayerHealth;
    }


    private void Update()
    {
        enemyInvisionRadius = Physics.CheckSphere(transform.position, visionRadius, EnemyLayer);
        enemyInshootingRadius = Physics.CheckSphere(transform.position, shootingRadius, EnemyLayer);

        if(enemyInvisionRadius && !enemyInshootingRadius)
        {
            PursueEnemy();
        }
        if(enemyInvisionRadius && enemyInshootingRadius)
        {
            ShootEnemy();
        }
    }


    private void PursueEnemy()
    {
        if(PlayerAgent.SetDestination(enemyBody.position))
        {
            //animations
            anim.SetBool("Running",true);
            anim.SetBool("Shooting",false);
        }
        else
        {
            anim.SetBool("Running",false);
            anim.SetBool("Shooting",false);
        }
    }


    private void ShootEnemy()
    {
        PlayerAgent.SetDestination(transform.position);
        transform.LookAt(LookPoint);
        if (!previouslyShoot)
        {
            muzzleSpark.Play();
            audioSource.PlayOneShot(shootingSound);
            RaycastHit hit;

            if(Physics.Raycast(ShootingRaycastArea.transform.position, ShootingRaycastArea.transform.forward, out hit, shootingRadius))
            {
                Debug.Log("Shooting" + hit.transform.name);

                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if(enemy != null)
                {
                    enemy.enemyHitDamage(giveDamage);
                }
                anim.SetBool("Running",false);
                anim.SetBool("Shooting",true);
            }

            // anim.SetBool("Running",false);
            // anim.SetBool("Shooting",true);
            previouslyShoot = true;
            Invoke(nameof(ActiveShooting), timebtwShoot);
        }

        // previouslyShoot = true;
        // Invoke(nameof(ActiveShooting), timebtwShoot);
    }


    private void ActiveShooting()
    {
        previouslyShoot = false;
    }


    public void PlayerAIHitDamage(float takeDamage)
    {
        presentHealth = presentHealth - takeDamage;

        if(presentHealth <= 0)
        {
            StartCoroutine(Respawn());
        }
    }


    IEnumerator Respawn()
    {
        PlayerAgent.SetDestination(transform.position);
        PlayerSpeed = 0f;
        shootingRadius = 0f;
        visionRadius = 0f;
        enemyInvisionRadius = false;
        enemyInshootingRadius = false;
        anim.SetBool("Die",true);
        anim.SetBool("Running",false);
        anim.SetBool("Shooting",false);

        //animation

        Debug.Log("Dead");
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        scoreManager.enemyKills = scoreManager.enemyKills + 1;

        yield return new WaitForSeconds(5f);

        Debug.Log("Spawn");
        gameObject.GetComponent<CapsuleCollider>().enabled = true;

        presentHealth = 120f;
        PlayerSpeed = 1f;
        shootingRadius = 10f;
        visionRadius = 100f;
        enemyInvisionRadius = true;
        enemyInshootingRadius = false;

        //animations
        anim.SetBool("Die",false);
        anim.SetBool("Running",true);

        //spawnpoint
        PlayerCharacter.transform.position = Spawn.transform.position;
        PursueEnemy();

    }
}