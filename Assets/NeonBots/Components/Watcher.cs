using UnityEngine;

public class Watcher : MonoBehaviour
{
    public GameObject target;

    public Vector3 offset = new(0, 20f, -20f);

    private void Update()
    {
        if(this.target == default) return;

        this.transform.position = this.target.transform.position + this.offset;
        this.transform.LookAt(this.target.transform);
    }
}
