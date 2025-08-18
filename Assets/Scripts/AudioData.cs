using UnityEngine;

namespace andywiecko.HeroesMusicManager
{
    [CreateAssetMenu(fileName = "AudioData", menuName = "HeroesMusicManager/Audio Data", order = 1)]
    public class AudioData : ScriptableObject
    {
        [field: Header("Town audio clips")]
        [field: SerializeField] public AudioClip CastleClip { get; private set; } = default;
        [field: SerializeField] public AudioClip RampartClip { get; private set; } = default;
        [field: SerializeField] public AudioClip TowerClip { get; private set; } = default;
        [field: SerializeField] public AudioClip InfernoClip { get; private set; } = default;
        [field: SerializeField] public AudioClip NecropolisClip { get; private set; } = default;
        [field: SerializeField] public AudioClip DungeonClip { get; private set; } = default;
        [field: SerializeField] public AudioClip StrongholdClip { get; private set; } = default;
        [field: SerializeField] public AudioClip FortressClip { get; private set; } = default;

        [field: Header("Battle audio clips")]
        [field: SerializeField] public AudioClip WinClip { get; private set; } = default;
        [field: SerializeField] public AudioClip LoseClip { get; private set; } = default;
        [field: Space(8)]
        [field: SerializeField] public AudioClip Battle1 { get; private set; } = default;
        [field: SerializeField] public AudioClip Battle2 { get; private set; } = default;
        [field: SerializeField] public AudioClip Battle3 { get; private set; } = default;
        [field: SerializeField] public AudioClip Battle4 { get; private set; } = default;
        [field: SerializeField] public AudioClip Battle5 { get; private set; } = default;
        [field: SerializeField] public AudioClip Battle6 { get; private set; } = default;
        [field: SerializeField] public AudioClip Battle7 { get; private set; } = default;
        [field: SerializeField] public AudioClip Battle8 { get; private set; } = default;
        [field: Space(8)]
        [field: SerializeField] public AudioClip Combat1 { get; private set; } = default;
        [field: SerializeField] public AudioClip Combat2 { get; private set; } = default;
        [field: SerializeField] public AudioClip Combat3 { get; private set; } = default;
        [field: SerializeField] public AudioClip Combat4 { get; private set; } = default;

        [field: Header("Misc audio clips")]
        [field: SerializeField] public AudioClip MainMenuClip { get; private set; } = default;
        [field: SerializeField] public AudioClip NextWeekClip { get; private set; } = default;
        [field: SerializeField] public AudioClip ClickClip { get; private set; } = default;
        [field: SerializeField] public AudioClip BuildClip { get; private set; } = default;
        [field: SerializeField] public AudioClip LevelUpClip { get; private set; } = default;

        public AudioClip GetTownClip(Town town) => town switch
        {
            Town.Castle => CastleClip,
            Town.Rampart => RampartClip,
            Town.Tower => TowerClip,
            Town.Inferno => InfernoClip,
            Town.Necropolis => NecropolisClip,
            Town.Dungeon => DungeonClip,
            Town.Stronghold => StrongholdClip,
            Town.Fortress => FortressClip,
            _ => throw new(),
        };

        public AudioClip GetRandomCombat(ref Unity.Mathematics.Random random) => random.NextInt(0, 4) switch
        {
            0 => Combat1,
            1 => Combat2,
            2 => Combat3,
            3 => Combat4,
            _ => throw new(),
        };

        public AudioClip GetRandomBattle(ref Unity.Mathematics.Random random) => random.NextInt(0, 8) switch
        {
            0 => Battle1,
            1 => Battle2,
            2 => Battle3,
            3 => Battle4,
            4 => Battle5,
            5 => Battle6,
            6 => Battle7,
            7 => Battle8,
            _ => throw new(),
        };
    }
}