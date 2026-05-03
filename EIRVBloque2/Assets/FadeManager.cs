using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public CanvasGroup faderGroup;
    public float duracionFade = 1.0f;
    public float esperaEnNegro = 0.5f; // El tiempo extra que pediste

    private void Start()
    {
        // Al empezar, siempre hacemos que aparezca desde negro
        if (faderGroup != null)
        {
            faderGroup.alpha = 1f;
            StartCoroutine(RutinaFade(1f, 0f));
        }
    }

    public void IniciarReinicioDeNivel()
    {
        // Detenemos cualquier fade que se esté ejecutando para que no haya conflictos
        StopAllCoroutines();
        StartCoroutine(RutinaFadeYRecargar());
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
        // 1. Fade a negro
        yield return StartCoroutine(RutinaFade(faderGroup.alpha, 1f));

        // 2. BLOQUEO DE SEGURIDAD: Forzamos el alfa a 1 
        faderGroup.alpha = 1f;

        // 3. ESPERA EXTRA: Lo que necesitabas para que el ojo no note el salto
        yield return new WaitForSeconds(esperaEnNegro);

        // 4. CARGA: Recargamos la escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}