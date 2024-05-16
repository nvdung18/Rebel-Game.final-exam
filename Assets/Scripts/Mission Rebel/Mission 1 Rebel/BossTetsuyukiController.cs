using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTetsuyukiController : MonoBehaviour
{
    [Header("Enemy information")]
    public GameObject body;
    public GameObject cannon;
    public GameObject body187;
    public float attackDamage = 25f;
    private Health health;
    private float maxHealth;
    private BlinkingSprite blinkingSprite;
    public GameObject projSpawnerLow;
    public GameObject projSpawnerHigh;
    public GameObject LaserSpawnerLow;
    public GameObject LaserSpawnerHigh;
    public GameObject CoverCloth;
    public GameObject propeller_1;
    public GameObject propeller_2;
    public GameObject propeller_destroy;
    public List<GameObject> listSimpleSoldiers;

    [Header("Fire")]
    public GameObject normalFire;
    public GameObject LaserLow;
    public GameObject LaserHigh;
    public bool canFire= true;

    [Header("Enemy activation")]
    public const float CHANGE_SIGN = -1;

    private Animator animator;
    private Boolean isBossActive = false;
    private BlinkingSprite cannonBlinkingSprite;
    private BlinkingSprite body187BlinkingSprite;
    Transform playerTransform;

    [Header("Time shoot")]
    private float shotTime = 0.0f;
    public float fireDelta = 0.5f;
    private float nextFire = 2f;

    private Transform launchProjSpawnerLow;
    private Transform launchProjSpawnerHigh;

    private bool isMovingLow=true;
    private bool isMovingHigh = false;
    private int numsOfFiring = 0;
    private int numsOfLaser= 0;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        registerHealth();
        maxHealth = health.GetMaxHealth();
        cannonBlinkingSprite= cannon.GetComponent<BlinkingSprite>();
        body187BlinkingSprite=body187.GetComponent<BlinkingSprite>();
        launchProjSpawnerLow = projSpawnerLow.transform;
        launchProjSpawnerHigh = projSpawnerHigh.transform;
        playerTransform = GameManager.GetPlayer().transform;
    }
    private void registerHealth()
    {
        health = body.GetComponent<Health>();
        // register health delegate
        health.onDead += OnDead;
        health.onHit += OnHit;
        // immortal until activeBoss
        health.immortal = true;

    }
    private void OnDead(float damage)
    {
        StartCoroutine(BossDead());
    }
    private void StopBossCoroutines()
    {
        StopAllCoroutines();
    }
    private void OnHit(float damage)
    {
        GameManager.AddScore(damage);
        body.GetComponent<BlinkingSprite>().Play();
        cannonBlinkingSprite.Play();
        body187BlinkingSprite.Play();
    }

    private void FixedUpdate()
    {
        if (GameManager.IsGameOver())
            return;
        if (isBossActive)
        {
            if (health.IsAlive())
            {

                if (isMovingLow)
                {
                    if (numsOfFiring == 3)
                    {
                        if (!animator.GetBool("isLaserLow"))
                        {
                            StartCoroutine(DelayShootLaser());
                        }
                    }
                    else
                    {
                        if (!animator.GetBool("isFiringLow"))
                        {
                            StartCoroutine(DelayShootBullet());
                        }
                    }
                }
                else if (isMovingHigh)
                {
                    if(numsOfFiring == 3)
                    {
                        if (!animator.GetBool("isLaserHigh"))
                        {
                            StartCoroutine(DelayShootLaser());
                        }
                    }
                    else
                    {
                        if (!animator.GetBool("isFiringHigh"))
                        {
                            StartCoroutine(DelayShootBullet());
                        }
                    }
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
    }

    public void TriggerActiveBoss()
    {
        if (!animator.GetBool("isCoverCloth"))
        {
            animator.SetBool("isCoverCloth", true);
            StartCoroutine(BossActive());
        }
    }

    private IEnumerator BossActive()
    {
        yield return new WaitForSeconds(.5f);
        health.immortal = false;
        isBossActive = true;
        yield return new WaitForSeconds(.5f);
        Destroy(CoverCloth);
        animator.SetBool("isCoverCloth", false);
    }

    public void FiringLow()
    {
        StartCoroutine(WaitFire(normalFire, launchProjSpawnerLow));
    }
    public void FiringHigh()
    {
        StartCoroutine(WaitFire(normalFire, launchProjSpawnerHigh));
    }
    public void AttackLaserLow()
    {
        StartCoroutine(WaitAttackLaser(LaserLow,LaserSpawnerLow));
    }

    public void AttackLaserHigh()
    {
        StartCoroutine(WaitAttackLaser(LaserHigh, LaserSpawnerHigh));
    }
    private IEnumerator WaitFire(GameObject throwableObj, Transform projSpawner)
    {
        yield return new WaitForSeconds(0.1f);
        Instantiate(throwableObj, projSpawner.position, projSpawner.rotation);
        yield return new WaitForSeconds(0.15f);
    }
    private IEnumerator DelayShootBullet()
    {
        if (isMovingLow)
        {
            yield return new WaitForSeconds(.5f);
            animator.SetBool("isFiringLow", true);
            animator.SetBool("isMovingLow", true);
            yield return new WaitForSeconds(3.5f);
            /*numsOfFiring += 0.5f;*/
            animator.SetBool("isFiringLow", false);
            animator.SetBool("isMovingLow", false);
            isMovingLow = false;
            isMovingHigh = true;
        }
        else if(!isMovingLow)
        {
            if (animator.GetBool("isMovingHigh") == false)
            {
                animator.SetBool("isMovingHigh", true);
                animator.SetBool("isFiringHigh", false);
            }
            yield return new WaitForSeconds(.5f);
            animator.SetBool("isFiringHigh", true);
            yield return new WaitForSeconds(3.5f);
            if(animator.GetBool("isFiringHigh"))
            {
                numsOfFiring += 1;
            }
            animator.SetBool("isMovingHigh", false);
            animator.SetBool("isFiringHigh", false);
            animator.SetInteger("numOfShootBullet", 3);
            isMovingLow = true;
            isMovingHigh = false;
        }
    }

    private IEnumerator DelayShootLaser()
    {
        if (isMovingLow)
        {
            animator.SetBool("isLaserLow", true);
            animator.SetBool("isMovingLow", true);
            yield return new WaitForSeconds(3.5f);
            animator.SetBool("isLaserLow", false);
            animator.SetBool("isMovingLow", false);
            isMovingLow = false;
            isMovingHigh = true;
        }else if (!isMovingLow)
        {
            yield return new WaitForSeconds(.5f);
            animator.SetBool("isMovingHigh", true);
            animator.SetBool("isLaserHigh", true);
            yield return new WaitForSeconds(3.5f);
            if (numsOfLaser < 2)
            {
                numsOfLaser += 1;
            }
            if(numsOfLaser==2)
            {
                numsOfFiring = 0;
            }
            animator.SetBool("isMovingHigh", false);
            animator.SetBool("isLaserHigh", false);
            isMovingLow = true;
            isMovingHigh = false;
        }
    }

    private IEnumerator WaitAttackLaser(GameObject laser, GameObject laserSpawner)
    {
        yield return new WaitForSeconds(0.1f);
        Instantiate(laser, laserSpawner.transform.position, laserSpawner.transform.rotation);
        yield return new WaitForSeconds(0.15f);
    }

    private IEnumerator BossDead()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("isDying", true);
        propeller_1.SetActive(false);
        propeller_2.SetActive(false);
        propeller_destroy.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isDying", false);
        yield return new WaitForSeconds(2f);GameManager.PlayerWin();
        GameManager.PlayerWin();
        StopBossCoroutines();
        yield return new WaitForSeconds(0.15f);
    }


}
