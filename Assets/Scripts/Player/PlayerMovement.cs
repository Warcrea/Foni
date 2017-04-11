﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {

    public Animator animator;
    private Rigidbody rb;
    public GameObject bulletPrefab;
    private GameObject enemy;

    private enum states { idle, attack1, attack2, attack3 };
    private const string performAttackString =  "PerformAttack";
    public bool ableToAttack { get; set; } //While this is true, pressing the attack button will set the animation attacking bool to true
    private states state;

    public bool enemyInAttackZone {get; set;}

    public float runSpeed = 0.5f;
    public float minDashDistance = 3f;
    public float maxDashDistance = 6f;

    private bool canFire;
    private float fireRate = 0.25f;
    private float fireRateTimer;

    void Start() {
        rb = GetComponent<Rigidbody>();
        enemy = GameObject.Find("Enemy");
        ableToAttack = true;
        canFire = true;
        fireRateTimer = fireRate;
    }

    void OnCollisionEnter(Collision coll) {
        Debug.Log(coll.transform.name);
    }

    void Update() {
        CheckInputs();
        if (!canFire) {
            fireRateTimer -= Time.deltaTime;
            if (fireRateTimer < 0) {
                canFire = true;
                fireRateTimer = fireRate;
            }
        }
    }

	void CheckInputs() {
        if (Input.GetButtonDown("Dash"))
            Dash();
        if (Input.GetButtonDown("Attack"))
            TryAttack();
    }
	
    void TryAttack() {
        if (ableToAttack) {
            animator.SetBool(performAttackString, true);
            ableToAttack = false;
        }
    }

	// Update is called once per frame
	void FixedUpdate () {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        transform.Translate((x * runSpeed) * Time.fixedDeltaTime, 0, (y * runSpeed) * Time.fixedDeltaTime, Space.World);
        if (x != 0 || y != 0) {
            transform.rotation = Quaternion.LookRotation(new Vector3(x, 0, y));
        }
        CheckShooting();
    }

    void CheckShooting() {
        float rX = Input.GetAxis("Right Horizontal");
        float rY = Input.GetAxis("Right Vertical");

        if (canFire && (rX != 0 || rY != 0)) {
            Shoot(rX, rY);
            canFire = false;
        }
    }

    void Shoot(float x, float y) {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetDirection(new Vector3(x, 0, -y));
        bullet.transform.SetParent(GameObject.FindGameObjectWithTag("Bullet Holder").transform);
    }

    void Dash() {
        transform.position += transform.forward * minDashDistance;
    }

    public void AttackImpact() {
        if (enemyInAttackZone) {
            Debug.Log("Hit the enemy");
        }
    }

}