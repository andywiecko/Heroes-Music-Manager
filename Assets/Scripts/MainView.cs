using UnityEngine;
using UnityEngine.UIElements;

namespace andywiecko.HeroesMusicManager
{
    public class MainView : MonoBehaviour
    {
        [SerializeField] private GameObject createGameView = default;

        private UIDocument ui;

        private void Awake()
        {
            ui = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            var root = ui.rootVisualElement;

            var newGame = root.Q<Button>("new-game");
            newGame.clicked += () => { gameObject.SetActive(false); createGameView.SetActive(true); };

            var credits = root.Q<Button>("credits");
            credits.clicked += () => Debug.Log("Show credits");

            var exit = root.Q<Button>("exit");
            exit.clicked += Application.Quit;
        }
    }
}