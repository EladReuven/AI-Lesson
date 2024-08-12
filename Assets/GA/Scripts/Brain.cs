using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public DNA dna;
    public GameObject eyes;
    public LayerMask ignore = 15;
    public float checkRange = 1f;
    public float eggsFound = 0f;

    List<int> eggIDs = new List<int>();
    (bool left, bool forward, bool right) seeWall;
    bool canMove = false;



    void Update()
    {
        seeWall = (false, false, false);
        bool left = false, forward = false, right = false;
        canMove = true;
        RaycastHit hit;
        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * checkRange, Color.red);
        Debug.DrawRay(eyes.transform.position, eyes.transform.right * checkRange, Color.red); ;
        Debug.DrawRay(eyes.transform.position, -eyes.transform.right * checkRange, Color.red);

        if (Physics.SphereCast(eyes.transform.position, 0.1f, eyes.transform.forward, out hit, checkRange, ~ignore))
        {
            if (hit.collider.gameObject.CompareTag("wall"))
            {
                forward = true;
                canMove = false;
            }
        }
        if (Physics.SphereCast(eyes.transform.position, 0.1f, eyes.transform.right, out hit, checkRange, ~ignore))
        {
            if (hit.collider.gameObject.CompareTag("wall"))
            {
                right = true;
            }
        }
        if (Physics.SphereCast(eyes.transform.position, 0.1f, -eyes.transform.right, out hit, checkRange, ~ignore))
        {
            if (hit.collider.gameObject.CompareTag("wall"))
            {
                left = true;
            }
        }

        seeWall = (left, forward, right);
    }


    void FixedUpdate()
    {
        this.transform.Rotate(0, dna.genes[seeWall], 0);
        if (canMove)
            this.transform.Translate(0, 0, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("egg"))
            return;
        Egg egg = other.GetComponent<Egg>();
        if (eggIDs.Contains(egg.ID))
            return;

        eggIDs.Add(egg.ID);
        eggsFound++;
        
        //other.gameObject.SetActive(false);
    }

    public void Init()
    {
        eggIDs.Clear();
        dna = new DNA();
    }
}

