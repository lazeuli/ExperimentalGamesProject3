using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ColorChange : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    public KeyCode toggleKey = KeyCode.P;
    public float saturatedValue = 0f;
    public float desaturatedValue = -100f;
    public float transitionSpeed = 2f; // Higher = faster transition

    private ColorGrading colorGrading;
    private float targetSaturation;
    private bool isDesaturated = false;

    void Start()
    {
        if (postProcessVolume == null)
        {
            Debug.LogError("PostProcessVolume not assigned.");
            return;
        }

        if (postProcessVolume.profile.TryGetSettings(out colorGrading))
        {
            colorGrading.saturation.value = saturatedValue;
            targetSaturation = saturatedValue;
        }
        else
        {
            Debug.LogError("Color Grading not found in Post Process Volume.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey) && colorGrading != null)
        {
            isDesaturated = !isDesaturated;
            targetSaturation = isDesaturated ? desaturatedValue : saturatedValue;
        }

        if (colorGrading != null)
        {
            colorGrading.saturation.value = Mathf.Lerp(
                colorGrading.saturation.value,
                targetSaturation,
                Time.deltaTime * transitionSpeed
            );
        }
    }
}