using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    public Dictionary<(bool left, bool forward, bool right), float> genes; // bool - wall in front, float - angle to turn 

    int dnaLength;

    public DNA()
    {
        genes = new Dictionary<(bool left, bool forward, bool right), float>();
        SetRandom();
    }

    void SetRandom()
    {
        genes.Clear();
        genes.Add((false,false,false), Random.Range(-90, 91));
        genes.Add((false, false, true), Random.Range(-90, 91));
        genes.Add((false, true, false), Random.Range(-90, 91));
        genes.Add((false, true, true), Random.Range(-90, 91));
        genes.Add((true, false, false), Random.Range(-90, 91));
        genes.Add((true, false, true), Random.Range(-90, 91));
        genes.Add((true, true, false), Random.Range(-90, 91));
        genes.Add((true, true, true), Random.Range(-90, 91));

        dnaLength = genes.Count;
    }

    public void CombineDNA(DNA d1,  DNA d2)
    {
        int i = 0;
        Dictionary<(bool left, bool forward, bool right), float> newGenes = new Dictionary<(bool left, bool forward, bool right), float>();

        foreach(var gene in genes)
        {
            if(i < dnaLength/2)
                newGenes.Add(gene.Key, d1.genes[gene.Key]);
            else
                newGenes.Add(gene.Key, d2.genes[gene.Key]);
            i++;
        }
        genes = newGenes;
    }

    public float GetGene((bool left, bool forward, bool right) seeWall)
    {
        return genes[seeWall];
    }
}
