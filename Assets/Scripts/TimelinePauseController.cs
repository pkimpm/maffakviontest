using UnityEngine;
using UnityEngine.Playables;

public class TimelinePauseController : MonoBehaviour
{
    [Header("PlayableDirector для контроля")]
    [SerializeField] private PlayableDirector playableDirector;

    private bool shouldBePaused = false;

    public void ForcePauseTimeline()
    {
        if (playableDirector == null) return;
        playableDirector.Pause();
        shouldBePaused = true;
    }

    public void UnpauseTimelineFromSignal()
    {
        if (playableDirector == null) return;
        shouldBePaused = false;

        var offsetManager = playableDirector.GetComponent<TimelineOffsetManager>();
        if (offsetManager != null)
        {
            offsetManager.ResumeWithOffset();
        }
        else
        {
#if UNITY_2021_2_OR_NEWER
            playableDirector.Resume();
#else
            playableDirector.Play();
#endif
        }
    }

    private void Update()
    {
        if (shouldBePaused && playableDirector != null && playableDirector.state != PlayState.Paused)
        {
            playableDirector.Pause();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (shouldBePaused && playableDirector != null && playableDirector.state != PlayState.Paused)
        {
            playableDirector.Pause();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (shouldBePaused && playableDirector != null && playableDirector.state != PlayState.Paused)
        {
            playableDirector.Pause();
        }
    }
}