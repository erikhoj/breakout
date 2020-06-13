using UnityEngine;

public class BlockSpawner : MonoBehaviour {
	[SerializeField] private Block _blockPrefab;

	private LevelLoader _loader;

	private void Awake() {
		_loader = new LevelLoader();

		SpawnLevel(1);
	}

	private void SpawnLevel(int index) {
		var levelInfo = _loader.LoadLevel(index);

		if (levelInfo == null) {
			Debug.LogError($"Unable to load level {index}");
		}

		SpawnBlocks(levelInfo.blocks);
	}

	private void SpawnBlocks(BlockInfo[,] blocks) {
		var blockWidth = 1.4f;
		var blockHeight = 0.4f;
		var spacing = 0.2f;

		var centerX = blocks.GetLength(0) / 2;

		for (var x = 0; x < blocks.GetLength(0); x++) {
			for (var y = 0; y < blocks.GetLength(1); y++) {
				var offsetX = x - centerX;

				var pos = new Vector2(offsetX * (blockWidth + spacing), - y * (blockHeight + spacing) + blockHeight / 2);

				var instance = Instantiate(_blockPrefab, transform);

				instance.transform.localPosition = pos;
			}
		}
	}
}
