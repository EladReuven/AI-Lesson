using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
//using Layer = NeuralNetWithCopyMono.Layer;

public class DragonManager : MonoBehaviour
{
    public static DragonManager Instance;


    public List<DragonController> dragons = new List<DragonController>();
    public float generationTime = 20f;
    public float topPercentFromList = 0.2f; // top 20 percent

    float timer = 0f;
    int generation = 1;

    public string savesFolderPath { get; private set; }

    

    private void Awake()
    {
        Instance = this;
        //CreateSavesFolder();
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
        List<Layer[]> topPerformingLayers = new();
        foreach (var d in topDragons)
            topPerformingLayers.Add(d.NN.layers);

        //save top performers
        //SaveNetworks(topPerformingLayers, savesFolderPath);

        //destroy and clear existing dragons
        DragonController[] oldDragons = dragons.ToArray();
        foreach (var d in oldDragons)
            Destroy(d.gameObject);
        dragons.Clear();

        //create new dragons from the top performing dragons
        EnvironmentManager.Instance.SpawnNewGeneration(topPerformingLayers);

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


    #region SavingNeuralNetwork_DOES-NOT-WORK

    private void CreateSavesFolder()
    {
        // Get the "Assets" folder path and go one folder up
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;

        // Define the "Saves" folder path
        savesFolderPath = Path.Combine(projectRoot, "Saves");

        // Check if the "Saves" folder exists, if not, create it
        if (!Directory.Exists(savesFolderPath))
        {
            Directory.CreateDirectory(savesFolderPath);
            Debug.Log("Created Saves folder at: " + savesFolderPath);
        }
    }

    public void SaveNetworks(List<Layer[]> bestNetworks, string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream file = File.Create(filePath))
        {
            formatter.Serialize(file, bestNetworks);
        }
    }

    public List<Layer[]> LoadNetworks(string filePath)
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream file = File.Open(filePath, FileMode.Open))
            {
                return (List<Layer[]>)formatter.Deserialize(file);
            }
        }
        else
        {
            Debug.LogError("No save file found at " + filePath);
            return null;
        }
    }

    #endregion SavingNeuralNetwork_DOES-NOT-WORK

}
