using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Interface that is inherited by any gameObject that can be interacted with the player
/// </summary>
public interface IInteractable
{
    public void Interact();

    public void ShowUI();
    public void HideUI();

}
