using System.Collections.Generic;
using FuzzyLogic;
using UnityEngine;


    
    
public class FightFuzzyLogic : MonoBehaviour
{
  private FuzzySystem fightSystem;

    void Start()
    {
        // Initialize Fuzzy System
        fightSystem = new FuzzySystem("Fight System");

        // Define Health variable
        FuzzyVariable health = new FuzzyVariable("Health");
        health.AddFuzzySet(new TriangularFuzzySet("Low", 0, 0, 50));
        health.AddFuzzySet(new TriangularFuzzySet("Medium", 30, 50, 70));
        health.AddFuzzySet(new TriangularFuzzySet("High", 50, 100, 100));
        fightSystem.AddIndependientVariable(health);

        // Define Enemy Proximity variable
        FuzzyVariable proximity = new FuzzyVariable("Proximity");
        proximity.AddFuzzySet(new TriangularFuzzySet("Close", 0, 0, 30));
        proximity.AddFuzzySet(new TriangularFuzzySet("Medium", 20, 50, 80));
        proximity.AddFuzzySet(new TriangularFuzzySet("Far", 70, 100, 100));
        fightSystem.AddIndependientVariable(proximity);

        // Define Aggressiveness variable
        FuzzyVariable aggressiveness = new FuzzyVariable("Aggressiveness");
        aggressiveness.AddFuzzySet(new TriangularFuzzySet("Low", 0, 0, 50));
        aggressiveness.AddFuzzySet(new TriangularFuzzySet("Medium", 30, 50, 70));
        aggressiveness.AddFuzzySet(new TriangularFuzzySet("High", 50, 100, 100));
        fightSystem.AddDependientVariable(aggressiveness);

        // Define rules
        DefineRules();

        // Test the system with input values
        TestFuzzySystem(45, 25);
    }

    void DefineRules()
    {
        // If health is low and enemy proximity is close, then aggressiveness is high
        Dictionary<string, string> antecedents1 = new Dictionary<string, string>
        {
            { "Health", "Low" },
            { "Proximity", "Close" }
        };
        Rule rule1 = new Rule(antecedents1, new KeyValuePair<string, string>("Aggressiveness", "High"));
        fightSystem.AddRule(rule1);

        // If health is medium and enemy proximity is medium, then aggressiveness is medium
        Dictionary<string, string> antecedents2 = new Dictionary<string, string>
        {
            { "Health", "Medium" },
            { "Proximity", "Medium" }
        };
        Rule rule2 = new Rule(antecedents2, new KeyValuePair<string, string>("Aggressiveness", "Medium"));
        fightSystem.AddRule(rule2);

        // If health is high and enemy proximity is far, then aggressiveness is low
        Dictionary<string, string> antecedents3 = new Dictionary<string, string>
        {
            { "Health", "High" },
            { "Proximity", "Far" }
        };
        Rule rule3 = new Rule(antecedents3, new KeyValuePair<string, string>("Aggressiveness", "Low"));
        fightSystem.AddRule(rule3);
    }

    void TestFuzzySystem(float healthValue, float proximityValue)
    {
        // Input values
        Dictionary<string, float> inputs = new Dictionary<string, float>
        {
            { "Health", healthValue },
            { "Proximity", proximityValue }
        };

        // Get output values
        Dictionary<string, float> outputs = fightSystem.FuzzyInferenceSystem(inputs, "min", "last_of_maxima");

        // Display the results
        foreach (var output in outputs)
        {
            Debug.Log($"{output.Key}: {output.Value}");
        }
    }
}




