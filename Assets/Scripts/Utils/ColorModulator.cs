using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorModulator : MonoBehaviour
{
    private int _currentColor;
    private Color _color;
    private bool _ascend;
    [SerializeField]
    private float _modulationRate = 0.05f;
    private Text _text;
    private SpriteRenderer _sprite;

    // Use this for initialization
    void Start()
    {
        _color = Color.green;
        _ascend = true;
        _currentColor = 2;

        _text = gameObject.GetComponent<Text>();
        _sprite = gameObject.GetComponent<SpriteRenderer>();
    }
	
    // Update is called once per frame
    void Update()
    {
        float modulateAmount = _ascend ? _modulationRate : -_modulationRate;
        switch (_currentColor)
        {
            case 0:
                _color.r += modulateAmount;
                if(_color.r >= 1f)
                {
                    _ascend = false;
                    _currentColor = 2;
                }
                else if(_color.r <= 0f)
                {
                    _ascend = true;
                    _currentColor = 2;
                }
                break;
            case 1:
                _color.g += modulateAmount;
                if(_color.g >= 1f)
                {
                    _ascend = false;
                    _currentColor = 0;
                }
                else if(_color.g <= 0f)
                {
                    _ascend = true;
                    _currentColor = 0;
                }
                break;
            case 2:
                _color.b += modulateAmount;
                if(_color.b >= 1f)
                {
                    _ascend = false;
                    _currentColor = 1;
                }
                else if(_color.b <= 0f)
                {
                    _ascend = true;
                    _currentColor = 1;
                }
                break;
            default:
                break;
        }

        if(_text != null)
            _text.color = _color;

        if(_sprite != null)
            _sprite.color = _color;
    }
}
