using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace andywiecko.HeroesMusicManager
{
    public class CreateGameView : MonoBehaviour
    {
        public const int TownMax = 8;

        [SerializeField] private AudioSource audioSource = default;
        [SerializeField] private AudioSource buildAudioSource = default;
        [SerializeField] private AudioSource levelAudioSource = default;

        [SerializeField] private GameObject mainView = default;
        [SerializeField] private VisualTreeAsset nextPlayerView = default;
        [SerializeField] private VisualTreeAsset playerView = default;
        [SerializeField] private BattleView battleView = default;

        [SerializeField] private AudioClip nextWeek = default;
        [SerializeField] private AudioClip castleClip = default;
        [SerializeField] private AudioClip rampartClip = default;
        [SerializeField] private AudioClip towerClip = default;
        [SerializeField] private AudioClip infernoClip = default;
        [SerializeField] private AudioClip necropolisClip = default;
        [SerializeField] private AudioClip dungeonClip = default;
        [SerializeField] private AudioClip strongholdClip = default;
        [SerializeField] private AudioClip fortressClip = default;

        [SerializeField] private AudioClip winClip = default;
        [SerializeField] private AudioClip loseClip = default;

        [SerializeField] private List<Town> townsData = new();

        private UIDocument ui;

        private void Awake()
        {
            ui = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            var root = ui.rootVisualElement;

            var begin = root.Q<Button>("begin");
            begin.clicked += () => { gameObject.SetActive(false); CreatePlayers(); };

            var back = root.Q<Button>("back");
            back.clicked += () => { gameObject.SetActive(false); mainView.SetActive(true); };

            var towns = root.Q<ListView>("towns");
            towns.makeItem = () => towns.itemTemplate.CloneTree();
            towns.itemsSource = townsData;
            towns.bindItem = (v, i) =>
            {
                v.Q<Label>("town").text = townsData[i].ToString();
                v.Q<Button>("remove").clicked += () => { townsData.RemoveAt(i); towns.Rebuild(); };
                v.Q<Button>("left").clicked += () => { townsData[i] = (Town)(((int)townsData[i] - 1 + TownMax) % TownMax); towns.Rebuild(); };
                v.Q<Button>("right").clicked += () => { townsData[i] = (Town)(((int)townsData[i] + 1) % TownMax); towns.Rebuild(); };

                if (i != 0 && townsData.Count > 1) v.Q<Button>("up").clicked += () => { (townsData[i - 1], townsData[i]) = (townsData[i], townsData[i - 1]); towns.Rebuild(); };
                else v.Q<Button>("up").SetEnabled(false);

                if (i != townsData.Count - 1 && townsData.Count > 1) v.Q<Button>("down").clicked += () => { (townsData[i + 1], townsData[i]) = (townsData[i], townsData[i + 1]); towns.Rebuild(); };
                else v.Q<Button>("down").SetEnabled(false);
            };

            var scrollView = towns.Q<ScrollView>();
            scrollView.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;

            var addPlayer = root.Q<Button>("add-player");
            addPlayer.clicked += () => { townsData.Add((Town)(townsData.Count % TownMax)); towns.Rebuild(); };
        }

        private void CreatePlayers()
        {
            var players = new GameObject("Players");

            var nViews = new List<NextPlayerView>();
            var pViews = new List<PlayerView>();

            for (int i = 0; i < townsData.Count; i++)
            {
                var turn = new GameObject($"Player {i + 1}'s turn") { transform = { parent = players.transform } };
                var ui0 = turn.AddComponent<UIDocument>();
                ui0.visualTreeAsset = nextPlayerView;
                ui0.panelSettings = ui.panelSettings;

                var nView = turn.AddComponent<NextPlayerView>();
                nView.Town = townsData[i];
                nView.SetAudio(audioSource, nextWeek);
                nViews.Add(nView);

                var player = new GameObject($"Player {i + 1}") { transform = { parent = players.transform } };
                var ui1 = player.AddComponent<UIDocument>();
                ui1.visualTreeAsset = playerView;
                ui1.panelSettings = ui.panelSettings;

                var pView = player.AddComponent<PlayerView>();
                pView.Town = townsData[i];
                pView.AudioSource = audioSource;
                pView.AudioClip = townsData[i] switch
                {
                    Town.Castle => castleClip,
                    Town.Rampart => rampartClip,
                    Town.Tower => towerClip,
                    Town.Inferno => infernoClip,
                    Town.Necropolis => necropolisClip,
                    Town.Dungeon => dungeonClip,
                    Town.Stronghold => strongholdClip,
                    Town.Fortress => fortressClip,
                    _ => throw new(),
                };
                pView.BattleView = battleView;
                pViews.Add(pView);
                pView.WinClip = winClip;
                pView.LoseClip = loseClip;
                pView.BuildAudioSource = buildAudioSource;
                pView.LevelAudioSource = levelAudioSource;

                turn.SetActive(false);
                player.SetActive(false);
            }

            for (int i = 0; i < townsData.Count; i++)
            {
                nViews[i].Next = pViews[i].gameObject;
                pViews[i].Next = nViews[(i + 1) % townsData.Count].gameObject;
            }

            nViews[0].gameObject.SetActive(true);
        }
    }
}