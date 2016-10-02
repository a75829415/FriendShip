using UnityEngine;
using UnityEngine.SceneManagement;

public class Preparation : MonoBehaviour
{
    public GameObject[] crossSceneObjectPrefabs;

    void Awake()
    {
        foreach(GameObject obj in crossSceneObjectPrefabs)
        {
            DontDestroyOnLoad(Instantiate(obj));
        }
    }

    void Start()
    {
        SceneManager.LoadScene(1);
    }
}
