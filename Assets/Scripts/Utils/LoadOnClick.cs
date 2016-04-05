using UnityEngine;
using System.Collections;

public class LoadOnClick : MonoBehaviour 
{
    public LoadingImage loadingImage;

    public void LoadScene(string level)
    {
        loadingImage.BeginTransition(level);
    }
}