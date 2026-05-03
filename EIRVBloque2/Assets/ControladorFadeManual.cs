using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class ControladorFadeManual : MonoBehaviour
{
    [Header("Referencias")]
    public CanvasGroup faderGroup;
    public float duracionFade = 1.0f;
    
    [Header("Ajustes de Tiempos")]
    public float esperaAlInicio = 1.5f; // Segundos que aguanta en negro al empezar
    public float esperaEnNegro = 0.5f;  // Segundos que aguanta en negro antes de recargar

    private Coroutine fadeActivo;

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.oKey.wasPressedThisFrame) FundirANegro();
            if (Keyboard.current.pKey.wasPressedThisFrame) FundirATransparente();
            if (Keyboard.current.rKey.wasPressedThisFrame) IniciarReinicioCompleto();
        }
    }

    // --- FUNCIONES MANUALES VISUALES ---

    public void FundirANegro()
    {
        if (faderGroup == null) return;
        if (fadeActivo != null) StopCoroutine(fadeActivo);
        fadeActivo = StartCoroutine(RutinaFade(faderGroup.alpha, 1f));
    }

    public void FundirATransparente()
    {
        if (faderGroup == null) return;
        if (fadeActivo != null) StopCoroutine(fadeActivo);
        fadeActivo = StartCoroutine(RutinaFade(faderGroup.alpha, 0f));
    }

    // --- FUNCIÓN DE REINICIO DE NIVEL ---

    public void IniciarReinicioCompleto()
    {
        if (faderGroup == null) return;
        if (fadeActivo != null) StopCoroutine(fadeActivo);
        StartCoroutine(RutinaFadeYRecargar());
    }

    // --- CORRUTINAS INTERNAS ---

    // NUEVA: Aguanta en negro al empezar y luego aclara
    private IEnumerator RutinaAclararConEspera()
    {
        // 1. Forzamos la pantalla a negro absoluto desde el primer milisegundo
        faderGroup.alpha = 1f;

        // 2. Aguantamos el tiempo que hayas puesto en el Inspector
        yield return new WaitForSeconds(esperaAlInicio);

        // 3. Empezamos a aclarar suavemente
        FundirATransparente();
    }

    private IEnumerator RutinaFade(float inicioAlpha, float finAlpha)
    {
        float tiempoPasado = 0f;
        float duracionAjustada = duracionFade * Mathf.Abs(finAlpha - inicioAlpha);

        while (tiempoPasado < duracionAjustada)
        {
            tiempoPasado += Time.deltaTime;
            faderGroup.alpha = Mathf.Lerp(inicioAlpha, finAlpha, tiempoPasado / duracionAjustada);
            yield return null;
        }

        faderGroup.alpha = finAlpha;
        fadeActivo = null;
    }

    private IEnumerator RutinaFadeYRecargar()
    {
        yield return StartCoroutine(RutinaFade(faderGroup.alpha, 1f));
        faderGroup.alpha = 1f;
        yield return new WaitForSeconds(esperaEnNegro);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}