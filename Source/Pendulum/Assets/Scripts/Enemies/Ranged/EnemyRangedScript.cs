﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedScript : EnemyScript
{
    [Header("Unique Porperties")]
    [Space]
    [Header("Attack")]
    [SerializeField]
    private float shootForce = 300f;
    [SerializeField]
    private float damageForce = 200f;

    [Space]
    [SerializeField]
    private float attackCooldown = 2f;

    [Space]
    [SerializeField]
    private Color alertColor = Color.red;

    [Header("Settings")]
    [Space]
    [SerializeField]
    private Transform shootPoint;
    [SerializeField]
    private GameObject bulletPrefab;

    [Header("Audio")]
    [Space]
    [SerializeField]
    private string shootSound = "Enemy_Shoot";
    [SerializeField]
    private string bulletHitSound = "Enemy_Bullet_Hit";

    private int bulletID;

    private float attackTimer;

    private bool isAttacking;

    private PoolManagerScript poolManager;

    protected override void Awake()
    {
        base.Awake();

        mySpriteRenderer = GetComponent<SpriteRenderer>();

        poolManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<PoolManagerScript>();
    }

    protected override void Start()
    {
        base.Start();

        bulletID = poolManager.PreCache(bulletPrefab, 2);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isAlive)
        {
            myAnimator.SetFloat("Speed", Mathf.Round(Mathf.Clamp01(Mathf.Abs(myRigidbody2D.velocity.x))));

            if (isSeeingPlayer)
            {
                //canMove = false;
                //StopCoroutine("Flip");
                myAnimator.enabled = true;

                mySpriteRenderer.color = alertColor;

                if (!isAttacking && Time.time > attackTimer) Attack(true);
            }
            else
            {
                canMove = true;
                mySpriteRenderer.color = initialColor;
            } 
        }
    }

    public void Attack(bool toggle)
    {
        if (isAlive)
        {
            if (toggle)
            {
                isAttacking = true;

                myAnimator.SetTrigger("Attack");

                waiting = false;
            }
            else
            {
                isAttacking = false;
                attackTimer = attackCooldown + Time.time;
            } 
        }
    }

    private void Shoot()
    {
        GameObject bullet = poolManager.GetCachedPrefab(bulletID);

        bullet.transform.SetPositionAndRotation(shootPoint.position, transform.rotation);

        bullet.SetActive(true);
        bullet.GetComponent<EnemyBulletScript>().SetStats(damage, damageToFutureSelf, damageForce, bulletHitSound);
        bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(shootForce * transform.right.x, 0));

        audioManager.PlaySound(shootSound, gameObject.name, myAudioSource);
    }
}
