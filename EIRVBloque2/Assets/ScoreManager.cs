using UnityEngine;
using TMPro;
using System.Collections;

// Esta línea asegura que Unity añada un AudioSource automáticamente si no lo tiene
[RequireComponent(typeof(AudioSource))] 
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instancia;

    [Header("Puntuación")]
    public int puntosTotales = 0;
    public TextMeshProUGUI textoPuntuacion;

    [Header("Animación")]
    public float multiplicadorEscalaMax = 1.3f; 
    public float duracionAnimacion = 0.25f; 
    public AnimationCurve curvaDePunch;

    [Header("Audio Arcade")]
    [Tooltip("Arrastra aquí el archivo mp3 o wav de tu sonido de punto")]
    public AudioClip sonidoPunto; 
    private AudioSource reproductorAudio;

    private Vector3 escalaOriginal;
    [SerializeField]
    private double volume = 0.5;

    private void Awake()
    {
        if (instancia == null) instancia = this;
        else Destroy(gameObject);

        // Referenciamos el componente de audio al inicio
        reproductorAudio = GetComponent<AudioSource>();
        //reproductorAudio.volume = volume;
    }

    private void Start()
    {
        if (textoPuntuacion != null)
        {
            escalaOriginal = textoPuntuacion.transform.localScale;
            textoPuntuacion.text = puntosTotales.ToString();
        }
    }

    public void SumarPuntos(int cantidad)
    {
        // 1. Sumamos los puntos
        puntosTotales += cantidad;
        textoPuntuacion.text = puntosTotales.ToString();
        
        // 2. Reproducimos el sonido (PlayOneShot permite que suenen varios a la vez si encestas muy rápido)
        if (sonidoPunto != null)
        {
            // Cambiamos un poquito el tono (+- 5%) para que suene orgánico y no canse
            reproductorAudio.pitch = Random.Range(1.0f, 1.4f);
            reproductorAudio.PlayOneShot(sonidoPunto);

        }

        // 3. Lanzamos la animación
        StopAllCoroutines();
        StartCoroutine(EfectoPunchSmooth());
    }

    IEnumerator EfectoPunchSmooth()
    {
        float tiempo = 0f;
        Vector3 escalaMaxima = escalaOriginal * multiplicadorEscalaMax;

        while (tiempo < duracionAnimacion)
        {
            tiempo += Time.deltaTime;
            float valorCurva = curvaDePunch.Evaluate(tiempo / duracionAnimacion);
            textoPuntuacion.transform.localScale = Vector3.LerpUnclamped(escalaOriginal, escalaMaxima, valorCurva);
            yield return null;
        }
        textoPuntuacion.transform.localScale = escalaOriginal;
    }
}