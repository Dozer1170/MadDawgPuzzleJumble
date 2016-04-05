using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingImage : MonoBehaviour 
{
    public GameObject menuCanvas;

    public Vector3 rotateDiff = new Vector3(0f, 0f, 12f);
    public Vector3 scaleDiff = new Vector3(0.01f, 0.01f, 0f);

    private string _level;

    public void BeginTransition(string levelToLoad)
    {
        _level = levelToLoad;
        gameObject.SetActive(true);
        GameObject.Destroy(menuCanvas);
        StartCoroutine(DelayedLoadMenu());
    }

    void Update() 
    {
        if(gameObject.activeSelf)
        {
            gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.eulerAngles + rotateDiff);
            gameObject.transform.localScale += scaleDiff;
        }
    }

    IEnumerator DelayedLoadMenu()
    {
        yield return new WaitUntil(() => gameObject.transform.localScale.x >= 2);

        scaleDiff *= -1;
        SceneManager.LoadScene(_level);

        if(_level == "Tetris")
        {
            gameObject.transform.position = new Vector3(1.6f, 3.0f);
        }

        yield return new WaitUntil(() => gameObject.transform.localScale.x <= 0);

        GameObject.Destroy(gameObject);
    }
}
