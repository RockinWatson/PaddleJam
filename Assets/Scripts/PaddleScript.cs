using UnityEngine;

public class PaddleScript : MonoBehaviour {

    public Rigidbody2D RB;
    public float Speed;
    public float MaxX;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var xInput = Input.GetAxis("Horizontal");

        if (xInput == 0)
        {
            Stop();
        }
        if (xInput < 0)
        {
            MoveLeft();
        }
        if (xInput > 0)
        {
            MoveRight();
        }

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -MaxX, MaxX);
        transform.position = pos;
    }

    void MoveLeft() {
        RB.velocity = new Vector2(-Speed, 0);
    }

    void MoveRight(){
        RB.velocity = new Vector2(Speed,0);
    }

    void Stop() {
        RB.velocity = Vector2.zero;
    }
}
