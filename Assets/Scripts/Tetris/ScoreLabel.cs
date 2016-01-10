using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreLabel : MonoBehaviour 
{
	[SerializeField]
	private Text _text;

	void Update () 
	{
		_text.text = "Score: " + TetrisGame.Instance.Score;
	}
}
