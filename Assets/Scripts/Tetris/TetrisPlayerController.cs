using UnityEngine;
using System.Collections;

public class TetrisPlayerController : MonoBehaviour 
{
    public static float ScreenSpaceRatio = 216f;

	public TetrisPiece CurrentPiece { get; set; }
	private Timer _moveDownTimer = new Timer(0.1f);

    private Touch _lastTouch;
    private Vector2 _touchAnchor;

	// Update is called once per frame
	void Update () 
	{
		if(CurrentPiece != null)
		{
			var deltaMovement = GetDeltaMovement();
			if(ValidHorizontalMove(deltaMovement.x))
			{
				CurrentPiece.transform.position += new Vector3(deltaMovement.x, 0);
			}
                
            var hasTouches = Input.touches.Length > 0;
            if(hasTouches)
            {
                var currentTouch = Input.touches[0];
                if(currentTouch.phase == TouchPhase.Began)
                {
                    _touchAnchor = currentTouch.position;
                }


                if(currentTouch.phase == TouchPhase.Moved)
                {
                    var worldPosDiff = (currentTouch.position - _touchAnchor) / ScreenSpaceRatio;
                    var worldPos = CurrentPiece.transform.position + new Vector3(worldPosDiff.x + TetrisGame.HALF_PIECE_SIZE, worldPosDiff.y);
                    var rowIndex = (int) (worldPos.x / TetrisGame.PIECE_SIZE);
                    var snappedPositionX = rowIndex * TetrisGame.PIECE_SIZE;
                    var deltaX = Mathf.Clamp(snappedPositionX - CurrentPiece.transform.position.x, -TetrisGame.PIECE_SIZE, TetrisGame.PIECE_SIZE);
                    Debug.Log("WorldX: " + worldPos.x + " TouchX: " + currentTouch.position.x + " TouchAnchorX: " + _touchAnchor.x +
                        " DeltaX: " + deltaX + " SnappedPosX: " + snappedPositionX);
                    if(deltaX != 0 && ValidHorizontalMove(deltaX))
                    {
                        CurrentPiece.transform.position += new Vector3(deltaX, 0);
                        Debug.Log("Moved X " + deltaX);
                        _touchAnchor = currentTouch.position;
                    }
                }

                if(currentTouch.phase == TouchPhase.Began  && currentTouch.tapCount % 2 == 0)
                {
                    CurrentPiece.TryRotate();
                }
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
		
		var left = Input.GetKeyDown(KeyCode.LeftArrow);
		var right = Input.GetKeyDown(KeyCode.RightArrow);
		rval.x = left ? -1 : right ? 1 : 0;

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
