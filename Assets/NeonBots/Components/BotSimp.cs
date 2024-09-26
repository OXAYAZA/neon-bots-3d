using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BotSimp : MonoBehaviour
{
    private Unit unit;

    private GameObject target;

    private Vector3 direction;

    private double distance;

    private void Start()
    {
        this.unit = this.GetComponent<Unit>();
    }

    private void Update()
    {
        this.Scan();

        if(this.target)
        {
            this.Calculate();
            this.Rotate();
            this.Move();
            this.Attack();
        }
    }

    private void Scan()
    {
        this.target = null;
        this.distance = Double.PositiveInfinity;

        var objects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach(var go in objects)
        {
            var obj = go.GetComponent<Unit>();

            if(go != this.gameObject && obj && obj.fraction != this.unit.fraction)
            {
                var trs = go.GetComponent<Transform>();
                var distance = Math.Sqrt(Math.Pow(trs.position.x - this.transform.position.x, 2) +
                    Math.Pow(trs.position.z - this.transform.position.z, 2));

                if(distance < this.distance)
                {
                    this.distance = distance;
                    this.target = go;
                }
            }
        }
    }

    private void Calculate()
    {
        this.direction = new(
            this.target.transform.position.x - this.transform.position.x,
            0,
            this.target.transform.position.z - this.transform.position.z
        );
    }

    private void Rotate()
    {
        this.unit.Rotate(this.direction);
    }

    private void Move()
    {
        if(this.distance > 10) this.unit.Move(this.transform.forward);
        else if(this.distance < 8) this.unit.Move(-this.transform.forward);
        else this.unit.Move(-this.transform.right);
    }

    private void Attack()
    {
        if(this.distance < 12) this.unit.Shot();
    }
}
