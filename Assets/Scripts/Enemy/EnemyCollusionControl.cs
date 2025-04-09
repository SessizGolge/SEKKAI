using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision) 
    {
        gameObject.GetComponent<EnemyController>().isPatrolling = false;
    }
}
