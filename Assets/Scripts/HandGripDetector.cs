using System;
using UnityEngine;
using Leap;
//using Leap.Unity;

public enum HandGripState
{
    Free,
    Grabbing
}

public class HandGripDetector : MonoBehaviour
{
    [Header("Leap Provider")]
    public LeapServiceProvider leapProvider;

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

    private void Reset()
    {
        if (leapProvider == null)
        {
            leapProvider = FindObjectOfType<LeapServiceProvider>();
        }
    }

    private void Update()
    {
        if (leapProvider == null) return;

        Frame frame = leapProvider.CurrentFrame;
        if (frame == null) return;

        Hand leftHand = null;
        Hand rightHand = null;

        foreach (var hand in frame.Hands)
        {
            if (hand.IsLeft) leftHand = hand;
            if (hand.IsRight) rightHand = hand;
        }

        // L
        if (leftHand != null)
        {
            leftGrabStrength = leftHand.GrabStrength;
            UpdateHandState(true, leftGrabStrength, ref leftState, OnLeftGrabStart, OnLeftGrabEnd);
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

        // R
        if (rightHand != null)
        {
            rightGrabStrength = rightHand.GrabStrength;
            UpdateHandState(false, rightGrabStrength, ref rightState, OnRightGrabStart, OnRightGrabEnd);
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

    private void UpdateHandState(
        bool isLeft,
        float grabStrength,
        ref HandGripState state,
        Action grabStartEvent,
        Action grabEndEvent)
    {
        switch (state)
        {
            case HandGripState.Free:
                if (grabStrength >= grabThreshold)
                {
                    state = HandGripState.Grabbing;
                    grabStartEvent?.Invoke();
                }
                break;

            case HandGripState.Grabbing:
                if (grabStrength <= releaseThreshold)
                {
                    state = HandGripState.Free;
                    grabEndEvent?.Invoke();
                }
                break;
        }
    }
}
