using UnityEngine;
using System.Collections;

public class EnemyCollusionControl : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision) 
    {
        gameObject.GetComponent<EnemyController>().isPatrolling = false;
    }
}
