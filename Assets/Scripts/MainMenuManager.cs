using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject instructionsPanel;

    private void Awake()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void ShowInstructions()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
        }
    }

    public void HideInstructions()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }
    }
}