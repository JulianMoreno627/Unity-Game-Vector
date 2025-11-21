using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptDeLaBala : MonoBehaviour
{
    public float bulletSpeed = 20f;  // Velocidad de la bala
    public int damageAmount = 10;    // Daño que hace

    void Start()
    {
        // (Opcional) Destruye la bala después de 5 segundos
        // si no choca con nada, para no llenar la escena.
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Mueve la bala hacia adelante (en su eje Z local)
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
    }

    // Esta función se llama porque el collider de la bala
    // es un "Trigger" y chocó con otro collider.
    private void OnTriggerEnter(Collider other)
    {
        // 1. Intentamos obtener el script "EnemyDeath" del objeto chocado
        EnemyDeath enemigo = other.GetComponent<EnemyDeath>();

        // 2. Si "enemigo" NO es nulo, significa que SÍ chocamos con un enemigo
        if (enemigo != null)
        {
            // 3. ¡Le hacemos daño!
            enemigo.DamageEnemy(damageAmount);
        }

        // 4. Destruimos la bala (sea pared o enemigo)
        //    (Excepto si choca con el "Player" al ser disparada)
        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}