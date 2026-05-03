using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabObjectEffects : MonoBehaviour
{
    [Header("Material overlay al coger")]
    [SerializeField] private Material overlayMaterial;

    private XRGrabInteractable grabInteractable;
    private Collider[] colliders;
    private Renderer[] renderers;

    private Material[][] originalMaterials;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        colliders = GetComponentsInChildren<Collider>();
        renderers = GetComponentsInChildren<Renderer>();

        originalMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].materials;
        }
    }

    private void OnEnable()
    {
        if (grabInteractable == null) return;

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        if (grabInteractable == null) return;

        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        SetTriggerState(true);
        AddOverlayMaterial();
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        SetTriggerState(false);
        RestoreOriginalMaterials();
    }

    private void SetTriggerState(bool isTrigger)
    {
        foreach (Collider col in colliders)
        {
            col.isTrigger = isTrigger;
        }
    }

    private void AddOverlayMaterial()
    {
        if (overlayMaterial == null) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] current = renderers[i].materials;
            Material[] extended = new Material[current.Length + 1];

            for (int j = 0; j < current.Length; j++)
            {
                extended[j] = current[j];
            }

            extended[current.Length] = overlayMaterial;

            renderers[i].materials = extended;
        }
    }

    private void RestoreOriginalMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = originalMaterials[i];
        }
    }
}