using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public GameObject explosion;

    private bool _explode = true;

    private void OnTriggerEnter(Collider other)
    {
        if (_explode)
        {
            GameObject particle = Instantiate(explosion, transform.position, Quaternion.identity);

            Destroy(particle, 1f);
            Destroy(gameObject);
        }
    }
}
