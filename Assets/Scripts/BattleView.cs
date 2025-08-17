using UnityEngine;
using UnityEngine.UIElements;

namespace andywiecko.HeroesMusicManager
{
    public class BattleView : MonoBehaviour
    {
        public GameObject Return { get; set; }

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioData audioData = default;

        private UIDocument ui;
        private bool isFirst = true;
        private bool iscombat = false;

        private void Awake()
        {
            ui = GetComponent<UIDocument>();
        }

        public void OnEnable()
        {
            audioSource.loop = false;
            audioSource.clip = audioData.GetRandomBattle();
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
            audioSource.clip = audioData.GetRandomCombat();
            iscombat = true;
            audioSource.Play();

            isFirst = false;
        }
    }
}