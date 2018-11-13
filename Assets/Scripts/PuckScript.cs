using UnityEngine;

public class PuckScript : MonoBehaviour {

    public Rigidbody2D RB;
    public float BallForce;
	public AudioClip PuckRelease;

    private bool _gamestart = false;
	private AudioSource source;
	private float volLowRange = .5f;
	private float volHighRange = 1.0f;

	// Use this for initialization
	void Start () {       	
		source = GetComponent<AudioSource>();
	}


	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Space) && _gamestart != true)
        {
            transform.SetParent(null);
            RB.isKinematic = false;

            RB.AddForce(new Vector2(BallForce, BallForce));
            _gamestart = true;
       
			float vol = Random.Range (volLowRange, volHighRange);
			source.PlayOneShot(PuckRelease,vol);

		}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
