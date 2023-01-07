
public abstract class EnemyFactory : GameObjectFactory
{
    public Enemy Get(EnemyType type)
    {
        EnemyConfig config = GetConfig(type);
        Enemy instance = CreateGameObjectInstance(config.Prefab);
        instance.OriginFactory = this;
        instance.Initialize(config.Scale.RandomValueInRange, config.PathOffset.RandomValueInRange,
            config.Speed.RandomValueInRange, config.Health.RandomValueInRange, config.KillReward);
        return instance;
    }

    protected abstract EnemyConfig GetConfig(EnemyType type);
    
    public void Reclaim(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }
}
