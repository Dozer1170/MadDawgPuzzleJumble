using UnityEngine;
using System.Collections;

public class Timer 
{
	private float _startTime = 0f;
	private float _timerLength = 0f;

	public Timer(float timerLength)
	{
		_timerLength = timerLength;
	}

	public void Start()
	{
		_startTime = Time.time;
	}

    public float CurrentTime 
    {
        get 
        {
            return Time.time - _startTime;
        }
    }

	public bool IsExpired()
	{
		return Time.time - _startTime > _timerLength;
	}
}
