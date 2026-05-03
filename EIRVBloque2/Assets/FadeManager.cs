using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Arrastra aquí el Canvas Group de tu Pantalla Negra")]
    public CanvasGroup faderGroup;
    
    [Tooltip("Tiempo en segundos que tarda en hacer el fundido")]
    public float duracionFade = 1.5f;

    private void Start()
    {
        // Al arrancar la escena, forzamos que la pantalla esté negra (alpha 1)
        if (faderGroup != null)
        {
            faderGroup.alpha = 1f;
            // Y automáticamente iniciamos el Fade Out (hacerla transparente)
            StartCoroutine(RutinaFade(1f, 0f));
        }
    }

    // Esta es la función que conectarás a tu botón de REINICIAR
    public void IniciarReinicioDeNivel()
    {
        StartCoroutine(RutinaFadeYRecargar());
    }

    // Corrutina general para animar el alpha
    private IEnumerator RutinaFade(float inicioAlpha, float finAlpha)
    {
        float tiempoPasado = 0f;

        while (tiempoPasado < duracionFade)
        {
            tiempoPasado += Time.deltaTime;
            // Interpola suavemente el valor entre inicio y fin
            faderGroup.alpha = Mathf.Lerp(inicioAlpha, finAlpha, tiempoPasado / duracionFade);
            yield return null;
        }

        faderGroup.alpha = finAlpha;
    }

    // Corrutina especial que hace el Fade a negro y LUEGO carga la escena
    private IEnumerator RutinaFadeYRecargar()
    {
        // 1. Hacemos Fade In (pantalla a negro)
        yield return StartCoroutine(RutinaFade(0f, 1f));

        // 2. Opcional: Pequeña pausa con la pantalla en negro para que no sea tan brusco
        yield return new WaitForSeconds(0.2f);

        // 3. Recargamos la escena actual
        string nombreEscenaActual = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(nombreEscenaActual);
    }
}