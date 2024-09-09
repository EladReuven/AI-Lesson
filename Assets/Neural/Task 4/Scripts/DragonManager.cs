using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonManager : MonoBehaviour
{
    public static DragonManager Instance;

    public List<DragonController> dragons = new List<DragonController>();
    public float generationTime = 20f;
    public float topPercentFromList = 0.2f; // top 20 percent

    float timer = 0f;
    int generation = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (CheckIfAllDragonsDead() || timer > generationTime)
        {
            MakeNewGeneration();
        }
    }

    private void MakeNewGeneration()
    {
        if (timer > generationTime) print("Timer ran out");

        print("Starting New Generation");

        //if any dragons remain, kill them.
        KillAllRemainingDragons();
        //sort list of dragons by fitness
        SortDragonsByFitness();

        //make new temp list of top percent of dragons
        int amountOfTopDragons = Mathf.FloorToInt((float)dragons.Count * topPercentFromList);
        List<DragonController> topDragons = new List<DragonController>();
        for (int i = 0; i < amountOfTopDragons; i++)
            topDragons.Add(dragons[i]);

        //save the top dragons' layers
        List<NeuralNetWithCopyMono.Layer[]> topPerformingLayers = new();
        foreach (var d in topDragons)
            topPerformingLayers.Add(d.NN.layers);

        //destroy and clear existing dragons
        DragonController[] oldDragons = dragons.ToArray();
        foreach (var d in oldDragons)
            Destroy(d.gameObject);
        dragons.Clear();

        //create new dragons from the top performing dragons
        EnvironmentManager.Instance.SpawnNewGeneration(topPerformingLayers);

        //CHECK HOW TO SAVE NEURALNETWORK WEIGHTS AND BIASES.

        //reset/configure generation parameters
        timer = 0f;
        generation++;
    }

    bool CheckIfAllDragonsDead()
    {
        foreach (var dragon in dragons)
        {
            if (dragon.Dead == false)
            {
                return false;
            }
        }
        print("all dragons dead");
        return true;
    }

    void KillAllRemainingDragons()
    {
        foreach (var dragon in dragons)
        {
            if (!dragon.Dead)
                dragon.Die();
        }
    }

    void SortDragonsByFitness()
    {
        dragons.Sort((d1, d2) => d2.Fitness.CompareTo(d1.Fitness));
        print("Highest fitness: " + dragons[0].Fitness);
    }


}
