using UnityEngine;
using System.Collections;

public class TetrisPlayerController : MonoBehaviour 
{
	public TetrisPiece CurrentPiece { get; set; }
	private Timer _moveDownTimer = new Timer(0.1f);
	private Timer _moveSideTimer = new Timer(0.1f);

	// Update is called once per frame
	void Update () 
	{
		if(CurrentPiece != null)
		{
			var deltaMovement = GetDeltaMovement();
			if(_moveSideTimer.IsExpired() && ValidHorizontalMove(deltaMovement.x))
			{
				CurrentPiece.transform.position += new Vector3(deltaMovement.x, 0);
				_moveSideTimer.Start();
			}

			if(_moveDownTimer.IsExpired() && ValidVerticalMove(deltaMovement.y))
			{
				CurrentPiece.transform.position += new Vector3(0, deltaMovement.y);
				_moveDownTimer.Start();
			}

			if(Input.GetKeyDown(KeyCode.Space))
			{
				CurrentPiece.TryRotate();
			}
		}
	}

	public bool DidTouchScreen() 
	{
		return (Application.platform != RuntimePlatform.IPhonePlayer && Input.GetMouseButtonDown(0)) ||
			Input.touches.Length > 0;
	}

	private Vector2 GetDeltaMovement()
	{
		Vector2 rval = new Vector2(0, 0);
		
		var left = Input.GetKey(KeyCode.LeftArrow);
		var right = Input.GetKey(KeyCode.RightArrow);
		rval.x = left ? -1 : right ? 1 : 0;
		
        if(Input.touches.Length > 0)
		{
            rval = CurrentPiece.transform.position - new Vector3(Input.touches[0].position.x, Input.touches[0].position.y);
		}

		rval = new Vector2(rval.x > 0f ? TetrisGame.PIECE_SIZE : rval.x < 0f ? -TetrisGame.PIECE_SIZE : 0f, 0);
		rval.y = Input.GetKey(KeyCode.DownArrow) ? -TetrisGame.PIECE_SIZE : 0;

		return rval;
	}

	private bool ValidHorizontalMove(float xDelta)
	{
		var isValidMove = !TetrisGame.Instance.CheckAndHandleCollisions(xDelta, 0, false);
		return isValidMove;
	}

	private bool ValidVerticalMove(float yDelta)
	{
		var isValidMove = !TetrisGame.Instance.CheckAndHandleCollisions(0, yDelta, false);
		return isValidMove;
	}
}
