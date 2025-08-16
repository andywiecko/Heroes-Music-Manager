using UnityEngine;
using UnityEngine.UIElements;

namespace andywiecko.HeroesMusicManager
{
    public class PlayerView : MonoBehaviour
    {
        public BattleView BattleView { get; set; }
        public GameObject Next { get; set; }
        public Town Town { get; set; }

        public AudioSource AudioSource;
        public AudioSource BuildAudioSource;
        public AudioSource LevelAudioSource;

        public AudioClip AudioClip;
        public AudioClip WinClip;
        public AudioClip LoseClip;

        public bool AfterWin;
        public bool AfterLose;

        private UIDocument ui;

        private void Awake()
        {
            ui = GetComponent<UIDocument>();
        }

        private void Play()
        {
            AudioSource.loop = !AfterWin && !AfterLose;
            AudioSource.clip = AfterWin ? WinClip : AfterLose ? LoseClip : AudioClip;
            AudioSource.Play();
        }

        public void OnEnable()
        {
            if (AudioSource) Play();

            var root = ui.rootVisualElement;

            var playerTown = root.Q<Label>("player-town");
            playerTown.text = Town.ToString();

            var next = root.Q<Button>("next");
            next.clicked += () => { gameObject.SetActive(false); Next.SetActive(true); };

            var fight = root.Q<Button>("fight");
            fight.clicked += () => { gameObject.SetActive(false); BattleView.Return = gameObject; BattleView.gameObject.SetActive(true); };

            var build = root.Q<Button>("build");
            build.clicked += () => { BuildAudioSource.Play(); };

            var level = root.Q<Button>("level");
            level.clicked += () => { LevelAudioSource.Play(); };

            var exit = root.Q<Button>("exit");
            exit.clicked += Application.Quit;
        }

        public void Update()
        {
            if ((AfterWin || AfterLose) && !AudioSource.isPlaying)
            {
                AfterWin = AfterLose = false;
                AudioSource.loop = true;
                AudioSource.clip = AudioClip;
                AudioSource.Play();
            }
        }
    }
}