using UnityEngine;
using System.Collections;

public class TetrisGame : MonoBehaviour 
{
	public const float PIECE_SIZE = 0.32f;
	public const float HALF_PIECE_SIZE = 0.16f;
	public const int BOARD_WIDTH = 10, BOARD_HEIGHT = 15;
	public const float EPSILON = 0.01f;

	public static TetrisGame Instance;

	[SerializeField]
	private GameObject[] _piecePrefabs;
	[SerializeField]
	private Transform _nextPieceSpawnTransform;
	[SerializeField]
	private float _secondsBetweenVerticalDrops;
	[SerializeField]
	private TetrisPlayerController _controller;
	[SerializeField]
	private bool _debug;

	private GameObject[,] _board;
	private GameObject[,] _gridHeirarchy;
	private bool _started;
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
		}
	}

	private void StartGame() 
	{
		_started = true;
		SpawnNextUpPiece();
		MoveNextPieceToTop();
	}

	private void SpawnNextUpPiece() 
	{
		_nextPiece = GameObject.Instantiate(GetRandomPiece()).GetComponent<TetrisPiece>();
		_nextPiece.transform.position = _nextPieceSpawnTransform.position;
		_nextPiece.name = "Next Piece";
	}

	private void MoveNextPieceToTop() 
	{
		_currentPiece = _nextPiece;
		_currentPiece.transform.position = new Vector3(BOARD_WIDTH/2 * PIECE_SIZE, BOARD_HEIGHT * PIECE_SIZE);
		_currentPiece.name = "Current Piece";

		_controller.CurrentPiece = _currentPiece;

		var descendingPiece = _currentPiece.gameObject.AddComponent<DescendingPiece>();
		descendingPiece.SecondsBetweenDrops = _secondsBetweenVerticalDrops;

		SpawnNextUpPiece();
	}

	private void DestroyCurrentPiece()
	{
		_controller.CurrentPiece = null;
		GameObject.Destroy(_currentPiece);
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
		return blockPosition.y + yMove - HALF_PIECE_SIZE < 0 ||
			minXBound < 0 || maxXBound > (TetrisGame.BOARD_WIDTH * TetrisGame.PIECE_SIZE);
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
	}

	private void SnapBlockToClosestGridLocation(Transform block) 
	{
		int xIndex = (int) ((block.position.x / PIECE_SIZE) + HALF_PIECE_SIZE);
		int yIndex = (int) ((block.position.y / PIECE_SIZE) + HALF_PIECE_SIZE);
		block.position = new Vector3(xIndex * PIECE_SIZE, yIndex * PIECE_SIZE);
		_board[xIndex,yIndex] = block.gameObject;
		block.parent = _gridHeirarchy[xIndex, yIndex].transform;
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
