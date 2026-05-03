using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    // Esto crea un "Singleton": una línea directa global a este script
    public static ScoreManager instancia;

    [Header("Puntuación")]
    public int puntosTotales = 0;
    public TextMeshProUGUI textoPuntuacion;

    [Header("Animación")]
    public float multiplicadorEscalaMax = 1.3f; 
    public float duracionAnimacion = 0.25f; 
    public AnimationCurve curvaDePunch;

    private Vector3 escalaOriginal;

    private void Awake()
    {
        // Configuramos el Singleton al arrancar
        if (instancia == null) instancia = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (textoPuntuacion != null)
        {
            escalaOriginal = textoPuntuacion.transform.localScale;
            textoPuntuacion.text = puntosTotales.ToString();
        }
    }

    // Esta es la función que los buzones llamarán
    public void SumarPuntos(int cantidad)
    {
        puntosTotales += cantidad;
        textoPuntuacion.text = puntosTotales.ToString();
        
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