using UnityEngine;
using System.Collections;

public class HighScoresUtil : MonoBehaviour {

	public const string TETRIS_HIGH_SCORE_PREF = "TetrisHighScore";

	private static int _tetrisHighScore;
	public static int TetrisHighScore 
	{
		get
		{
			return _tetrisHighScore;
		}
		set 
		{ 
			_tetrisHighScore = value;
			SaveHighScore(TETRIS_HIGH_SCORE_PREF, _tetrisHighScore);
		} 
	}

	static HighScoresUtil()
	{
		TetrisHighScore = PlayerPrefs.GetInt(TETRIS_HIGH_SCORE_PREF);
	}

	public static int GetHighScore(string highScorePref)
	{
		return PlayerPrefs.GetInt(highScorePref);
	}

	private static void SaveHighScore(string highScorePref, int score)
	{
		PlayerPrefs.SetInt(highScorePref, score);
		PlayerPrefs.Save();
	}
}
