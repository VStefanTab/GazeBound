using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float speed = 3f;
    public float rotationSpeed = 5f;

    public Vector3 velocity;
    
    public void Move(Vector3 direction)
    {
        if(direction.magnitude > 0.1f)
        {
            velocity = direction.normalized * speed;

            transform.position += velocity * Time.deltaTime;

            Quaternion targetBot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetBot, rotationSpeed * Time.deltaTime);
        }
    }
}
