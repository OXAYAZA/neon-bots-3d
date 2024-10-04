using Cysharp.Threading.Tasks;
using NeonBots.Components;
using NeonBots.Managers;
using UnityEngine;

public class Exit : MonoBehaviour
{
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private const float ExitCharge = 5f;

    [SerializeField]
    private string destinationScene;

    private new Renderer renderer;

    private Color initialColor;

    private bool active;

    private float charge;

    private void Start()
    {
        this.renderer = this.gameObject.GetComponent<Renderer>();
        this.initialColor = this.renderer.material.GetColor(EmissionColor);
    }

    private void Update()
    {
        if(this.active)
            this.charge = this.charge < ExitCharge ? this.charge + Time.deltaTime : ExitCharge;
        else
            this.charge = this.charge > 0 ? this.charge - Time.deltaTime : 0;

        this.renderer.material.SetColor(EmissionColor,
            Color.Lerp(this.initialColor, Color.white, this.charge / ExitCharge));

        if(this.charge >= ExitCharge) this.ToNextLevel().Forget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<ObjectLink>(out var link) &&
           link.target != default && ((Unit)link.target).fraction == "green") this.active = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<ObjectLink>(out var link) &&
           link.target != default && ((Unit)link.target).fraction == "green") this.active = false;
    }

    private async UniTask ToNextLevel()
    {
        var storage = MainManager.GetManager<ObjectStorage>();

        if(!storage.TryGet<GameObject>("hero", out var hero))
            throw new("No hero when exiting from level");

        hero.SetActive(false);
        await MainManager.UnloadScene();
        var sceneData = await MainManager.LoadScene("Level-1");
        hero.transform.position = sceneData.heroSpawn.position;
        hero.SetActive(true);
    }
}
