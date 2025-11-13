using UnityEngine;
using System;

using Leap;
//using Leap.Unity;

public class HandGripDetector : MonoBehaviour
{
    [Header("Leap Provider")]
    public LeapServiceProvider leapProvider;   // 在 Inspector 里拖入你的 LeapServiceProvider

    [Header("Thresholds")]
    [Range(0f, 1f)] public float grabThreshold = 0.7f;
    [Range(0f, 1f)] public float releaseThreshold = 0.5f;

    [Header("Debug")]
    public HandGripState leftState = HandGripState.Free;
    public HandGripState rightState = HandGripState.Free;

    public float leftGrabStrength;
    public float rightGrabStrength;

    
    public event Action OnLeftGrabStart;
    public event Action OnLeftGrabEnd;
    public event Action OnRightGrabStart;
    public event Action OnRightGrabEnd;

    void Reset()
    {
        
        if (leapProvider == null)
        {
            leapProvider = FindObjectOfType<LeapServiceProvider>();
        }
    }

    void Update()
    {
        if (leapProvider == null) return;

        Frame frame = leapProvider.CurrentFrame;
        if (frame == null) return;

        Hand leftHand = null;
        Hand rightHand = null;

        foreach (var hand in frame.Hands)
        {
            if (hand.IsLeft) leftHand = hand;
            else if (hand.IsRight) rightHand = hand;
        }

        // left hand
        if (leftHand != null)
        {
            leftGrabStrength = leftHand.GrabStrength;
            UpdateHandState(
                isLeft: true,
                grabStrength: leftGrabStrength,
                ref leftState,
                OnLeftGrabStart,
                OnLeftGrabEnd
            );
        }
        else
        {
            
            if (leftState == HandGripState.Grabbing)
            {
                leftState = HandGripState.Free;
                OnLeftGrabEnd?.Invoke();
            }
            leftGrabStrength = 0f;
        }

        // right hand
        if (rightHand != null)
        {
            rightGrabStrength = rightHand.GrabStrength;
            UpdateHandState(
                isLeft: false,
                grabStrength: rightGrabStrength,
                ref rightState,
                OnRightGrabStart,
                OnRightGrabEnd
            );
        }
        else
        {
            if (rightState == HandGripState.Grabbing)
            {
                rightState = HandGripState.Free;
                OnRightGrabEnd?.Invoke();
            }
            rightGrabStrength = 0f;
        }
    }

    void UpdateHandState(
        bool isLeft,
        float grabStrength,
        ref HandGripState state,
        Action grabStartEvent,
        Action grabEndEvent)
    {
        switch (state)
        {
            case HandGripState.Free:
                // Free → Grabbing
                if (grabStrength >= grabThreshold)
                {
                    state = HandGripState.Grabbing;
                    grabStartEvent?.Invoke();
                    // Debug.Log((isLeft ? "Left" : "Right") + " hand GRAB");
                }
                break;

            case HandGripState.Grabbing:
                // Grabbing → Free
                if (grabStrength <= releaseThreshold)
                {
                    state = HandGripState.Free;
                    grabEndEvent?.Invoke();
                    // Debug.Log((isLeft ? "Left" : "Right") + " hand RELEASE");
                }
                break;
        }
    }
}

public enum HandGripState
{
    Free,
    Grabbing
}
