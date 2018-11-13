using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {

    private bool _space() { return Input.GetKeyDown(KeyCode.Space); }
    private string level = "JTest";

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {

        if (_space())
        {
            SceneManager.LoadScene(level);
        }	
	}
}
