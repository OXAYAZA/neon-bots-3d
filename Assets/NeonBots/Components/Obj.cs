using UnityEngine;

public class Obj : MonoBehaviour
{
    protected static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    protected static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public Color color = new(200, 200, 200);

    public string fraction;

    public GameObject body;

    public new Renderer renderer;

    public Rigidbody rigidBody;

    protected virtual void Start() { }

    protected virtual void SetColor()
    {
        this.renderer.material.SetColor(BaseColor, this.color);
        this.renderer.material.SetColor(EmissionColor, this.color);
    }
}
