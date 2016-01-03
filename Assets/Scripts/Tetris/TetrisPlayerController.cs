using UnityEngine;
using System.Collections;

public class TetrisPlayerController : MonoBehaviour 
{
	public TetrisPiece CurrentPiece { get; set; }

	// Update is called once per frame
	void Update () 
	{
		if(CurrentPiece != null)
		{
			var deltaMovement = new Vector3(GetDeltaMovement().x, 0);
			if(deltaMovement.x != 0 && ValidMove(deltaMovement.x))
			{
				CurrentPiece.transform.position += deltaMovement;
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
		if(Application.platform != RuntimePlatform.IPhonePlayer)
		{
			rval = CursorUtils.MouseDeltaPos;
		}
		else if(Input.touches.Length > 0)
		{
			rval = Input.touches[0].deltaPosition;
		}

		rval = new Vector2(rval.x > 0f ? TetrisGame.PIECE_SIZE : rval.x < 0f ? -TetrisGame.PIECE_SIZE : 0f, 0);

		return rval;
	}

	private bool ValidMove(float xDelta)
	{
		var isValidMove = !TetrisGame.Instance.CheckAndHandleCollisions(xDelta, 0, false);
		return isValidMove;
	}
}
