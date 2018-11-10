using UnityEngine;

public class PuckScript : MonoBehaviour {

    public Rigidbody2D RB;
    public float BallForce;

	// Use this for initialization
	void Start () {
        RB.AddForce(new Vector2(BallForce, BallForce));
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
