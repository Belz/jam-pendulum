﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    private float damage, damageToFutureSelf;
    private float damageForce;

    private TrailRenderer myTrailRenderer;

    private PlayerScript playerScript;

    private void Awake()
    {
        myTrailRenderer = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        myTrailRenderer.enabled = true;
        myTrailRenderer.emitting = true;
    }

    private void OnDisable()
    {
        myTrailRenderer.emitting = false;
        myTrailRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerScript = playerScript ?? collision.GetComponent<PlayerScript>();

            playerScript.TakeDamage(damage, damageToFutureSelf, damageForce, transform);
        }
        else if (collision.CompareTag("PlayerDummy"))
        {
            playerScript = playerScript ?? GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();

            playerScript.TakeDamage(damage, damageToFutureSelf, damageForce, transform, true);
        }

        gameObject.SetActive(false);
    }

    public void SetStats(float damageAmount, float damageToFS, float damageForceAmount)
    {
        damage = damageAmount;
        damageToFutureSelf = damageToFS;
        damageForce = damageForceAmount;
    }
}
