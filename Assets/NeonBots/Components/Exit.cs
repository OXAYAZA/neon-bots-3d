using System.Collections.Generic;
using System.Linq;
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

    private float charge;

    private List<Unit> units;

    private void Start()
    {
        this.renderer = this.gameObject.GetComponent<Renderer>();
        this.initialColor = this.renderer.material.GetColor(EmissionColor);
        this.units = new();
    }

    private void Update()
    {
        if(this.units.Count > 0 && this.units.FirstOrDefault(item => item.fraction == "green") != default)
            this.charge = this.charge < ExitCharge ? this.charge + Time.deltaTime : ExitCharge;
        else
            this.charge = this.charge > 0f ? this.charge - Time.deltaTime : 0f;

        this.renderer.material.SetColor(EmissionColor,
            Color.Lerp(this.initialColor, Color.white, this.charge / ExitCharge));

        if(this.charge < ExitCharge) return;

        this.ToNextLevel().Forget();
        this.charge = 0f;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.TryGetComponent<ObjectLink>(out var link) && link.target != default)
            this.units.Add((Unit)link.target);
    }

    private void OnTriggerExit(Collider collider)
    {
        if(collider.TryGetComponent<ObjectLink>(out var link) && link.target != default)
            this.units.Remove((Unit)link.target);
    }

    private async UniTask ToNextLevel() => await MainManager.LoadLevel();
}
