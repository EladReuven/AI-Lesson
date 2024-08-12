namespace FuzzyLogicAndGeneticAlgorithm
{

    public class Health
    {
        public float currentHealth;
        public float maxHealth;

        public Health(float maxHealth = 100f)
        {
            this.maxHealth = maxHealth;
            this.currentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth < 0) currentHealth = 0;
        }

        public void Heal(float amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
        }
    }
}
