using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEmitter : MonoBehaviour
{
    float timer = 1;
    public GameObject blood;

    void FixedUpdate()
    {
        timer -= 1;
        if (timer <= 0)
        {
            // Create blood instance...
            var inst = GameObject.Instantiate(blood);
            inst.transform.position = transform.position;
            GameObject.Destroy(gameObject);
            return;
        }

    }
}
