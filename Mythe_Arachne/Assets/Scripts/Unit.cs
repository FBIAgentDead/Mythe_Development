﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public Vector2 position;
    public Vector2 velocity;
    public Vector2 acceleration;

    public AllUnits allUnits;
    public UnitConfig unitConfig;

    private Vector2 wanderTarget;

    private UnitHolder unitHolder;

    private Rigidbody2D rigidbody2D;

    private bool canJump;

    [SerializeField]
    private bool grounded;

    private Collider2D collider;

    [SerializeField]
    private LayerMask groundLayer;

    private float gravity;

    // Use this for initialization
    void Start()
    {
        collider = GetComponent<Collider2D>();
        canJump = true;
        allUnits = FindObjectOfType<AllUnits>();
        unitConfig = FindObjectOfType<UnitConfig>();
        unitHolder = FindObjectOfType<UnitHolder>();

        rigidbody2D = GetComponent<Rigidbody2D>();

        gravity = unitConfig.gravity;

        position = transform.position;
        velocity = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
    }

    void Update()
    {

        grounded = IsGrounded();

        acceleration = Combine();

        acceleration = Vector2.ClampMagnitude(acceleration, unitConfig.maxAcceleration);

        velocity = velocity + acceleration * Time.deltaTime;
        velocity = Vector2.ClampMagnitude(velocity, unitConfig.maxVelocity);

        velocity.y -= gravity;

        Jump();

        Debug.Log(velocity.y);

        rigidbody2D.velocity = velocity;


        position = transform.position;
    }

    protected Vector2 Wander()
    {
        float jitter = unitConfig.wanderJitter * Time.deltaTime;

        wanderTarget += new Vector2(RandomBinomial() * jitter, 0);
        wanderTarget = wanderTarget.normalized;
        wanderTarget *= unitConfig.wanderRadius;

        Vector2 targetInWorldSpace = transform.TransformPoint(wanderTarget);
        targetInWorldSpace -= position;
        return targetInWorldSpace.normalized;
    }

    private void Jump()
    {
        if (!canJump || !grounded) return;
        StartCoroutine(JumpCooldown());
        StartCoroutine(GravityCooldown());
        //rigidbody2D.AddForce(new Vector2(0, unitConfig.jumpForce));

        gravity = unitConfig.gravityJump;
        velocity.y += unitConfig.jumpForce;

    }

    private Vector2 Follow()
    {
        Vector2 followVector = new Vector2();

        Vector2 followPoint = new Vector2(position.x, 0);

        followVector = (Vector2) unitHolder.transform.position - followPoint;

        followVector.y = 0;

        return followVector;
    }

    private Vector2 Cohesion()
    {
        if (!IsGrounded()) return Vector2.zero;
        Vector2 cohesionVector = new Vector2();
        int countUnits = 0;
        var neighbors = allUnits.GetAllNeighbors();

        if (neighbors.Count == 0)
            return cohesionVector;

        foreach (var unit in neighbors)
        {
            if (IsInFOV(unit.position))
            {
                cohesionVector += unit.position;
                countUnits++;
            }
        }

        if (countUnits == 0)
            return cohesionVector;

        cohesionVector /= countUnits;
        cohesionVector = cohesionVector - position;
        cohesionVector = Vector3.Normalize(cohesionVector);

        return cohesionVector;
    }

    private Vector2 Separation()
    {
        if (!IsGrounded()) return Vector2.zero;
        Vector2 separateVector = new Vector2();
        var units = allUnits.GetNeighbors(this, unitConfig.separationRadius);

        if (units.Count == 0)
            return separateVector;

        foreach (var unit in units)
        {
            if (IsInFOV(unit.position))
            {
                Vector2 movingTowards = position - unit.position;

                if (movingTowards.magnitude > 0)
                {
                    separateVector += movingTowards.normalized / movingTowards.magnitude;
                }
            }
        }

        return separateVector.normalized;
    }

    virtual protected Vector2 Combine()
    {
        Vector2 finalVec = unitConfig.cohesionPriority * Cohesion() + unitConfig.wanderPriority * Wander()
            + unitConfig.separationPriority * Separation() + unitConfig.followPriority * Follow();
        return finalVec;
    }

    private float RandomBinomial()
    {
        return Random.Range(0f, 1f) - Random.Range(0f, 1f);
    }

    bool IsInFOV(Vector2 vec)
    {
        return Vector2.Angle(velocity, vec - position) <= unitConfig.maxFOV;
    }

    private IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(unitConfig.jumpCooldown);

        canJump = true;
    }

    private IEnumerator GravityCooldown()
    {
        yield return new WaitForSeconds(unitConfig.gravityJumpCooldown);

        gravity = unitConfig.gravity;
    }

    private bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 0.3f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            Debug.DrawLine(transform.position, transform.position - new Vector3(0, distance, 0), Color.red);
            return true;
        }

        return false;
    }
}
