using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tetsuyuki_FireMovement : MonoBehaviour
{
    [Header("Damage Details")]
    private float bulletDamageBoss = 15f;
    private float LaserDamageBoss = 25f;
    public float throwableForce = 2.5f;


    public enum LauncherType
    {
        Player,
        Enemy
    };
    public LauncherType launcher = LauncherType.Player;

    public enum ThrowableType
    {
        BossBullet,
        Laser,
        LaserHigh
    };
    public ThrowableType throwable = ThrowableType.BossBullet;

    public bool canExplode = true;


    private Animator throwableAnimator;
    private Rigidbody2D rb;

    Vector3 throwableDirection;

    private bool hasHit;
    private bool isSpawned;
    public float speed = 5f;
    public float bulletForce = 10;

    public float lifeTimeLaserHigh = 1f;
    private float expireTime;

    private void Start()
    {
        throwableAnimator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        Init();
    }

    void Update()
    {
        if (throwable == ThrowableType.LaserHigh)
        {
            if (!isSpawned)
                return;

            expireTime -= Time.deltaTime;
            if (expireTime <= 0)
                Despawn();
        }
    }

    void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        if (throwable == ThrowableType.Laser)
        {
            // Lấy vị trí của người chơi
            Vector3 rightDirection = transform.right;
            Vector3 bulletDirection = -rightDirection;
            /*rb.AddForce(bulletDirection * bulletForce, ForceMode2D.Impulse);*/
            rb.velocity = bulletDirection * 2f;
        }
        else if (throwable == ThrowableType.BossBullet)
        {
            // Lấy vị trí của người chơi
            Vector3 playerPosition = GameManager.GetPlayer().transform.position;
            Vector3 direction = playerPosition - transform.position;
            float distance = direction.magnitude;

            // Tính toán lực ném cần thiết
            Vector2 launchForce = CalculateLaunchForce(direction, distance);

            /*throwableDirection = Quaternion.AngleAxis(45, Vector3.forward) * Vector3.right;*/
            if (distance <= 1f)
                rb.AddForce(launchForce, ForceMode2D.Impulse);
            else if (distance > 1f)
                rb.AddForce(Quaternion.AngleAxis(30, Vector3.forward) * Vector3.right * launchForce, ForceMode2D.Impulse);
        }
        else if(throwable == ThrowableType.LaserHigh)
        {
            expireTime = lifeTimeLaserHigh;
        }
        else 
            return;
        
        hasHit = false;
        isSpawned = true;
    }
    Vector2 CalculateLaunchForce(Vector3 direction, float distance)
    {
        // Tính toán thời gian rơi
        float time = Mathf.Sqrt((2 * distance) / Physics2D.gravity.magnitude);

        // Tính toán vận tốc y cần thiết để viên đạn rơi đúng vào targetPos
        float launchSpeedY = Physics2D.gravity.magnitude * time;

        // Tính toán lực ném cần thiết dựa trên vận tốc y
        float launchForceY = launchSpeedY * rb.mass;

        // Tính toán lực ném theo hướng x
        float launchForceX = direction.x / time;

        // Điều chỉnh lực ném để cải thiện độ chính xác
        //1.0=>35 degree, 0.5=>45 degree
        if (distance <= 1f)
            launchForceX *= 0.5f;// Điều chỉnh hệ số lực ném
        else if (distance > 1f)
            launchForceX *= 1.0f;

        return new Vector2(launchForceX, launchForceY);
    }

    private void Despawn()
    {
        if (!isSpawned)
            return;

        isSpawned = false;

        Destroy(gameObject);

    }

    //Destroy the bulled when out of camera
    private void OnBecameInvisible()
    {
        if (throwable == ThrowableType.Laser)
        {
            Despawn();
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (hasHit)
            return;
        if (throwable == ThrowableType.BossBullet)
        {
            if (GameManager.CanTriggerThrowable(collider) && !(launcher == LauncherType.Player && GameManager.IsPlayer(collider)) && !(launcher == LauncherType.Enemy && (collider.CompareTag("Enemy") || collider.CompareTag("EnemyBomb"))))
            {
                ResetMovement(collider);
                Despawn();
            }
        }else if (throwable == ThrowableType.LaserHigh)
        {
            if (GameManager.IsPlayer(collider))
            {
                ResetMovement(collider);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (throwable == ThrowableType.Laser)
        {
            if (GameManager.CanTriggerThrowable(collision.collider) && GameManager.IsPlayer(collision.collider))
            {
                ResetMovement(collision.collider);
                Despawn();
            }
        }
    }

    private void ResetMovement(Collider2D collider)
    {
        var target = collider.gameObject;
        if (GameManager.IsPlayer(collider))
            target = GameManager.GetPlayer(collider);

        switch (throwable)
        {
            case ThrowableType.BossBullet:
                target.GetComponent<Health>()?.Hit(bulletDamageBoss);
                rb.angularVelocity = 0;
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;
                break;
            case ThrowableType.Laser:
                target.GetComponent<Health>()?.Hit(LaserDamageBoss);
                rb.angularVelocity = 0;
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;
                break;
            case ThrowableType.LaserHigh:
                target.GetComponent<Health>()?.Hit(LaserDamageBoss);
                break;
        }

        
    }
}
