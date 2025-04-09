using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    protected ObjectController objectController;

    protected virtual void Start()
    {
        objectController = GetComponent<ObjectController>();
    }

    public virtual void Interact()
    {
        if (objectController == null) 
        {
            Debug.LogWarning("Objenin Controller'ı yok!");
            return;
        }

        if (!objectController.isOpened)
        {
            objectController.OpenChestUI();
            objectController.isOpened = true;
        }
        else
        {
            objectController.CloseChestUI();
            objectController.isOpened = false;
        }
    }
}
