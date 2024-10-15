using System.Collections.Generic;
using UnityEngine;

public class Obj : MonoBehaviour
{
    protected static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    protected static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    [Header("Obj")]
    public Color color = new(200, 200, 200);

    public string fraction;

    public Rigidbody rigidBody;

    public SpriteRenderer mapFigure;

    public List<Renderer> renderers;

    public List<Collider> colliders;

    protected virtual void Start() { }

    protected virtual void SetColor(Color color)
    {
        if(this.mapFigure != default) this.mapFigure.color = color;

        foreach(var renderer in this.renderers)
        {
            renderer.material.SetColor(BaseColor, color);
            renderer.material.SetColor(EmissionColor, color);
        }
    }

    protected virtual void SetColor(Color baseColor, Color emissionColor)
    {
        if(this.mapFigure != default) this.mapFigure.color = baseColor;

        foreach(var renderer in this.renderers)
        {
            renderer.material.SetColor(BaseColor, baseColor);
            renderer.material.SetColor(EmissionColor, emissionColor);
        }
    }
}
