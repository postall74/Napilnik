namespace Test
{
    public class Weapon
    {
        public int Damage { get; private set; }
        public int Bullets { get; private set; }

        public int Fire()
        {
            Bullets--;
            return Damage;
        }
    }

    public class Bot
    {
        private readonly Weapon Weapon;

        public void OnSeePlayer(Player player)
        {
            player.TakeDamage(Weapon.Fire());
        }
    }

    public class Player
    {
        private int _health;

        public int Health => _health;

        public int TakeDamage(int damage)
        {
            return _health -= damage;
        }
    }
}
