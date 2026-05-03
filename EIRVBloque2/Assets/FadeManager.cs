using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    [Header("Referencias")]
    public CanvasGroup faderGroup;
    
    [Header("Tiempos")]
    public float duracionFade = 1.0f;
    public float esperaAlInicio = 1.5f; // Segundos en negro al cargar la escena
    public float esperaEnNegro = 0.5f;  // Segundos en negro antes de recargar

    private void Start()
    {
        // Al empezar, iniciamos la rutina que aguanta en negro y luego revela el juego
        if (faderGroup != null)
        {
            StartCoroutine(RutinaAclararConEspera());
        }
    }

    // Llama a esta función desde el botón "Reiniciar" de tu interfaz
    public void IniciarReinicioDeNivel()
    {
        StopAllCoroutines();
        StartCoroutine(RutinaFadeYRecargar());
    }

    private IEnumerator RutinaAclararConEspera()
    {
        // 1. Pantalla negra absoluta inmediata
        faderGroup.alpha = 1f;

        // 2. Esperamos en negro el tiempo configurado
        yield return new WaitForSeconds(esperaAlInicio);

        // 3. Hacemos el Fade Out (de negro a transparente)
        yield return StartCoroutine(RutinaFade(1f, 0f));
    }

    private IEnumerator RutinaFade(float inicioAlpha, float finAlpha)
    {
        float tiempoPasado = 0f;
        while (tiempoPasado < duracionFade)
        {
            tiempoPasado += Time.deltaTime;
            faderGroup.alpha = Mathf.Lerp(inicioAlpha, finAlpha, tiempoPasado / duracionFade);
            yield return null;
        }
        faderGroup.alpha = finAlpha;
    }

    private IEnumerator RutinaFadeYRecargar()
    {
        // 1. Hacemos el Fade In (hacia el negro total)
        yield return StartCoroutine(RutinaFade(faderGroup.alpha, 1f));

        // 2. Bloqueo de seguridad al 100% de negro
        faderGroup.alpha = 1f;

        // 3. Esperamos el tiempo de seguridad para ocultar tirones
        yield return new WaitForSeconds(esperaEnNegro);

        // 4. Recargamos la escena limpia
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}