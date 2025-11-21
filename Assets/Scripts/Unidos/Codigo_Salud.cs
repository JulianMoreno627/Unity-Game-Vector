using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Codigo_Salud : MonoBehaviour
{

    public float Salud = 100;
    public float SaludMaxima = 100;
    // Update is called once per frame

    public Image BarraSalud;
    public Text TextoSalud;
    public GameObject Muerto;
    public CanvasGroup OjosRojos;
    void Update()
    {
        if (OjosRojos.alpha > 0)
        {
            OjosRojos.alpha -= Time.deltaTime;
        }
        ActualizarInterfaz();
    }

    public void RecibirDaño(float Daño)
    {
        Salud -= Daño;
        OjosRojos.alpha = 1;
        if (Salud <= 0)
        {
            Instantiate(Muerto);
            Destroy(gameObject);
        }
    }


    public void RecibirCura(float cura)
    {
        Salud += cura;

        if (Salud > SaludMaxima)
        {
            Salud = SaludMaxima;
        }
    }


    void ActualizarInterfaz()
    {
        BarraSalud.fillAmount = Salud / SaludMaxima;
        TextoSalud.text = "Salud: " + Salud.ToString("f0");
    }
}