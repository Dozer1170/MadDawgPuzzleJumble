using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HighScoreLabel : MonoBehaviour {

	[SerializeField]
	private Text _text;
	[SerializeField]
	private string _playerPrefName = string.Empty;

	void Update () 
	{
		_text.text = "High Score: " + HighScoresUtil.GetHighScore(_playerPrefName);
	}
}
