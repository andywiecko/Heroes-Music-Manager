using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace andywiecko.HeroesMusicManager
{
    public enum Town
    {
        Castle,
        Rampart,
        Tower,
        Inferno,
        Necropolis,
        Dungeon,
        Stronghold,
        Fortress,
    }

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
        [SerializeField] private VisualTreeAsset creditsViewTemplate = default;

        private VisualElement Root => ui.rootVisualElement;
        private UIDocument ui;

        private AudioSource mainAudio, buildAudio, levelAudio, clickAudio;
        private VisualElement mainView, createView, battleView, currentPlayer, creditsView;
        private readonly List<VisualElement> nextPlayerViews = new(), playerViews = new();
        private readonly List<Town> townsData = new() { Town.Castle };
        private Town currentTown;
        private bool isBattle, afterBattle;
        private Unity.Mathematics.Random random;

        private void Awake()
        {
            ui = GetComponent<UIDocument>();
            random = new(seed: 42);

            mainAudio = gameObject.AddComponent<AudioSource>();
            mainAudio.clip = audioData.MainMenuClip;
            mainAudio.loop = true;
            mainAudio.playOnAwake = false;
            mainAudio.Play();

            buildAudio = gameObject.AddComponent<AudioSource>();
            buildAudio.clip = audioData.BuildClip;
            buildAudio.playOnAwake = false;
            buildAudio.loop = false;

            levelAudio = gameObject.AddComponent<AudioSource>();
            levelAudio.clip = audioData.LevelUpClip;
            levelAudio.playOnAwake = false;
            levelAudio.loop = false;

            clickAudio = gameObject.AddComponent<AudioSource>();
            clickAudio.clip = audioData.ClickClip;
            clickAudio.playOnAwake = false;
            clickAudio.loop = false;

            mainView = mainViewTemplate.CloneTree();
            createView = createViewTemplate.CloneTree();
            battleView = battleViewTemplate.CloneTree();
            creditsView = creditsViewTemplate.CloneTree();

            RegisterMainView();
            RegisterCreateView();
            RegisterBattleView();
            RegisterCreditsView();

            ui.rootVisualElement.Add(mainView);
        }

        private void Update()
        {
            if (afterBattle && !mainAudio.isPlaying)
            {
                MainAudio(audioData.GetTownClip(currentTown), loop: true);
                afterBattle = false;
            }

            if (isBattle && !mainAudio.isPlaying)
            {
                MainAudio(audioData.GetRandomCombat(ref random), loop: true);
                isBattle = false;
            }
        }

        private void RegisterMainView()
        {
            mainView.style.height = new StyleLength(Length.Percent(100));
            mainView.Q<Button>("new-game").clicked += () => { Root.Clear(); Root.Add(createView); clickAudio.Play(); };
            mainView.Q<Button>("credits").clicked += () => { Root.Clear(); Root.Add(creditsView); clickAudio.Play(); };
            mainView.Q<Button>("exit").clicked += Application.Quit;
        }

        private void MainAudio(AudioClip clip, bool loop = default)
        {
            mainAudio.clip = clip;
            mainAudio.loop = loop;
            mainAudio.Play();
        }

        private void RegisterCreateView()
        {
            createView.style.height = new StyleLength(Length.Percent(100));

            var begin = createView.Q<Button>("begin");
            begin.clicked += () => { Root.Clear(); CreatePlayers(); Root.Add(nextPlayerViews[0]); MainAudio(audioData.NextWeekClip); clickAudio.Play(); };

            var towns = createView.Q<ListView>("towns");
            towns.RegisterCallback<PointerMoveEvent>(e => e.StopPropagation(), TrickleDown.TrickleDown); // NOTE: disable dragging
            towns.makeItem = () => towns.itemTemplate.CloneTree();
            towns.itemsSource = townsData;
            towns.bindItem = (v, i) =>
            {
                v.Q<Label>("town").text = townsData[i].ToString();

                var preview = v.Q<VisualElement>("preview");
                preview.RemoveFromClassList("town-castle");
                preview.RemoveFromClassList("town-rampart");
                preview.RemoveFromClassList("town-tower");
                preview.RemoveFromClassList("town-inferno");
                preview.RemoveFromClassList("town-necropolis");
                preview.RemoveFromClassList("town-dungeon");
                preview.RemoveFromClassList("town-stronghold");
                preview.RemoveFromClassList("town-fortress");
                var previewStyle = townsData[i] switch
                {
                    Town.Castle => "town-castle",
                    Town.Rampart => "town-rampart",
                    Town.Tower => "town-tower",
                    Town.Inferno => "town-inferno",
                    Town.Necropolis => "town-necropolis",
                    Town.Dungeon => "town-dungeon",
                    Town.Stronghold => "town-stronghold",
                    Town.Fortress => "town-fortress",
                    _ => throw new NotImplementedException(),
                };
                preview.AddToClassList(previewStyle);

                v.Q<Button>("remove").clicked += () => { townsData.RemoveAt(i); towns.Rebuild(); begin.SetEnabled(townsData.Count > 0); clickAudio.Play(); };
                v.Q<Button>("remove").SetEnabled(townsData.Count > 1);
                v.Q<Button>("remove").RegisterCallback<PointerMoveEvent>(e => e.StopPropagation(), TrickleDown.TrickleDown); // NOTE: disable dragging

                v.Q<Button>("left").clicked += () => { townsData[i] = (Town)(((int)townsData[i] - 1 + TownMax) % TownMax); towns.Rebuild(); clickAudio.Play(); };
                v.Q<Button>("left").RegisterCallback<PointerMoveEvent>(e => e.StopPropagation(), TrickleDown.TrickleDown); // NOTE: disable dragging

                v.Q<Button>("right").clicked += () => { townsData[i] = (Town)(((int)townsData[i] + 1) % TownMax); towns.Rebuild(); clickAudio.Play(); };
                v.Q<Button>("right").RegisterCallback<PointerMoveEvent>(e => e.StopPropagation(), TrickleDown.TrickleDown); // NOTE: disable dragging

                v.Q<Button>("up").clicked += () => { (townsData[i - 1], townsData[i]) = (townsData[i], townsData[i - 1]); towns.Rebuild(); clickAudio.Play(); };
                v.Q<Button>("up").SetEnabled(i != 0 && townsData.Count > 1);
                v.Q<Button>("up").RegisterCallback<PointerMoveEvent>(e => e.StopPropagation(), TrickleDown.TrickleDown); // NOTE: disable dragging

                v.Q<Button>("down").clicked += () => { (townsData[i + 1], townsData[i]) = (townsData[i], townsData[i + 1]); towns.Rebuild(); clickAudio.Play(); };
                v.Q<Button>("down").SetEnabled(i != townsData.Count - 1 && townsData.Count > 1);
                v.Q<Button>("down").RegisterCallback<PointerMoveEvent>(e => e.StopPropagation(), TrickleDown.TrickleDown); // NOTE: disable dragging
            };

            var scrollView = towns.Q<ScrollView>();
            scrollView.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;

            createView.Q<Button>("add-player").clicked += () => { townsData.Add((Town)(townsData.Count % TownMax)); towns.Rebuild(); begin.SetEnabled(townsData.Count > 0); clickAudio.Play(); };
            createView.Q<Button>("back").clicked += () => { Root.Clear(); Root.Add(mainView); townsData.Clear(); townsData.Add(Town.Castle); towns.Rebuild(); begin.SetEnabled(townsData.Count > 0); clickAudio.Play(); };
        }

        private void RegisterBattleView()
        {
            battleView.style.height = new StyleLength(Length.Percent(100));

            battleView.Q<Button>("win").clicked += () => { Root.Clear(); Root.Add(currentPlayer); MainAudio(audioData.WinClip); isBattle = false; afterBattle = true; clickAudio.Play(); };
            battleView.Q<Button>("lose").clicked += () => { Root.Clear(); Root.Add(currentPlayer); MainAudio(audioData.LoseClip); isBattle = false; afterBattle = true; clickAudio.Play(); };
        }

        private void RegisterCreditsView()
        {
            creditsView.style.height = new StyleLength(Length.Percent(100));
            creditsView.Q<Button>("ok").clicked += () => { Root.Clear(); Root.Add(mainView); clickAudio.Play(); };
            creditsView.Q<Label>("version").text = $"Version: {Application.version}";
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
                nextPlayerView.Q<VisualElement>("flag").AddToClassList(townsData[i] switch
                {
                    Town.Castle => "flag-castle",
                    Town.Rampart => "flag-rampart",
                    Town.Tower => "flag-tower",
                    Town.Inferno => "flag-inferno",
                    Town.Necropolis => "flag-necropolis",
                    Town.Dungeon => "flag-dungeon",
                    Town.Stronghold => "flag-stronghold",
                    Town.Fortress => "flag-fortress",
                    _ => throw new NotImplementedException()
                });

                var playerView = playerViewTemplate.CloneTree();
                playerViews.Add(playerView);
                playerView.style.height = new StyleLength(Length.Percent(100));

                playerView.Q<Label>("player-town").text = townsData[i].ToString();
                playerView.Q<Button>("fight").clicked += () => { Root.Clear(); Root.Add(battleView); currentPlayer = playerView; afterBattle = false; isBattle = true; MainAudio(audioData.GetRandomBattle(ref random)); clickAudio.Play(); };
                playerView.Q<Button>("build").clicked += buildAudio.Play;
                playerView.Q<Button>("level").clicked += levelAudio.Play;
                playerView.Q<Button>("exit").clicked += Application.Quit;
            }

            for (int i = 0; i < nextPlayerViews.Count; i++)
            {
                var player = playerViews[i];
                var townClip = audioData.GetTownClip(townsData[i]);
                var town = townsData[i];
                nextPlayerViews[i].Q<Button>("ok").clicked += () => { Root.Clear(); Root.Add(player); MainAudio(townClip, loop: true); currentTown = town; clickAudio.Play(); };

                var next = nextPlayerViews[(i + 1) % townsData.Count];
                var nextClip = audioData.NextWeekClip;
                playerViews[i].Q<Button>("next").clicked += () => { Root.Clear(); Root.Add(next); MainAudio(nextClip); clickAudio.Play(); };
            }
        }
    }
}