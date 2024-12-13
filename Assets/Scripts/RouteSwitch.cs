using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteSwitch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EnemyBehavior enemy = FindObjectOfType<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.ActivateAltRoute();
            }
        }
    }
}
