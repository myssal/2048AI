using UnityEngine;
using UnityEngine.SceneManagement;
public class NewBehaviourScript : MonoBehaviour
{
    public void QuitApp()
	{
		Application.Quit();
		Debug.Log("Quit");
	}

	public void NavigateToScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
		Debug.Log("Navigating to " + sceneName);
	}
}
