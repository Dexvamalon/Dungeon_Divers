using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("References")]
    private Info info;

    private void Start()
    {
        if(gameObject.layer == 7)
        {
            info = GetComponent<Info>();
        }
        else
        {
            info = GetComponentInParent<Info>();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    { 
        if(collision.tag == "Player" )
        {
            Vector3 playerPosition = collision.GetComponent<Transform>().position;
            float leeway = collision.GetComponent<PlayerMovement>().bounceLeeway;
            BoxCollider2D boxCol2d = GetComponent<BoxCollider2D>();

            if (playerPosition.y + leeway > transform.position.y - info.GetLength() / 2 && playerPosition.y - leeway < transform.position.y + info.GetLength()/2)
            {
                List<GameObject> obstacles = new List<GameObject>();
                if (gameObject.layer == 7)
                {
                    for (int i = 0; i < transform.parent.childCount; i++)
                    {
                        obstacles.Add(transform.parent.GetChild(i).gameObject);
                    }
                }
                else
                {
                    for (int i = 0; i < transform.parent.parent.childCount; i++)
                    {
                        obstacles.Add(transform.parent.parent.GetChild(i).gameObject);
                    }
                }

                collision.GetComponent<PlayerMovement>().BouncePlayer(obstacles);
            }

            if(playerPosition.x > transform.position.x - (transform.lossyScale.x * boxCol2d.size.x) / 2 + boxCol2d.offset.x &&
                playerPosition.x < transform.position.x + (transform.lossyScale.x * boxCol2d.size.x) / 2 + boxCol2d.offset.x && !collision.GetComponent<PlayerMovement>().invicible)
            {
                collision.GetComponent<PlayerHealth>().TakeDamage(info.damage);
            }
        }
    }
}
