using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private int width = 1;
    [SerializeField] private List<bool> structureGround = new List<bool>();
    [SerializeField] private List<bool> structureAir = new List<bool>();
    private bool[] occupyingLanes = new bool[4];
    private bool[] occupyingLanesInAir = new bool[4];
    [SerializeField] private float end = 1f;
    [SerializeField] private float start = -1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Vector3 playerPosition = collision.GetComponent<Transform>().position;
            float leeway = collision.GetComponent<PlayerMovement>().bounceLeeway;
            BoxCollider2D boxCol2d = GetComponent<BoxCollider2D>();

            if(playerPosition.y + leeway > transform.position.y + start && playerPosition.y - leeway < transform.position.y + end)
            {
                collision.GetComponent<PlayerMovement>().BouncePlayer();
            }

            if(playerPosition.x > transform.position.x - (transform.lossyScale.x * boxCol2d.size.x) / 2 + boxCol2d.offset.x &&
                playerPosition.x < transform.position.x + (transform.lossyScale.x * boxCol2d.size.x) / 2 + boxCol2d.offset.x)
            {
                collision.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }
    }
}
