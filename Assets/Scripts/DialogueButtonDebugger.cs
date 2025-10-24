using UnityEngine;
using UnityEngine.UI;

public class DialogueButtonDebugger : MonoBehaviour
{
    private Button continueButton;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DebugDialogueButton();
        }
    }

    private void DebugDialogueButton()
    {
        var dialogueView = FindObjectOfType<DialogueView>();
        if (dialogueView == null)
        {
            Debug.Log("❌ DialogueView not found");
            return;
        }

        continueButton = dialogueView.GetComponentInChildren<Button>();
        if (continueButton == null)
        {
            Debug.Log("❌ Continue button not found");
            return;
        }

        Debug.Log("=== DIALOGUE BUTTON DEBUG ===");
        Debug.Log($"Button: {continueButton.name}");
        Debug.Log($"Interactable: {continueButton.interactable}");
        Debug.Log($"DialogueManager.IsDialogueActive: {DialogueManager.IsDialogueActive}");
        Debug.Log($"Time.timeScale: {Time.timeScale}");
        Debug.Log($"AudioListener.pause: {AudioListener.pause}");
        Debug.Log("============================");
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 50, 300, 20), "Press 'D' to debug dialogue button");
    }
}