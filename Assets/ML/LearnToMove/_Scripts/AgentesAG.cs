using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlgoritmoGenetico;
public class AgentesAG : MonoBehaviour
{

    public Individuo individuo;
    float distanceFitnnes = 0;

    int position = 0;

    void Start()
    {
        StartCoroutine(MoveTransform(individuo.chromosome));
    }

    IEnumerator MoveTransform(MovementType[] chromosome)
    {
        while (chromosome.Length != position)
        {
            switch (chromosome[position])
            {
                case MovementType.Forward:
                    transform.Translate(Vector3.forward);
                    break;
                case MovementType.Left:
                    transform.Translate(Vector3.left);
                    break;
                case MovementType.Right:
                    transform.Translate(Vector3.right); ;
                    break;
            }

            yield return new WaitForSeconds(Random.Range(0.015f, 0.025f));
            distanceFitnnes = distanceFitnnes + 1;


            Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Obstacle"))
                {

                    individuo.fitness = distanceFitnnes - 10;
                    Genetic_Algorithm.population.Add(individuo);
                    Destroy(this.gameObject);
                    break;
                }
            }

            position++;
        }
        individuo.fitness = distanceFitnnes;
        Genetic_Algorithm.population.Add(individuo);

        Destroy(this.gameObject);
        // this.gameObject.SetActive(false);
    }



}
