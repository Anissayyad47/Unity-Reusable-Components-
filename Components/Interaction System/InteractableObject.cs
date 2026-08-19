using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("UI Reference")]
    [SerializeField] private GameObject UI_Interact;
    
    public void InteractEnter(GameObject obj = null)
    {
        UI_Interact.SetActive(true);
    }
    public void Interact(GameObject obj = null)
    {
        // Action or Work
    }
    public void InteractExite()
    {
        UI_Interact.SetActive(false);
    }
}
