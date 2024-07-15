using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushScript : MonoBehaviour
{
    public float maxSize;

    // Start is called before the first frame update
    void Start()
    {
        // Start the bush as green
        GetComponent<Renderer>().material.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        // Increase the size of the bush
        Vector3 scaleChange = new Vector3(0.003f, 0.003f, 0.003f);
        if (transform.localScale.x <= maxSize) {
            transform.localScale += scaleChange;
        }
    }
}
