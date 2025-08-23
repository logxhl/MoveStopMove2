public static class Params
{
    public const string IsIdle = "IsIdle";
    public const string IsDead = "IsDead";
    public const string IsAttack = "IsAttack";
    public const string IsWin = "IsWin";
    public const string IsDance = "IsDance";
    public const string IsUlti = "IsUlti";
    public const string BotTag = "Bot";
    public const string PlayerTag = "Player";
    public const string GiftTag = "Gift";
    public const string WallTag = "Wall";
    public const string IsWalk = "IsWalk";
}

public enum PlayerState
{
    Idle,
    Run,
    Attack,
    Die,
    Dance,
    Win
}

public enum EnemyState
{
    Run,     
    Idle,      
    Attack,    
    Dead      
}

public enum SpawnState
{
    Idle,      
    Spawned    
}

public enum ZombieState
{
    Walk,
    Run,
    Victory
}

public enum SkillType
{
    None,
    DoubleThrow,
    Triple,
    UpScale,
    MoveFaster
}