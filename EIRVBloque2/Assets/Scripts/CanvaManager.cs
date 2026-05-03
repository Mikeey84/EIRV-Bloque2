using UnityEngine;
using System.Collections.Generic;

// Esto añade automáticamente el AudioSource al objeto que tenga este script
[RequireComponent(typeof(AudioSource))]
public class CanvaManager : MonoBehaviour
{
    [Header("Pantallas UI")]
    public GameObject canvaInicio;
    public GameObject canvaJuego;
    public GameObject canvaFinal;
    
    [Header("Coches")]
    public List<CarAI> objetosJuego;

    [Header("Audio")]
    public AudioClip sonidoBotonEmpezar; // Arrastra aquí tu sonido
    private AudioSource reproductorAudio;

    private void Awake()
    {
        // Configuramos el reproductor de audio para la UI (Sonido 2D)
        reproductorAudio = GetComponent<AudioSource>();
        reproductorAudio.playOnAwake = false;
        reproductorAudio.spatialBlend = 0f; // 0 = 2D (se escucha en ambos oídos por igual)
    }

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
        // 1. Reproducimos el sonido de confirmación al darle a Empezar
        if (sonidoBotonEmpezar != null)
        {
            reproductorAudio.PlayOneShot(sonidoBotonEmpezar);
        }

        // 2. Cambiamos las pantallas
        canvaInicio.SetActive(false);
        canvaJuego.SetActive(true);
        canvaFinal.SetActive(false);
        
        // 3. Arrancamos los coches
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