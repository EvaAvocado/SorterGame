using Configuration;
using Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace UI
{
    /// <summary>
    /// Контроллер, управляющий всеми элементами игрового интерфейса
    /// Подписывается на игровые события и обновляет UI (счет, жизни, панели победы/поражения)
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [Header("In-Game UI")] [SerializeField]
        private TMP_Text _scoreText;

        [SerializeField] private TMP_Text _healthText;

        [Header("End-Game Panels")] [SerializeField]
        private GameObject _winPanel;

        [SerializeField] private TMP_Text _finalScoreText;
        [SerializeField] private GameObject _losePanel;

        [Header("Buttons")] [SerializeField] private Button _restartButtonWin;
        [SerializeField] private Button _restartButtonLose;

        private EventBus _eventBus;
        private GameConfig _config;
        private Sequence _scoreSequence;
        private Sequence _healthSequence;

        [Inject]
        public void Construct(EventBus eventBus, GameConfig config)
        {
            _eventBus = eventBus;
            _config = config;
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<GameEvents.ScoreUpdated>(OnScoreUpdated);
            _eventBus.Subscribe<GameEvents.HealthUpdated>(OnHealthUpdated);
            _eventBus.Subscribe<GameEvents.GameWon>(OnGameWon);
            _eventBus.Subscribe<GameEvents.GameLost>(OnGameLost);

            _restartButtonWin.onClick.AddListener(RestartGame);
            _restartButtonLose.onClick.AddListener(RestartGame);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<GameEvents.ScoreUpdated>(OnScoreUpdated);
            _eventBus.Unsubscribe<GameEvents.HealthUpdated>(OnHealthUpdated);
            _eventBus.Unsubscribe<GameEvents.GameWon>(OnGameWon);
            _eventBus.Unsubscribe<GameEvents.GameLost>(OnGameLost);

            _restartButtonWin.onClick.RemoveListener(RestartGame);
            _restartButtonLose.onClick.RemoveListener(RestartGame);

            _scoreSequence?.Kill();
            _healthSequence?.Kill();
        }

        private void OnScoreUpdated(GameEvents.ScoreUpdated e)
        {
            _scoreText.text = $"Счет: {e.NewScore}";

            _scoreSequence?.Kill();
            _scoreText.transform.localScale = Vector3.one;

            float duration = _config.Animations.UIPulseDuration;
            float scale = _config.Animations.ScorePulseScale;

            _scoreSequence = DOTween.Sequence()
                .Append(_scoreText.transform.DOScale(scale, duration / 2))
                .Append(_scoreText.transform.DOScale(1f, duration / 2))
                .SetUpdate(true);
        }

        private void OnHealthUpdated(GameEvents.HealthUpdated e)
        {
            _healthText.text = $"Жизни: {e.NewHealth}";

            _healthSequence?.Kill();
            _healthText.transform.localScale = Vector3.one;

            float duration = _config.Animations.UIPulseDuration;
            float scale = _config.Animations.HealthPulseScale;

            _healthSequence = DOTween.Sequence()
                .Append(_healthText.transform.DOScale(scale, duration / 2))
                .Append(_healthText.transform.DOScale(1f, duration / 2))
                .SetUpdate(true);
        }

        private void OnGameWon(GameEvents.GameWon e)
        {
            _finalScoreText.text = $"Ваш счет: {e.FinalScore}";
            _winPanel.SetActive(true);
        }

        private void OnGameLost(GameEvents.GameLost e)
        {
            _losePanel.SetActive(true);
        }

        private void RestartGame()
        {
            // Убиваем все анимации твинов перед перезагрузкой, чтобы избежать ошибок
            DOTween.KillAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}