using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    private IInteractable _IInteractable;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }
    
    private void Interact()
    {
        if(_IInteractable == null) return;
        _IInteractable.Interact();
    }

    private void OnTriggerEnter(Collider other)
    {
        _IInteractable = other.GetComponent<Comp_IInteractable>();
        if(_IInteractable != null)
        {
            Debug.Log("Interaction Collision");
            _IInteractable.InteractEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _IInteractable = other.GetComponent<Comp_IInteractable>();
        if(_IInteractable != null)
        {
            _IInteractable.InteractExite();
        }
    }
}
