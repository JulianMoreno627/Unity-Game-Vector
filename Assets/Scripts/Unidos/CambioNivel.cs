using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioNivel : MonoBehaviour
{
    public int Nivel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
                SceneManager.LoadScene(Nivel);
        }
    }
}