using UnityEngine;

public class CanvaManager : MonoBehaviour
{
    public GameObject canvaInicio;
    public GameObject canvaJuego;
    public GameObject canvaFinal;
    
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
    }
    public void MostrarCanvaFinal()
    {
        canvaInicio.SetActive(false);
        canvaJuego.SetActive(false);
        canvaFinal.SetActive(true);
    }
}
