using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TetrisPiece : MonoBehaviour {

	[SerializeField]
	private bool[] _layout;
	[SerializeField]
	private Color _blockColor;

	private GameObject[] _blocks = new GameObject[16];

	void Awake()
	{
		InitForLayout();
	}

	private void InitForLayout()
	{
		for(int i = 0; i < 4; i++)
		{
			for(int j = 0; j < 4; j++)
			{
				if(_layout[i * 4 + j])
				{
					var block = GameObject.Instantiate(Resources.Load("Tetris/TetrisBlock")) as GameObject;
					block.transform.parent = transform;
					block.transform.localPosition = new Vector3(j * TetrisGame.PIECE_SIZE - TetrisGame.HALF_PIECE_SIZE, i * TetrisGame.PIECE_SIZE - TetrisGame.HALF_PIECE_SIZE);

					var sprite = block.GetComponent<SpriteRenderer>();
					sprite.color = _blockColor;

					_blocks[i * 4 + j] = block;
				}
			}
		}
	}

	private void Start()
	{
		if(TetrisGame.Instance.CheckAndHandleCollisions(0, 0, false))
		{
			TetrisGame.Instance.GameOver();
		}
	}

	public void TryRotate()
	{
		var proposedLayout = new bool[16];
		for(int i = 0; i < 4; i++)
		{
			for(int j = 0; j < 4; j++)
			{
				proposedLayout[(3 - j) * 4 + i] = _blocks[i * 4 + j] != null;
			}
		}

		if(!TetrisGame.Instance.CheckRotateCollisions(proposedLayout, transform.position))
		{
			ApplyRotate(proposedLayout);
		}
	}

	private void ApplyRotate(bool[] newLayout)
	{
		_layout = newLayout;
		DeleteOldBlocks();
		InitForLayout();
	}

	private void DeleteOldBlocks()
	{
		foreach(var block in _blocks)
		{
			if(block != null)
				GameObject.Destroy(block);
		}
	}
}
