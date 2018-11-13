using UnityEngine;

public class ArmScript : MonoBehaviour {

    public Vector2 aPosition1 = new Vector2(9, 2);

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "puck")
        {
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), aPosition1, 3 * Time.deltaTime);
        }
    }
}
