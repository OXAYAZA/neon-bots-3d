using UnityEngine;

public class KeyboardMouseController : MonoBehaviour
{
    private Unit unit;

    public void Init(Unit unit) => this.unit = unit;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape");
            //Root.Instance.PlayPauseGame();
        }

        if(this.unit)
        {
            // if(this.camera)
            // {
            //     this._cursorPos = this.cameraController.cursor;
            //
            //     if(this._cursorPos != this.camera.transform.position)
            //     {
            //         var direction = new Vector3(this._cursorPos.x - this.hero.transform.position.x, 0,
            //             this._cursorPos.z - this.hero.transform.position.z);
            //         this.hero.Rotate(direction);
            //     }
            // }

            if(Input.GetKey(KeyCode.A)) this.unit.Move(Vector3.left);

            if(Input.GetKey(KeyCode.D)) this.unit.Move(Vector3.right);

            if(Input.GetKey(KeyCode.S)) this.unit.Move(Vector3.back);

            if(Input.GetKey(KeyCode.W)) this.unit.Move(Vector3.forward);

            if(Input.GetKey(KeyCode.LeftArrow)) this.unit.Rotate(Vector3.left);

            if(Input.GetKey(KeyCode.RightArrow)) this.unit.Rotate(Vector3.right);

            if(Input.GetKey(KeyCode.UpArrow)) this.unit.Rotate(Vector3.forward);

            if(Input.GetKey(KeyCode.DownArrow)) this.unit.Rotate(Vector3.back);

            if(Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) this.unit.Shot();
        }
    }
}