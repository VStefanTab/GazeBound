using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCBehaviourType
{
    wander,
    seek,
}

public class NPCBehaviour : MonoBehaviour
{
    public NPCBehaviourType behaviourType;
    public Transform player;
    public Transform target;
    public NPCController controller;

    public Vector3 wanderDirection;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<NPCController>();
        PickNewWanderDirection();
    }

    // Update is called once per frame
    void Update()
    {
        switch (behaviourType)
        {
            case NPCBehaviourType.wander:
                Wander();
                break;
            case NPCBehaviourType.seek:
                Seek(player.position);
                break;
        }
    }

    void Wander()
    {
        if (Random.value < 0.01f)
            PickNewWanderDirection();

        controller.Move(wanderDirection);
    }
    void PickNewWanderDirection()
    {
        wanderDirection = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f));
    }

    void Seek(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        controller.Move(direction);
    }
}
