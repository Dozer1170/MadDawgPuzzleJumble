using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorLerper : MonoBehaviour 
{
	private	Text _text;
	[SerializeField]
	private Color _colorOne;
	[SerializeField]
	private Color _colorTwo;

	// Use this for initialization
	void Start () 
	{
		_text = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(_text != null)
		{
			_text.color = Color.Lerp(_colorOne, _colorTwo,  Mathf.PingPong(Time.time, 1));
		}
	}
}
