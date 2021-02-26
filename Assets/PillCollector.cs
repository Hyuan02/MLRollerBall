using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillCollector : MonoBehaviour
{
    public PillTypes type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Agent"))
        {
            transform.parent.gameObject.GetComponent<ItensSpawner>().CollectedPill(type, this.gameObject);
        }
    }
}
