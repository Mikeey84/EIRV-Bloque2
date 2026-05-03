using System.Collections.Generic;
using UnityEngine;

public class CanvaManager : MonoBehaviour
{
    public GameObject canvaInicio;
    public GameObject canvaJuego;
    public GameObject canvaFinal;
    
    public List<CarAI> objetosJuego;

    private void Start()
    {
        foreach (CarAI objeto in objetosJuego)
        {
            objeto.move = false;
        }
    }

    public void MostrarCanvaInicio()
    {
        canvaInicio.SetActive(true);
        canvaJuego.SetActive(false);
        canvaFinal.SetActive(false);
    }
    public void MostrarCanvaJuego()
    {
        canvaInicio.SetActive(false);
        canvaJuego.SetActive(true);
        canvaFinal.SetActive(false);
        foreach (CarAI objeto in objetosJuego)
        {
            objeto.move = true;
        }
    }
    public void MostrarCanvaFinal()
    {
        canvaInicio.SetActive(false);
        canvaJuego.SetActive(false);
        canvaFinal.SetActive(true);
    }
}
