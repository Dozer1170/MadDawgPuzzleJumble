using UnityEngine;
using System.Collections;

public class DescendingPiece : MonoBehaviour 
{
	public float SecondsBetweenDrops { get; set; }

	void Start()
	{
		StartCoroutine(DropCoroutine());
	}

	private IEnumerator DropCoroutine()
	{
		while(TetrisGame.Instance.Started)
		{
			yield return new WaitForSeconds(SecondsBetweenDrops);

			if(!TetrisGame.Instance.CheckAndHandleCollisions(0, -TetrisGame.PIECE_SIZE))
			{
				transform.position += new Vector3(0, -TetrisGame.PIECE_SIZE, 0);
			}
		}
	}
}
