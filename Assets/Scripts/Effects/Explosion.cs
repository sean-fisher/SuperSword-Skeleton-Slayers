using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    public static bool isExploding = false;

	// Use this for initialization
	void Start () {
        Explode();
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            //Explode();
        }
    }

    public GameObject bubble1;
    public GameObject smoke1;

    Vector3 ne = new Vector2(.2f, .1f);
    Vector3 se = new Vector2(.2f, -.2f);
    Vector3 nw = new Vector2(-.1f, .1f);
    Vector3 sw = new Vector2(-.1f, -.2f);

    private IEnumerator ExplodeAnimate()
    {
        isExploding = true;
        GameObject b1 = GameObject.Instantiate(bubble1, transform);
        GameObject s1 = GameObject.Instantiate(smoke1, transform);
        GameObject b2 = GameObject.Instantiate(bubble1, transform);
        b2.transform.localScale = new Vector2(-b2.transform.localScale.x, b2.transform.localScale.y);
        GameObject s2 = GameObject.Instantiate(smoke1, transform);
        GameObject s3 = GameObject.Instantiate(smoke1, transform);
        /*
        b1.transform.position = transform.position;
        s1.transform.position = transform.position;
        b2.transform.position = transform.position;
        s2.transform.position = transform.position;
        s3.transform.position = transform.position;*/

        for (int i = 1; i < 70; i++)
        {
            float c = (float) 50f / i;
            b1.transform.position += ne * c;
            s1.transform.position += se * c;
            b2.transform.position += sw * c;
            s2.transform.position += sw * c;
            s3.transform.position += nw * c;
            yield return null;
        }
        Destroy(b1);
        Destroy(s1);
        Destroy(b2);
        Destroy(s2);
        Destroy(s3);
        Destroy(this.gameObject);
        isExploding = false;
    }

    void Explode()
    {
        StartCoroutine(ExplodeAnimate());
    }
}
