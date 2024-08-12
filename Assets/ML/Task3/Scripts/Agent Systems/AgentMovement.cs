namespace FuzzyLogicAndGeneticAlgorithm
{

    public class AgentMovement
    {
        public float distanceTraveled;

        public AgentMovement()
        {
            distanceTraveled = 0f;
        }

        public void Move(float distance)
        {
            distanceTraveled += distance;
        }
    }
}
