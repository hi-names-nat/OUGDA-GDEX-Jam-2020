using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogMovementAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(player.position);
    }
}
