using System;
using UnityEngine;

public class BotTurret : MonoBehaviour
{
    [SerializeField]
    private float acceleration = 500f;

    [SerializeField]
    private float torque = 50f;

    private Rigidbody _rigidbody;
    private Unit _unit;
    private Gun _gun;
    private Vector3 _movementDirection;
    private Vector3 _rotatingDirection;
    private GameObject _target;
    private Vector3 _direction;
    private double _distance;
    private double _angle;

    private void Start()
    {
        this._rigidbody = this.GetComponent<Rigidbody>();
        this._gun = this.GetComponent<Gun>();
        this._unit = this.GetComponent<Unit>();
    }

    private void Update()
    {
        this._movementDirection = Vector3.zero;
        this._rotatingDirection = Vector3.zero;

        this.Scan();

        if ( this._target )
        {
            this.Calculate();
            this.Rotate();
            this.Attack();
        }
    }
    
    private void FixedUpdate()
    {
        this._rigidbody.AddForce( this._movementDirection * this.acceleration, ForceMode.Acceleration );
        this._rigidbody.AddTorque( this._rotatingDirection * this.torque, ForceMode.Acceleration );
    }

    private void Scan ()
    {
        this._target = null;
        this._distance = Double.PositiveInfinity;
        
        var objects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach ( var obj in objects )
        {
            var unit = obj.GetComponent<Unit>();
            if ( unit && obj != this.gameObject && unit.fraction != this._unit.fraction )
            {
                var trs = obj.GetComponent<Transform>();
                var distance = Math.Sqrt( Math.Pow( trs.position.x - this.transform.position.x, 2 ) + Math.Pow( trs.position.z - this.transform.position.z, 2 ) );

                if ( distance < this._distance )
                {
                    this._distance = distance;
                    this._target = obj;
                }
            }
        }
    }

    private void Calculate ()
    {
        this._direction = new Vector3( this._target.transform.position.x - this.transform.position.x, 0, this._target.transform.position.z - this.transform.position.z );
        this._angle = Vector3.SignedAngle( this.transform.forward, this._direction, this.transform.up );
        //Debug.Log( this.gameObject + " :: " + this._target + " :: " + this._angle );
    }

    private void Rotate ()
    {
        if ( this._angle < 0 )
            this._rotatingDirection -= this.transform.up;
        else if ( this._angle > 0 )
            this._rotatingDirection += this.transform.up;
    }

    private void Move ()
    {
        if ( this._distance > 10 )
            this._movementDirection += this.transform.forward;
        else if ( this._distance < 8 )
            this._movementDirection -= this.transform.forward;
        else
            this._movementDirection -= this.transform.right;
    }

    private void Attack ()
    {
        if ( this._distance < 20 && this._gun )
            this._gun.Shot();
            
    }
}
