using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {

	private bool _return() { return Input.GetKeyDown(KeyCode.Return); }
    private string level = "JTest";

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {

        if (_return())
        {
            SceneManager.LoadScene(level);
        }	
	}
}
