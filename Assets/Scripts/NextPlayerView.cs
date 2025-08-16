using UnityEngine;
using UnityEngine.UIElements;

namespace andywiecko.HeroesMusicManager
{
    public class NextPlayerView : MonoBehaviour
    {
        public GameObject Next { get; set; }
        public Town Town { get; set; }
        public AudioSource AudioSource;
        public AudioClip AudioClip;

        private UIDocument ui;

        private void Awake()
        {
            ui = GetComponent<UIDocument>();
        }

        public void SetAudio(AudioSource source, AudioClip clip)
        {
            AudioSource = source;
            AudioClip = clip;
        }

        private void Play()
        {
            AudioSource.loop = false;
            AudioSource.clip = AudioClip;
            AudioSource.Play();
        }

        public void OnEnable()
        {
            if (AudioSource) Play();

            var root = ui.rootVisualElement;

            var playerName = root.Q<Label>("player-name");
            playerName.text = gameObject.name;

            var playerColor = root.Q<Label>("player-color");
            playerColor.text = Town.ToString();

            var ok = root.Q<Button>("ok");
            ok.clicked += () => { gameObject.SetActive(false); Next.SetActive(true); };
        }
    }
}