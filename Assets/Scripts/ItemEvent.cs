using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEvent : MonoBehaviour
{
    public GameObject target;

    private void OnTriggerEnter(Collider other)
    {
        target.GetComponent<LeverRotate>().autoRotate(90f);

        Destroy(gameObject);
    }
}
