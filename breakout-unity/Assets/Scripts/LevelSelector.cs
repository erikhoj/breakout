using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour {
	[SerializeField] private Level _levelPrefab;
	[SerializeField] private float _distBetweenLevels;
	[SerializeField] private float _selectedScale;
	[SerializeField] private float _unSelectedScale;

	[SerializeField] private Button _previousButton;
	[SerializeField] private Button _nextButton;
	[SerializeField] private Button _selectButton;

	public float selectedScale => _selectedScale;
	
	private int _currentIndex;

	private Level[] _levels;
	private Transform _levelParent;

	public event Action<Level> selected;
	
	private void Awake() {
		_levelParent = new GameObject("LevelParent").transform;
		
		var numLevels = Resources.LoadAll<TextAsset>("levels/").Length;
		
		// TODO - Optimize this, levels should be spawned when they are needed
		_levels = new Level[numLevels];

		for (var i = 0; i < numLevels; i++) {
			var level = Instantiate(_levelPrefab, _levelParent.transform);
			
			level.SetIndex(i + 1);

			level.transform.position += Vector3.right * _distBetweenLevels * i;

			_levels[i] = level;

			var selected = i == _currentIndex;
			var scale = selected ? _selectedScale : _unSelectedScale;
			level.transform.localScale = Vector3.one * scale;
		}
		
		_previousButton.onClick.AddListener(ShowPrevious);
		_nextButton.onClick.AddListener(ShowNext);
		_selectButton.onClick.AddListener(Select);
	}

	private void ShowPrevious() {
		ShowIndex(Mathf.Max(0, _currentIndex - 1));
	}

	private void ShowNext() {
		ShowIndex(Mathf.Min(_levels.Length - 1, _currentIndex + 1));
	}

	private void Select() {
		selected?.Invoke(_levels[_currentIndex]);	
	}

	public Sequence Hide() {
		var sequence = DOTween.Sequence();

		return sequence;
	}
	
	private void ShowIndex(int index) {
		if (index == _currentIndex) {
			return;
		}
		
		DOTween.Kill("LevelSelector");
		
		_currentIndex = index;
		
		for (var i = 0; i < _levels.Length; i++) {
			var level = _levels[i];
			
			var selected = i == _currentIndex;
			var scale = selected ? _selectedScale : _unSelectedScale;
			level.transform.DOScale(Vector3.one * scale, 0.5f)
				.SetEase(Ease.OutSine)
				.SetId("LevelSelector");
		}

		_levelParent.DOMove(Vector3.left * _distBetweenLevels * _currentIndex, 0.5f)
			.SetEase(Ease.OutSine)
			.SetId("LevelSelector");
	}
}
