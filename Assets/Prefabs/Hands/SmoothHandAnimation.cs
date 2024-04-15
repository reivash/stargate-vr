using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandsController : MonoBehaviour
{
    [SerializeField] private Animator _handAnimator;
    [SerializeField] private InputActionReference _triggerActionReference;
    [SerializeField] private InputActionReference _gripActionReference;

    private static readonly int TriggerAnimation = Animator.StringToHash("Trigger");
    private static readonly int GripAnimation = Animator.StringToHash("Grip");

    private void Update() {
        float triggerValue = _triggerActionReference.action.ReadValue<float>();
        float gripValue = _gripActionReference.action.ReadValue<float>();
        _handAnimator.SetFloat(TriggerAnimation, triggerValue);
        _handAnimator.SetFloat(GripAnimation, gripValue);
    }
}
