using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour {
	[SerializeField] private Button _backButton;
	[SerializeField] private LevelSelector _levelSelector;

	private CanvasGroup _backButtonGroup;
	private CanvasGroup _levelSelectorGroup;
	private Level _currentLevel;

	private void Awake() {
		_backButtonGroup = _backButton.GetComponent<CanvasGroup>();
		_levelSelectorGroup = _levelSelector.GetComponent<CanvasGroup>();

		_backButtonGroup.alpha = 0;
		_backButtonGroup.blocksRaycasts = false;

		_levelSelector.selected += OnLevelSelected;
		
		_backButton.onClick.AddListener(ShowLevelSelector);
	}

	private void OnLevelSelected(Level level) {
		_currentLevel = level;
		level.lost += ShowLevelSelector;
		level.won += ShowLevelSelector;
		
		_levelSelectorGroup.blocksRaycasts = false;

		var sequence = DOTween.Sequence();
		sequence.Append(_levelSelectorGroup.DOFade(0f, 0.5f).SetEase(Ease.OutSine));
		sequence.Insert(0.25f, level.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutSine));
		sequence.Insert(0.5f, _backButtonGroup.DOFade(1f, 0.5f).SetEase(Ease.OutSine));
		sequence.AppendCallback(() => {
			level.StartLevel();
			_backButtonGroup.blocksRaycasts = true;
		});
	}

	private void ShowLevelSelector() {
		if (_currentLevel) {
			_currentLevel.lost -= ShowLevelSelector;
			_currentLevel.won -= ShowLevelSelector;

			_currentLevel.Reset();
		}

		_backButtonGroup.blocksRaycasts = false;
		
		var sequence = DOTween.Sequence();
		sequence.Append(_backButtonGroup.DOFade(0, 0.5f).SetEase(Ease.OutSine));
		sequence.Insert(0.25f, _currentLevel.transform.DOScale(Vector3.one * _levelSelector.selectedScale, 0.5f).SetEase(Ease.InOutSine));
		sequence.Insert(0.5f, _levelSelectorGroup.DOFade(1f, 0.5f).SetEase(Ease.OutSine));
		sequence.AppendCallback(() => { _levelSelectorGroup.blocksRaycasts = true; });
		
		_currentLevel = null;
	}
}
