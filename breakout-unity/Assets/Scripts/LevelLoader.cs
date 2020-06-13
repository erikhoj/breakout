using System;
using UnityEngine;

public class LevelLoader {
	public LevelInfo LoadLevel(int index) {
		var textAsset = Resources.Load<TextAsset>($"levels/{index}");

		if (textAsset != null) {
			var text = textAsset.text;

			return ParseText(text);
		}

		return null;
	}

	// Parse the CSV file into a 2D block array
	private LevelInfo ParseText(string text) {
		var lines = text.Split(new [] { Environment.NewLine }, StringSplitOptions.None);

		var height = lines.Length;

		var width = 0;

		for (var y = 0; y < lines.Length; y++) {
			var numBlocks = lines[y].Split(',').Length;

			if (numBlocks > width) {
				width = numBlocks;
			}
		}

		var info = new LevelInfo() {
			blocks = new BlockInfo[width, height],
		};

		for (var y = 0; y < lines.Length; y++) {
			var blocks = lines[y].Split(',');
			
			for (var x = 0; x < lines.Length; x++) {
				var blockChar = blocks[x];

				if (int.TryParse(blockChar, out var hits)) {
					info.blocks[x, y] = new BlockInfo() {
						hits = hits,
					};
				} else {
					Debug.LogError($"Invalid character: {blockChar}, inserting single life block instead");
					info.blocks[x, y] = new BlockInfo() {
						hits = 1,
					};
				}
			}
		}

		return info;
	}
}
