using UnityEngine;

public class PuckScript : MonoBehaviour {

    public Rigidbody2D RB;
    public float BallForce;

    private bool _gamestart = false;

	// Use this for initialization
	void Start () {
        	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Space) && _gamestart != true)
        {
            transform.SetParent(null);
            RB.isKinematic = false;

            RB.AddForce(new Vector2(BallForce, BallForce));
            _gamestart = true;
        }
    }
}
