using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader
{
    public static SceneLoader Instance = null;

    public void AddScene(string name)
    { 
        SceneManager.LoadScene(name, LoadSceneMode.Additive);
    }
    
    public void LoadScene(string name) 
    { 
        SceneManager.LoadScene(name, LoadSceneMode.Single);
        UIManager.Instance.isStage = !UIManager.Instance.isStage;
        if (UIManager.Instance.isStage)
            UIManager.Instance.giveupButton.SetActive(UIManager.Instance.isStage);
    }

    public void RemoveScene(string name)
    {
        SceneManager.UnloadSceneAsync(name);
    }
}