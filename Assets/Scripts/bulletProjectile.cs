using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletProjectile : MonoBehaviour
{

    private Rigidbody bulletRb;

    private void Awake()
    {
        bulletRb = GetComponent<Rigidbody>();

    }

    private void Start()
    {
        float speed = 10f;
        bulletRb.velocity = transform.forward*speed;


    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
