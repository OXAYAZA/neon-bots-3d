using UnityEngine;
using UnityEngine.UI;

public class FpsMeter : MonoBehaviour
{
    private Text fpsText;
    private float deltaTime;

    void Start ()
    {
        this.fpsText = this.gameObject.GetComponent<Text>();
    }

    void Update ()
    {
        this.deltaTime += ( Time.deltaTime - this.deltaTime ) * 0.1f;
        float fps = 1.0f / this.deltaTime;
        this.fpsText.text = Mathf.Ceil ( fps ).ToString();
    }
}
