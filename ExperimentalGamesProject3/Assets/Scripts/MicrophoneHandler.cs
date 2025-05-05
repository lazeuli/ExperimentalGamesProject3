using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class MicrophoneHandler : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject originalObject;
    public GameObject replacementObject;
    public int sensitivity = 2;

    public float cooldownTime = 1f;
    public float fadeSpeed = 1f;

    private AudioClip micClip;
    private string micDevice;
    private float lastDetectedTime;
    private bool fadingBack = false;
    private Material originalMaterial;
    private Color originalColor;

    //Color ChangePostProcessing
    public PostProcessVolume postProcessVolume;
    public KeyCode toggleKey = KeyCode.P;
    public float saturatedValue = 0f;
    public float desaturatedValue = -100f;
    public float transitionSpeed = 2f;

    private ColorGrading colorGrading;
    private float targetSaturation;
    private bool isDesaturated = false;
    void Start()
    {
        //Color Post Processing
        if (postProcessVolume == null)
    {
        Debug.LogError("PostProcessVolume not assigned.");
        return;
    }

    if (postProcessVolume.profile.TryGetSettings(out colorGrading))
    {
        colorGrading.saturation.overrideState = true;
        StartCoroutine(ForceDesaturationOnStart());
    }
    else
    {
        Debug.LogError("Color Grading not found in Post Process Volume.");
    }
        //Color Post Processing

         if (Microphone.devices.Length == 0){
            Debug.LogError("No microphone found!");
            return;
        }

        micDevice = Microphone.devices[0];
        micClip = Microphone.Start(micDevice, true, 1, 44100);
        originalMaterial = originalObject.GetComponent<MeshRenderer>().material;
        originalColor = originalMaterial.color;


        if (originalObject != null && replacementObject != null)
        {
            replacementObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (micClip == null) return;

        float[] samples = new float[128];
        int micPos = Microphone.GetPosition(micDevice) - samples.Length + 1;
        if (micPos < 0) return;

        micClip.GetData(samples, micPos);

        float volume = 0f;
        foreach (float sample in samples)
        {
            volume += Mathf.Abs(sample);
        }

        Debug.Log("Mic volume: " + volume);

        if (volume > sensitivity)
        {
            lastDetectedTime = Time.time;
            fadingBack = false;

            originalObject.SetActive(false);
            replacementObject.SetActive(true);
            SetAlpha(originalMaterial, 1f);
            isDesaturated = false;
            targetSaturation = saturatedValue;
        }
        else if (Time.time - lastDetectedTime > cooldownTime && !fadingBack){
            fadingBack = true;
            StartCoroutine(FadeBack());
            isDesaturated = true;
            targetSaturation = desaturatedValue;
        }
        

        //color post processing
        if (Input.GetKey(toggleKey) && colorGrading != null)
        {
        targetSaturation = saturatedValue;
        }
        else if (colorGrading != null && !isDesaturated)
        {
        targetSaturation = desaturatedValue;
        }

        if (colorGrading != null)
        {
            colorGrading.saturation.value = Mathf.Lerp(
                colorGrading.saturation.value,
                targetSaturation,
                Time.deltaTime * transitionSpeed
            );
        }
        //color post processing
    }

    System.Collections.IEnumerator FadeBack()
    {
        replacementObject.SetActive(false);
        originalObject.SetActive(true);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            SetAlpha(originalMaterial, t);
            yield return null;
        }

        SetAlpha(originalMaterial, 1f);
        fadingBack = false;
    }

    void SetAlpha(Material mat, float alpha)
    {
        Color c = mat.color;
        c.a = Mathf.Clamp01(alpha);
        mat.color = c;
    }

   private void SetActiveObject(bool micDetected)
    {
        originalObject.SetActive(!micDetected);
        replacementObject.SetActive(micDetected);
    }
    IEnumerator ForceDesaturationOnStart()
    {
    yield return null; // wait 1 frame to let post processing initialize
    colorGrading.saturation.value = desaturatedValue;
    targetSaturation = desaturatedValue;
    isDesaturated = true;
    }
}
