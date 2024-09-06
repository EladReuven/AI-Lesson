using System;

public class NeuralNetwork
{
    private int[] layers; // layers
    private float[][] neurons; // neuron matix
    private float[][][] weights; // weight matrix

    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }
        InitNeurons();
        InitWeights();
    }

    private void InitNeurons()
    {
        neurons = new float[layers.Length][];
        for (int i = 0; i < layers.Length; i++)
        {
            neurons[i] = new float[layers[i]];
        }
    }

    private void InitWeights()
    {
        weights = new float[layers.Length - 1][][];
        for (int i = 0; i < layers.Length - 1; i++)
        {
            weights[i] = new float[neurons[i].Length][];
            for (int j = 0; j < neurons[i].Length; j++)
            {
                weights[i][j] = new float[neurons[i + 1].Length];
                for (int k = 0; k < neurons[i + 1].Length; k++)
                {
                    weights[i][j][k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }
            }
        }
    }

    public float[] FeedForward(float[] inputs)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f;
                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    value += weights[i - 1][k][j] * neurons[i - 1][k];
                }
                neurons[i][j] = (float)Math.Tanh(value);
            }
        }

        return neurons[neurons.Length - 1];
    }

    public void Mutate(float mutationRate)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    if (UnityEngine.Random.Range(0f, 1f) < mutationRate)
                    {
                        weights[i][j][k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                }
            }
        }
    }
}
