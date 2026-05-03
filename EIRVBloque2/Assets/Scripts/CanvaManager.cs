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
    private AudioSource reproductorAudio;
    public AudioSource reproductorAudio2;
    public AudioSource reproductorAudio3;


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
    public void PlaySound()
    {
        reproductorAudio.Play();
    }
    public void MostrarCanvaJuego()
    {

        // 2. Cambiamos las pantallas
        canvaInicio.SetActive(false);
        canvaJuego.SetActive(true);
        canvaFinal.SetActive(false);
        reproductorAudio2.Play();
        reproductorAudio3.Play();
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