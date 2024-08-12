namespace FuzzyLogicAndGeneticAlgorithm
{

    public class HealthPackCollector
    {
        public int healthPacksCollected;

        public HealthPackCollector()
        {
            healthPacksCollected = 0;
        }

        public void CollectHealthPack()
        {
            healthPacksCollected++;
        }
    }
}
