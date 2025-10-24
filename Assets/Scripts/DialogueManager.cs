using UnityEngine;
using System.Collections;
using UnityEngine.Playables;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static bool IsDialogueActive { get; private set; }

    [SerializeField] private DialogueView dialogueView;
    [SerializeField] private PlayableDirector playableDirector;

    public UnityAction OnDialogueEnd;

    private DialogueNode currentNode;
    private PuzzleInput[] allPuzzleInputs;
    private bool isWaitingForBattle = false;
    private bool isTransitioning = false;

    private void Start()
    {
        if (dialogueView != null)
        {
            dialogueView.OnContinueClick += OnContinueClicked;
            dialogueView.Hide();
        }
        
        IsDialogueActive = false;
        FindAllPuzzleInputs();
    }
    
    private void FindAllPuzzleInputs()
    {
        allPuzzleInputs = FindObjectsOfType<PuzzleInput>(true);
    }

    private void OnDestroy()
    {
        if (dialogueView != null)
        {
            dialogueView.OnContinueClick -= OnContinueClicked;
        }
    }

    private void Update()
    {
        if (!IsDialogueActive || dialogueView == null || currentNode == null) return;

        bool shouldBeLocked = ShouldButtonBeLockedNow();

        if (shouldBeLocked && dialogueView.IsButtonInteractable())
        {
            dialogueView.ForceDisableButton();
            Debug.LogWarning("⚠️ Кнопка принудительно заблокирована в Update!");
        }
        else if (!shouldBeLocked && !dialogueView.IsButtonInteractable() && !dialogueView.IsProcessingClick())
        {
            dialogueView.SetContinueInteractable(true);
            Debug.Log("✅ Кнопка разблокирована в Update.");
        }
    }

    private bool ShouldButtonBeLockedNow()
    {
        if (currentNode == null) return true;
        if (isTransitioning) return true;

        if (FocusManager.Instance != null && !FocusManager.Instance.IsApplicationFocused)
        {
            return true;
        }

        if (currentNode.blockUntilCutsceneEnd && 
            playableDirector != null && 
            playableDirector.state == PlayState.Playing)
        {
            return true;
        }

        return false;
    }

    public void StartDialogue(DialogueNode node)
    {
        if (node == null || IsDialogueActive) return;
        
        IsDialogueActive = true; 
        isTransitioning = true;
        DisableAllPuzzleInputs();
        RunNode(node);
    }
    
    private void DisableAllPuzzleInputs()
    {
        if (allPuzzleInputs == null || allPuzzleInputs.Length == 0) FindAllPuzzleInputs();
        foreach (var input in allPuzzleInputs)
        {
            if (input != null && input.enabled) input.enabled = false;
        }
    }

    private void RunNode(DialogueNode node)
    {
        if (node == null) return;
        
        currentNode = node;
        isTransitioning = true;

        if (node.resumeCutsceneOnStart && playableDirector != null && playableDirector.state != PlayState.Playing)
        {
            playableDirector.Play();
        }

        if (dialogueView == null) return;
        
        dialogueView.Show();
        dialogueView.SetContent(node);

        if (node.voiceClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVoice(node.voiceClip);
        }
        
        dialogueView.ForceDisableButton();
        ExecuteNodeLogic(node);
    }

    private void ExecuteNodeLogic(DialogueNode node)
    {
        if (node == null) return;

        node.Execute(this, () =>
        {
            isTransitioning = false;
        });
    }

    private void OnContinueClicked()
    {
        if (currentNode == null || !IsDialogueActive || isTransitioning) return;
        
        isTransitioning = true;
        dialogueView.SetContinueInteractable(false);

        if (currentNode.pauseCutsceneAtEnd && playableDirector != null)
        {
            playableDirector.Pause();
        }
        
        if (currentNode is TriggerBattleNode battleNode && !isWaitingForBattle)
        {
            isWaitingForBattle = true;
            battleNode.StartBattle(this, () => 
            { 
                isWaitingForBattle = false; 
                ContinueAfterBattle(); 
            });
            return;
        }
        
        if (currentNode.isLast || currentNode.nextNode == null)
        {
            EndDialogue();
        }
        else
        {
            StartCoroutine(TransitionToNextNode(currentNode.nextNode));
        }
    }

    private IEnumerator TransitionToNextNode(DialogueNode nextNode)
    {
        yield return new WaitForSeconds(0.1f);
        RunNode(nextNode);
    }

    private void ContinueAfterBattle()
    {
        if (dialogueView != null) dialogueView.Show();
        
        if (currentNode != null && currentNode.nextNode != null)
        {
            StartCoroutine(TransitionToNextNode(currentNode.nextNode));
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        IsDialogueActive = false; 
        isTransitioning = false;
        currentNode = null;
        
        if (dialogueView != null) dialogueView.Hide();
        
        OnDialogueEnd?.Invoke();
    }
    
    public PlayableDirector GetDirector() => playableDirector;
}