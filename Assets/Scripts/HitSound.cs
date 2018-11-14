using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSound : MonoBehaviour {

	public AudioSource PuckReleaseSource;

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
		}
	}
}