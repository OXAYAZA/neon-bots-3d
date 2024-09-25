using UnityEngine;

public class RootLocal : MonoBehaviour
{
    public Root root;
    public GameObject map;
    public GameObject hero;

    public void Link ( Root root )
    {
        this.root = root;
    }
}
