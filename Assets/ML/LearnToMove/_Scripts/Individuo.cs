namespace AlgoritmoGenetico
{
    using UnityEngine;

    public enum MovementType
    {
        Forward,
        Left,
        Right,
        Count
    }



    public class Individuo
    {
        public MovementType[] chromosome;
        public float fitness;
        public float PorcentajeFitness;

        public Individuo(int chromosomeLength)
        {
            chromosome = new MovementType[chromosomeLength];
            fitness = 0;
            PorcentajeFitness = 0;

            for (int i = 0; i < chromosomeLength; i++)
            {
                chromosome[i] = GetRandomMovement();
            }

        }
        public MovementType GetRandomMovement()
        {
            return (MovementType)Random.Range(0, (int)MovementType.Count);
        }
        public void ShowCromosomas()
        {
            int number = chromosome.Length;

        }


    }
}