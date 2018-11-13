using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiggle : MonoBehaviour {

    public Vector2 moveToWhenHit;
    private bool _isHit = false;

    public AnimationCurve curve;
    public Vector3 distance;
    public float speed;

    private Vector3 startPos, toPos;
    private float timeStart;

    void randomToPos()
    {
        toPos = startPos;
        toPos.x += Random.Range(-1.0f, +1.0f) * distance.x;
        toPos.y += Random.Range(-1.0f, +1.0f) * distance.y;
        toPos.z += Random.Range(-1.0f, +1.0f) * distance.z;
        timeStart = Time.time;
    }

    // Use this for initialization
    void Start()
    {
        startPos = transform.position;
        randomToPos();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isHit)
        {
            float d = (Time.time - timeStart) / speed, m = curve.Evaluate(d);
            if (d > 1)
            {
                randomToPos();
            }
            else if (d < 0.5)
            {
                transform.position = Vector3.Lerp(startPos, toPos, m * 2.0f);
            }
            else
            {
                transform.position = Vector3.Lerp(toPos, startPos, (m - 0.5f) * 2.0f);
            }
        }

        if (_isHit)
        {
            //Move off of screen
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), moveToWhenHit, 3 * Time.deltaTime);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "puck")
        {
            _isHit = true;
        }
    }
}
