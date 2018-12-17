﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region Debug
#if UNITY_EDITOR
using UnityEditor;
#endif 
#endregion

public class EnemyScript : MonoBehaviour
{
    [Header("Life")]
    [Space]
    [SerializeField]
    private float dieFadeTime = 2f;

    [Header("Movement")]
    [Space]
    [SerializeField]
    protected float moveSpeed = 1f;

    [Space]
    [SerializeField]
    [Tooltip("Time until enemy inverts direction. 0 for instantaneous.")]
    private float flipTimer = 2f;

    [Header("Attack")]
    [Space]
    [SerializeField]
    protected float damage = 20f;
    [SerializeField]
    [Tooltip("In seconds")]
    protected float damageToFutureSelf = 5f;

    [Header("Settings")]
    [Space]
    [SerializeField]
    private Sprite deathSprite;

    [Space]
    [SerializeField]
    private Transform eyes;

    [Space]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private LayerMask groundLayerMask;
    [SerializeField]
    private LayerMask wallLayerMask;

    [Space]
    [SerializeField]
    private Transform playerCheck;
    [SerializeField]
    private LayerMask playerLayerMask;

    protected float currentMoveSpeed;

    protected bool isAlive;
    private bool waiting;
    protected bool isSeeingPlayer, canMove = true;

    protected Color initialColor;

    protected SpriteRenderer mySpriteRenderer;
    protected Animator myAnimator;
    protected Rigidbody2D myRigidbody2D;
    private Collider2D myCollider2D;

    protected virtual void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myCollider2D = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        isAlive = true;
        currentMoveSpeed = moveSpeed;

        initialColor = mySpriteRenderer.color;
    }

    protected virtual void FixedUpdate()
    {
        if (isAlive)
        {
            if (canMove)
            {
                if (Physics2D.Linecast(eyes.position, groundCheck.position, groundLayerMask) &&
                    !Physics2D.Linecast(eyes.position, groundCheck.position, wallLayerMask)) myRigidbody2D.velocity = new Vector2(currentMoveSpeed * transform.right.x, myRigidbody2D.velocity.y);
                else if (!waiting) StartCoroutine(Flip());
            }

            isSeeingPlayer = Physics2D.Linecast(eyes.position, playerCheck.position, playerLayerMask); 
        }
    }

    private IEnumerator Flip()
    {
        waiting = true;
        myAnimator.enabled = false;

        yield return new WaitForSeconds(flipTimer);

        Vector2 currentEulerAngles = transform.eulerAngles;
        currentEulerAngles.y += 180;
        transform.eulerAngles = currentEulerAngles;

        waiting = false;
        myAnimator.enabled = true;
    }

    public void TakeDamage()
    {
        isAlive = false;

        myAnimator.enabled = false;
        myRigidbody2D.isKinematic = true;
        myCollider2D.enabled = false;

        mySpriteRenderer.sprite = deathSprite;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float timeElapsed = 0;

        while (timeElapsed < dieFadeTime)
        {
            mySpriteRenderer.color = Color.Lerp(initialColor, Color.clear, Mathf.Clamp01(timeElapsed / dieFadeTime));

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        gameObject.SetActive(false);
    }

    #region Debug
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(eyes.position, groundCheck.position);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(eyes.position, playerCheck.position);
    }
#endif 
    #endregion
}
