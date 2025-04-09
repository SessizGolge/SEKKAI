using System.Collections;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactionRange = 2f;  // Etkileşim mesafesi
    public KeyCode interactKey = KeyCode.E; // Etkileşim tuşu
    [SerializeField] ObjectController objectController;
    [SerializeField] public GameObject crosshair;
    InteractableObject interactable;

    public LayerMask interactionLayer; // Etkileşim yapılabilir katman (Interaction Layer)

    public Transform circleOrigin; // Etkileşim alanının merkezi
    public float radius; // Etkileşim yarıçapı

    void Update()
    {
        if (Input.GetKeyDown(interactKey) && !objectController.isOpened) 
        {
            CheckForInteraction();
        }
        else if (Input.GetKeyDown(interactKey) && objectController.isOpened) 
        {
            objectController.isOpened = false;
        }
    }

    private void CheckForInteraction()
    {
        // Kılıç çevresindeki etkileşim yapılabilir nesneleri kontrol et
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position, radius, interactionLayer))
        {
            // Eğer nesne etkileşim yapılabilirse
            interactable = collider.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    // Gizmos kullanarak etkileşim alanını görselleştirebiliriz
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(circleOrigin.position, radius);
    }
}
