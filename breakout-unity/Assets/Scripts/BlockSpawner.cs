using System;
using UnityEngine;

public class BlockSpawner : MonoBehaviour {
	[SerializeField] private Block _blockPrefab;

	private LevelLoader _loader = new LevelLoader();

	private int blocksLeft = 0;
	public event Action allDestroyed;
	
	public void SpawnLevel(int index) {
		var levelInfo = _loader.LoadLevel(index);

		if (levelInfo == null) {
			Debug.LogError($"Unable to load level {index}");
			return;
		}

		ClearBlocks();
		SpawnBlocks(levelInfo.blocks);
	}

	private void ClearBlocks() {
		foreach (var block in GetComponentsInChildren<Block>()) {
			block.destroyed -= OnBlockDestroyed;
			Destroy(block.gameObject);
		}

		blocksLeft = 0;
	}

	private void SpawnBlocks(BlockInfo[,] blocks) {
		var blockWidth = 1.4f;
		var blockHeight = 0.4f;
		var spacing = 0.2f;

		var maxWidth = 10;
		var maxHeight = 6;

		var scaleX = maxWidth / ((blockWidth + spacing) * blocks.GetLength(0));
		var scaleY = maxHeight / ((blockHeight + spacing) * blocks.GetLength(1));

		var scale = Mathf.Min(scaleX, scaleY, 1);
		
		blockWidth *= scale;
		blockHeight *= scale;
		spacing *= scale;
		
		var centerX = (blocks.GetLength(0) - 1) / 2f;

		for (var x = 0; x < blocks.GetLength(0); x++) {
			for (var y = 0; y < blocks.GetLength(1); y++) {
				var blockInfo = blocks[x, y];

				if (blockInfo == null) {
					continue;
				}
				
				var offsetX = x - centerX;

				var pos = new Vector2(offsetX * (blockWidth + spacing), - y * (blockHeight + spacing) + blockHeight / 2);

				var instance = Instantiate(_blockPrefab, transform);
				instance.transform.localScale = Vector3.one * scale;

				instance.transform.localPosition = pos;
				instance.SetInfo(blockInfo);

				blocksLeft++;
				instance.destroyed += OnBlockDestroyed;
			}
		}
	}

	private void OnBlockDestroyed() {
		blocksLeft--;

		if (blocksLeft == 0) {
			allDestroyed?.Invoke();
		}
	}
}
