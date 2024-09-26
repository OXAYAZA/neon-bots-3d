using UnityEngine;

public class Watcher : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    [SerializeField]
    private Vector3 offset;

    private void Update()
    {
        if(this.target) this.transform.position = this.target.transform.position + this.offset;
    }
}
