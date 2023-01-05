using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private float _damage;

    public float damage
    { get { return _damage; } set { _damage = value; } }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player_Controller player = other.gameObject.GetComponent<Player_Controller>();

            player.TakeDamage(damage);
        }
        else
        if (other.gameObject.CompareTag("AI_Entity"))
        {
            if (other.gameObject.GetComponent<AI_Controller>())
            {
                AI_Controller AI = other.gameObject.GetComponent<AI_Controller>();

                AI.TakeDamage(damage);
            }
            else
            if (other.gameObject.GetComponent<AI_Skeleton>())
            {
                AI_Skeleton skeleton = other.gameObject.GetComponent<AI_Skeleton>();

                skeleton.TakeDamage(damage);
            }
            else
            if (other.gameObject.GetComponent<AI_Witch>())
            {
                AI_Witch skeleton = other.gameObject.GetComponent<AI_Witch>();

                skeleton.TakeDamage(damage);
            }
        }
    }
}
