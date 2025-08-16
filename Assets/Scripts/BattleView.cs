using UnityEngine;
using UnityEngine.UIElements;

namespace andywiecko.HeroesMusicManager
{
    public class BattleView : MonoBehaviour
    {
        public GameObject Return { get; set; }

        [SerializeField] private AudioSource audioSource;

        [SerializeField] private AudioClip begin1;
        [SerializeField] private AudioClip begin2;
        [SerializeField] private AudioClip begin3;
        [SerializeField] private AudioClip begin4;
        [SerializeField] private AudioClip begin5;
        [SerializeField] private AudioClip begin6;
        [SerializeField] private AudioClip begin7;
        [SerializeField] private AudioClip begin8;

        [SerializeField] private AudioClip combat1;
        [SerializeField] private AudioClip combat2;
        [SerializeField] private AudioClip combat3;
        [SerializeField] private AudioClip combat4;

        private int battleMax = 4;
        private int beginMax = 8;

        private Unity.Mathematics.Random rnd;

        private UIDocument ui;
        private bool isFirst = true;
        private bool iscombat = false;

        private void Awake()
        {
            ui = GetComponent<UIDocument>();
            rnd = new(seed: 42);
        }

        public void OnEnable()
        {
            audioSource.loop = false;
            audioSource.clip = Begin(rnd.NextInt(0, beginMax));
            audioSource.Play();
            isFirst = true;
            iscombat = false;

            var root = ui.rootVisualElement;

            var player = Return.GetComponent<PlayerView>();

            var win = root.Q<Button>("win");
            win.clicked += () => { gameObject.SetActive(false); player.AfterLose = false; player.AfterWin = true; Return.SetActive(true); };

            var lose = root.Q<Button>("lose");
            lose.clicked += () => { gameObject.SetActive(false); player.AfterLose = true; player.AfterWin = false; Return.SetActive(true); };
        }

        public void Update()
        {
            if (isFirst && audioSource.isPlaying || iscombat) return;

            audioSource.loop = true;
            audioSource.clip = Battle(rnd.NextInt(0, battleMax)); ;
            iscombat = true;
            audioSource.Play();

            isFirst = false;
        }

        private AudioClip Begin(int i) => i switch
        {
            0 => begin1,
            1 => begin2,
            2 => begin3,
            3 => begin4,
            4 => begin5,
            5 => begin6,
            6 => begin7,
            7 => begin8,
            _ => throw new(),
        };

        private AudioClip Battle(int i) => i switch
        {
            0 => combat1,
            1 => combat2,
            2 => combat3,
            3 => combat4,
            _ => throw new(),
        };
    }
}