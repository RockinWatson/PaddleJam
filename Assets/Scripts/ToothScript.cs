using UnityEngine;

public class ToothScript : MonoBehaviour {

    public SpriteRenderer _spRend;
    public Sprite[] _sprites;
	public AudioSource PuckReleaseSource;

    private int _health = 3;

	// Use this for initialization
	void Start () {
		PuckReleaseSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {   
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "puck")
        {
            PuckReleaseSource.Play();
            _health -= 1;
            if (_health <= 0)
            {
                gameObject.SetActive(false);
            }
        }	
    }

    private void OnGUI()
    {
        if (_health == 2)
        {
            _spRend.sprite = _sprites[1];
        }
        if (_health == 1)
        {
            _spRend.sprite = _sprites[2];
        }
    }
}
