using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TetrisGame : MonoBehaviour 
{
	public const float PIECE_SIZE = 0.32f;
	public const float HALF_PIECE_SIZE = 0.16f;
	public const int BOARD_WIDTH = 10, BOARD_HEIGHT = 20;
	public const float EPSILON = 0.01f;

	public static TetrisGame Instance;

	public int Score { get; private set; }

	[SerializeField]
	private GameObject[] _piecePrefabs;
	[SerializeField]
	private float _secondsBetweenVerticalDrops;
	[SerializeField]
	private TetrisPlayerController _controller;
	[SerializeField]
	private GameObject _gameOverText;
	[SerializeField]
	private bool _debug;

	private GameObject[,] _board;
	private GameObject[,] _gridHeirarchy;
	private bool _started, _gameOver;
	public bool Started { get { return _started; } }

	private TetrisPiece _currentPiece, _nextPiece;

	void Awake() 
	{
		Instance = this;

		_board = new GameObject[BOARD_WIDTH, BOARD_HEIGHT];
		_gridHeirarchy = new GameObject[BOARD_WIDTH, BOARD_HEIGHT];
		_started = false;

		CreateGridHeirarchyObjects();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(_gameOver && _controller.DidTouchScreen())
		{
			SceneManager.LoadScene(0);
		}

		if(!_started && _controller.DidTouchScreen())
		{
			StartGame();
		}
	}

	void OnGUI()
	{
		if(_debug)
		{
			foreach(GameObject boardPiece in _board)
			{
				if(boardPiece != null)
				{
					Debug.DrawLine(new Vector3(boardPiece.transform.position.x - HALF_PIECE_SIZE, boardPiece.transform.position.y + HALF_PIECE_SIZE),
						new Vector3(boardPiece.transform.position.x + HALF_PIECE_SIZE, boardPiece.transform.position.y + HALF_PIECE_SIZE));
					Debug.DrawLine(new Vector3(boardPiece.transform.position.x + HALF_PIECE_SIZE, boardPiece.transform.position.y + HALF_PIECE_SIZE),
						new Vector3(boardPiece.transform.position.x + HALF_PIECE_SIZE, boardPiece.transform.position.y - HALF_PIECE_SIZE));
					Debug.DrawLine(new Vector3(boardPiece.transform.position.x - HALF_PIECE_SIZE, boardPiece.transform.position.y + HALF_PIECE_SIZE),
						new Vector3(boardPiece.transform.position.x - HALF_PIECE_SIZE, boardPiece.transform.position.y - HALF_PIECE_SIZE));
					Debug.DrawLine(new Vector3(boardPiece.transform.position.x - HALF_PIECE_SIZE, boardPiece.transform.position.y - HALF_PIECE_SIZE),
						new Vector3(boardPiece.transform.position.x + HALF_PIECE_SIZE, boardPiece.transform.position.y - HALF_PIECE_SIZE));
				}
			}

			for(int i = 0; i < BOARD_HEIGHT; i++)
			{
				for(int j = 0; j < BOARD_WIDTH; j++)
				{
					Debug.DrawLine(new Vector3(j * PIECE_SIZE, i * PIECE_SIZE), new Vector3(j * PIECE_SIZE + PIECE_SIZE, i * PIECE_SIZE), Color.green);
					Debug.DrawLine(new Vector3(j * PIECE_SIZE, i * PIECE_SIZE + PIECE_SIZE), new Vector3(j * PIECE_SIZE + PIECE_SIZE, i * PIECE_SIZE + PIECE_SIZE), Color.green);
					Debug.DrawLine(new Vector3(j * PIECE_SIZE, i * PIECE_SIZE), new Vector3(j * PIECE_SIZE, i * PIECE_SIZE + PIECE_SIZE), Color.green);
					Debug.DrawLine(new Vector3(j * PIECE_SIZE + PIECE_SIZE, i * PIECE_SIZE), new Vector3(j * PIECE_SIZE + PIECE_SIZE, i * PIECE_SIZE + PIECE_SIZE), Color.green);
				}
			}
		}
	}

	private void StartGame() 
	{
		_started = true;
		SpawnNextUpPiece();
		MoveNextPieceToTop();
	}

	public void GameOver()
	{
		if(Score > HighScoresUtil.TetrisHighScore)
		{
			HighScoresUtil.TetrisHighScore = Score;
		}

		_gameOverText.SetActive(true);
		_gameOver = true;
		Debug.Log("Game Over");
		DestroyCurrentPiece();
		DestroyNextUpPiece();
	}

	private void SpawnNextUpPiece() 
	{
		_nextPiece = GameObject.Instantiate(GetRandomPiece()).GetComponent<TetrisPiece>();
		_nextPiece.transform.position = new Vector3(BOARD_WIDTH * PIECE_SIZE - PIECE_SIZE, BOARD_HEIGHT * PIECE_SIZE + PIECE_SIZE);
		_nextPiece.name = "Next Piece";
	}

	private void MoveNextPieceToTop() 
	{
		_currentPiece = _nextPiece;
		_currentPiece.transform.position = new Vector3(BOARD_WIDTH/2 * PIECE_SIZE - PIECE_SIZE, BOARD_HEIGHT * PIECE_SIZE - (3 * PIECE_SIZE));
		_currentPiece.name = "Current Piece";

		_controller.CurrentPiece = _currentPiece;

		var descendingPiece = _currentPiece.gameObject.AddComponent<DescendingPiece>();
		descendingPiece.SecondsBetweenDrops = _secondsBetweenVerticalDrops;

		SpawnNextUpPiece();
	}

	private void DestroyCurrentPiece()
	{
		GameObject.Destroy(_currentPiece.gameObject);
		_controller.CurrentPiece = null;
	}

	private void DestroyNextUpPiece()
	{
		GameObject.Destroy(_nextPiece);
	}

	private GameObject GetRandomPiece() 
	{
		return _piecePrefabs[Random.Range(0, _piecePrefabs.Length)];
	}

	public bool CheckAndHandleCollisions(float xMove, float yMove, bool lockPieceOnCollision = true) 
	{
		if(_currentPiece != null)
		{
			for(int i = 0; i < _currentPiece.transform.childCount; i++)
			{
				var block = _currentPiece.transform.GetChild(i);
				if(OutOfBoardBounds(block.position, xMove, yMove) || BlockIsCollidingWithGrid(block.position, xMove, yMove))
				{
					if(lockPieceOnCollision)
					{
						SnapCurrentPieceInPlace();	
					}
					return true;
				}
			}
		}

		return false;
	}

	public bool CheckRotateCollisions(bool[] proposedLayout, Vector3 anchorPoint)
	{
		for(int i = 0; i < 4; i++)
		{
			for(int j = 0; j < 4; j++)
			{
				var blockPosition = anchorPoint + new Vector3(j * TetrisGame.PIECE_SIZE, i * TetrisGame.PIECE_SIZE);
				if(proposedLayout[i * 4 + j] && (OutOfBoardBounds(blockPosition) || BlockIsCollidingWithGrid(blockPosition)))
				{
					return true;
				}
			}
		}

		return false;
	}

	private bool OutOfBoardBounds(Vector3 blockPosition, float xMove = 0, float yMove = 0)
	{
		var minXBound = blockPosition.x - TetrisGame.HALF_PIECE_SIZE + xMove;
		var maxXBound = blockPosition.x + TetrisGame.HALF_PIECE_SIZE + xMove;
		return blockPosition.y + yMove - HALF_PIECE_SIZE + EPSILON < 0 ||
			minXBound + EPSILON < 0 || maxXBound - EPSILON > (TetrisGame.BOARD_WIDTH * TetrisGame.PIECE_SIZE);
	}

	private bool BlockIsCollidingWithGrid(Vector3 blockPosition, float xOffset = 0, float yOffset = 0)
	{
		for(int i = 0; i < BOARD_WIDTH; i++)
		{
			for(int j = 0; j < BOARD_HEIGHT; j++)
			{
				if(_board[i,j] != null)
				{
					var xDiff = Mathf.Abs(blockPosition.x + xOffset - _board[i,j].transform.position.x) + EPSILON;
					var yDiff = Mathf.Abs(blockPosition.y + yOffset - _board[i,j].transform.position.y) + EPSILON;
					if(xDiff < PIECE_SIZE && yDiff < PIECE_SIZE)
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	private void SnapCurrentPieceInPlace() 
	{
		for(int i = 0; i < _currentPiece.transform.childCount; i++)
		{
			var child = _currentPiece.transform.GetChild(i);
			SnapBlockToClosestGridLocation(child);
			i--;
		}
			
		DestroyCurrentPiece();
		MoveNextPieceToTop();
		ClearFullRows();
	}

	private void SnapBlockToClosestGridLocation(Transform block) 
	{
		int xIndex = (int) ((block.position.x / PIECE_SIZE));
		int yIndex = (int) ((block.position.y / PIECE_SIZE));
		block.position = new Vector3(xIndex * PIECE_SIZE + HALF_PIECE_SIZE, yIndex * PIECE_SIZE + HALF_PIECE_SIZE);
		_board[xIndex,yIndex] = block.gameObject;
		block.parent = _gridHeirarchy[xIndex, yIndex].transform;
	}

	private void ClearFullRows()
	{
		for(int row = 0; row < BOARD_HEIGHT; row++)
		{
			var fullRow = true;
			for(int col = 0; col < BOARD_WIDTH; col++)
			{
				if(_board[col, row] == null)
				{
					fullRow = false;
					break;
				}
			}

			if(fullRow)
			{
				ClearRow(row);
				MoveBoardDown(row);
			}
		}
	}

	private void ClearRow(int row)
	{
		for(int i = 0; i < BOARD_WIDTH; i++)
		{
			var block = _board[i, row];
			GameObject.Destroy(block);
			_board[i, row] = null;
		}

		Score +=  BOARD_WIDTH * 100;
	}

	private void MoveBoardDown(int clearedRow)
	{
		for(int row = clearedRow + 1; row < BOARD_HEIGHT; row++)
		{
			for(int col = 0; col < BOARD_WIDTH; col++)
			{
				var block = _board[col, row];
				if(block != null)
				{
					_board[col, row] = null;
					_board[col, row - 1] = block;
					block.transform.parent = _gridHeirarchy[col, row - 1].transform;
					block.transform.position -= new Vector3(0, PIECE_SIZE);
				}
			}
		}
	}

	private void CreateGridHeirarchyObjects()
	{
		var grid = new GameObject("Grid");
		for(int i = 0; i < BOARD_WIDTH; i++)
		{
			var row = new GameObject("Row " + (i + 1));
			row.transform.parent = grid.transform;
			for(int j = 0; j < BOARD_HEIGHT; j++)
			{
				var column = new GameObject("Column " + (j + 1));
				column.transform.parent = row.transform;
				_gridHeirarchy[i, j] = column;
			}
		}
	}
}
