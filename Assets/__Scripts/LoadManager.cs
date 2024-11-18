using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene(1);
    }
}
