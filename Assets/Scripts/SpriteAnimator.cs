//This class maintains a collection of sprite objects as frames of animation
//It shows and hides those frames according to a set of playback settings
using UnityEngine;
using System.Collections;

public class SpriteAnimator : MonoBehaviour
{
    //Playback types - run once or loop forever
    public enum AnimatorPlaybackType { Playonce = 0, Playloop = 1 };

    //Playback type for this animation
    public AnimatorPlaybackType PlaybackType = AnimatorPlaybackType.Playonce;

    //Frames per second to play for this animation
    public int FPS = 5;

    //Custom ID for animation - used with function PlayObjectAnimation
    public int AnimationId = 0;

    //Frames of animation
    public SpriteRenderer[] SpriteRenderers = null;

    //Should auto-play?
    public bool AutoPlay = false;

    //Should first hide all sprite renderers on playback? or leave at defaults
    public bool HideSpritesOnStart = true;

    //Boolean indicating whether animation is currently playing
    bool isPlaying = false;

    void Start()
    {
        //Should we auto-play at start up?
        if (AutoPlay) StartCoroutine(PlaySpriteAnimation(AnimationId));
    }
    
    //Function to run animation
    public IEnumerator PlaySpriteAnimation(int animId = 0)
    {
        //Check if this animation should be started
        if (animId != AnimationId) yield break;

        //Should hide all sprite renderers?
        if (HideSpritesOnStart) foreach (SpriteRenderer spriteRenderer in SpriteRenderers) spriteRenderer.enabled = false;

        //Set is playing
        isPlaying = true;

        //Calculate delay time
        float delayTime = 1.0f / FPS;

        bool playOnce = AnimatorPlaybackType.Playonce == PlaybackType;

        if (playOnce) yield return StartCoroutine(PlayAnimation(delayTime));
        else while (true) yield return StartCoroutine(PlayAnimation(delayTime));

        //Stop animation
        StopSpriteAnimation(AnimationId);
    }

    private IEnumerator PlayAnimation(float delayTime)
    {
        foreach (SpriteRenderer spriteRenderer in SpriteRenderers)
        {
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(delayTime);
            spriteRenderer.enabled = false;
        }
    }

    //Function to stop animation
    public void StopSpriteAnimation(int animId = 0)
    {
        //Check if this animation can and should be stopped
        if ((animId != AnimationId) || !isPlaying) return;

        //Stop all coroutines (animation will no longer play)
        StopAllCoroutines();

        //Is playing false
        isPlaying = false;

        //Send Sprite Animation stopped event to gameobject
        gameObject.SendMessage("SpriteAnimationStopped", animId, SendMessageOptions.DontRequireReceiver);
    }
}
