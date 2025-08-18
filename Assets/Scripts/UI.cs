using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace andywiecko.HeroesMusicManager
{
    [RequireComponent(typeof(UIDocument))]
    public class UI : MonoBehaviour
    {
        private const int TownMax = 8;

        [SerializeField] private AudioData audioData = default;

        [Header("UI Assets")]
        [SerializeField] private VisualTreeAsset mainViewTemplate = default;
        [SerializeField] private VisualTreeAsset createViewTemplate = default;
        [SerializeField] private VisualTreeAsset nextPlayerViewTemplate = default;
        [SerializeField] private VisualTreeAsset playerViewTemplate = default;
        [SerializeField] private VisualTreeAsset battleViewTemplate = default;

        private VisualElement Root => ui.rootVisualElement;
        private UIDocument ui;

        private AudioSource mainAudioSource, buildAudioSource, levelAudioSource, clickAudioSource;
        private VisualElement mainView, createView, battleView, currentPlayer;
        private readonly List<VisualElement> nextPlayerViews = new(), playerViews = new();
        private readonly List<Town> townsData = new() { Town.Castle };
        private Town currentTown;
        private bool isBattle, afterBattle;
        private Unity.Mathematics.Random random;

        private void Awake()
        {
            ui = GetComponent<UIDocument>();
            random = new(seed: 42);

            mainAudioSource = gameObject.AddComponent<AudioSource>();
            mainAudioSource.clip = audioData.MainMenuClip;
            mainAudioSource.loop = true;
            mainAudioSource.playOnAwake = false;
            mainAudioSource.Play();

            buildAudioSource = gameObject.AddComponent<AudioSource>();
            buildAudioSource.clip = audioData.BuildClip;
            buildAudioSource.playOnAwake = false;
            buildAudioSource.loop = false;

            levelAudioSource = gameObject.AddComponent<AudioSource>();
            levelAudioSource.clip = audioData.LevelUpClip;
            levelAudioSource.playOnAwake = false;
            levelAudioSource.loop = false;

            clickAudioSource = gameObject.AddComponent<AudioSource>();
            clickAudioSource.clip = audioData.ClickClip;
            clickAudioSource.playOnAwake = false;
            clickAudioSource.loop = false;

            mainView = mainViewTemplate.CloneTree();
            createView = createViewTemplate.CloneTree();
            battleView = battleViewTemplate.CloneTree();

            RegisterMainView();
            RegisterCreateView();
            RegisterBattleView();

            ui.rootVisualElement.Add(mainView);
        }

        private void Update()
        {
            if (afterBattle && !mainAudioSource.isPlaying)
            {
                MainAudio(audioData.GetTownClip(currentTown), loop: true);
                afterBattle = false;
            }

            if (isBattle && !mainAudioSource.isPlaying)
            {
                MainAudio(audioData.GetRandomCombat(ref random), loop: true);
                isBattle = false;
            }
        }

        private void RegisterMainView()
        {
            mainView.style.height = new StyleLength(Length.Percent(100));
            mainView.Q<Button>("new-game").clicked += () => { Root.Clear(); Root.Add(createView); clickAudioSource.Play(); };
            mainView.Q<Button>("credits").clicked += () => { Debug.Log("Show credits"); clickAudioSource.Play(); };
            mainView.Q<Button>("exit").clicked += Application.Quit;
        }

        private void MainAudio(AudioClip clip, bool loop = default)
        {
            mainAudioSource.clip = clip;
            mainAudioSource.loop = loop;
            mainAudioSource.Play();
        }

        private void RegisterCreateView()
        {
            createView.style.height = new StyleLength(Length.Percent(100));

            var begin = createView.Q<Button>("begin");
            begin.clicked += () => { Root.Clear(); CreatePlayers(); Root.Add(nextPlayerViews[0]); MainAudio(audioData.NextWeekClip); clickAudioSource.Play(); };

            createView.Q<Button>("back").clicked += () => { Root.Clear(); Root.Add(mainView); townsData.Clear(); townsData.Add(Town.Castle); begin.SetEnabled(townsData.Count > 0); clickAudioSource.Play(); };

            var towns = createView.Q<ListView>("towns");
            towns.makeItem = () => towns.itemTemplate.CloneTree();
            towns.itemsSource = townsData;
            towns.bindItem = (v, i) =>
            {
                v.Q<Label>("town").text = townsData[i].ToString();
                v.Q<Button>("remove").clicked += () => { townsData.RemoveAt(i); towns.Rebuild(); begin.SetEnabled(townsData.Count > 0); clickAudioSource.Play(); };
                v.Q<Button>("left").clicked += () => { townsData[i] = (Town)(((int)townsData[i] - 1 + TownMax) % TownMax); towns.Rebuild(); clickAudioSource.Play(); };
                v.Q<Button>("right").clicked += () => { townsData[i] = (Town)(((int)townsData[i] + 1) % TownMax); towns.Rebuild(); clickAudioSource.Play(); };

                if (i != 0 && townsData.Count > 1) v.Q<Button>("up").clicked += () => { (townsData[i - 1], townsData[i]) = (townsData[i], townsData[i - 1]); towns.Rebuild(); clickAudioSource.Play(); };
                else v.Q<Button>("up").SetEnabled(false);

                if (i != townsData.Count - 1 && townsData.Count > 1) v.Q<Button>("down").clicked += () => { (townsData[i + 1], townsData[i]) = (townsData[i], townsData[i + 1]); towns.Rebuild(); clickAudioSource.Play(); };
                else v.Q<Button>("down").SetEnabled(false);
            };

            var scrollView = towns.Q<ScrollView>();
            scrollView.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;

            createView.Q<Button>("add-player").clicked += () => { townsData.Add((Town)(townsData.Count % TownMax)); towns.Rebuild(); begin.SetEnabled(townsData.Count > 0); clickAudioSource.Play(); };
        }

        private void RegisterBattleView()
        {
            battleView.style.height = new StyleLength(Length.Percent(100));

            battleView.Q<Button>("win").clicked += () => { Root.Clear(); Root.Add(currentPlayer); MainAudio(audioData.WinClip); afterBattle = true; clickAudioSource.Play(); };
            battleView.Q<Button>("lose").clicked += () => { Root.Clear(); Root.Add(currentPlayer); MainAudio(audioData.LoseClip); afterBattle = true; clickAudioSource.Play(); };
        }

        private void CreatePlayers()
        {
            for (int i = 0; i < townsData.Count; i++)
            {
                var nextPlayerView = nextPlayerViewTemplate.CloneTree();
                nextPlayerViews.Add(nextPlayerView);
                nextPlayerView.style.height = new StyleLength(Length.Percent(100));

                nextPlayerView.Q<Label>("player-name").text = $"Player {i + 1}'s turn";
                nextPlayerView.Q<Label>("player-color").text = townsData[i].ToString();

                var playerView = playerViewTemplate.CloneTree();
                playerViews.Add(playerView);
                playerView.style.height = new StyleLength(Length.Percent(100));

                playerView.Q<Label>("player-town").text = townsData[i].ToString();
                playerView.Q<Button>("fight").clicked += () => { Root.Clear(); Root.Add(battleView); currentPlayer = playerView; isBattle = true; MainAudio(audioData.GetRandomBattle(ref random)); clickAudioSource.Play(); };
                playerView.Q<Button>("build").clicked += buildAudioSource.Play;
                playerView.Q<Button>("level").clicked += levelAudioSource.Play;
                playerView.Q<Button>("exit").clicked += Application.Quit;
            }

            for (int i = 0; i < nextPlayerViews.Count; i++)
            {
                var player = playerViews[i];
                var townClip = audioData.GetTownClip(townsData[i]);
                var town = townsData[i];
                nextPlayerViews[i].Q<Button>("ok").clicked += () => { Root.Clear(); Root.Add(player); MainAudio(townClip, loop: true); currentTown = town; clickAudioSource.Play(); };

                var next = nextPlayerViews[(i + 1) % townsData.Count];
                var nextClip = audioData.NextWeekClip;
                playerViews[i].Q<Button>("next").clicked += () => { Root.Clear(); Root.Add(next); MainAudio(nextClip); clickAudioSource.Play(); };
            }
        }
    }
}