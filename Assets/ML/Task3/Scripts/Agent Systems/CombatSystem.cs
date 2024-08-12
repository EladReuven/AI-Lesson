namespace FuzzyLogicAndGeneticAlgorithm
{

    public class CombatSystem
    {
        public int enemiesDefeated;

        public CombatSystem()
        {
            enemiesDefeated = 0;
        }

        public void DefeatEnemy()
        {
            enemiesDefeated++;
        }
    }

}