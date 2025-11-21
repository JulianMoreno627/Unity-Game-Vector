using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HacerCura : MonoBehaviour
{
    public float CantidadCura;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Codigo_Salud>())
        {
            other.GetComponent<Codigo_Salud>().RecibirCura(CantidadCura);

            Destroy(gameObject);
        }
    }
}