using UnityEngine;

public class Obj : MonoBehaviour
{
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    public Color color = new(200, 200, 200);

    public string fraction;

    public GameObject body;

    public new Renderer renderer;

    public Rigidbody rigidBody;

    protected virtual void Start()
    {
        // this.data = this.GetComponent<ObjectData>();
        // this.body = this.transform.Find( "Body" ).gameObject;
        // this.renderer = this.body.GetComponent<Renderer>();
        // this.rigidBody = this.gameObject.GetComponent<Rigidbody>();

        this.renderer.material.SetColor(BaseColor, this.color);
    }
}
