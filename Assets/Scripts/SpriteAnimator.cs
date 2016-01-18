﻿//This class maintains a collection of sprite objects as frames of animation
//It shows and hides those frames according to a set of playback settings
using UnityEngine;
using System.Collections;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] private bool playOnce = true;

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
    private float delayTime;

    void Start()
    {
        delayTime = 1.0f / FPS;

        //Should we auto-play at start up?
        if (AutoPlay) StartCoroutine(PlaySpriteAnimation(AnimationId));
    }
    
    //Function to run animation
    public IEnumerator PlaySpriteAnimation(int animId = 0)
    {
        if (!ShouldPlay(animId)) yield break;

        Init();

        if (playOnce) yield return StartCoroutine(LoopThroughSprites());
        else while (true) yield return StartCoroutine(LoopThroughSprites());

        StopSpriteAnimation(AnimationId);
    }

    private void Init( )
    {
        if (HideSpritesOnStart) HideAllSprites();

        //Set is playing
        isPlaying = true;
    }

    private bool ShouldPlay(int animId)
    {
        return animId == AnimationId;
    }

    private void HideAllSprites()
    {
        foreach (SpriteRenderer spriteRenderer in SpriteRenderers) spriteRenderer.enabled = false;
    }

    private IEnumerator LoopThroughSprites()
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
