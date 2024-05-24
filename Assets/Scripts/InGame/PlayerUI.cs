using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private GameObject dragImage;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return; // 방장만 드래그 가능
        }

        // Create a drag image
        dragImage = new GameObject("DragImage");
        dragImage.transform.SetParent(transform.root, false);
        dragImage.transform.SetAsLastSibling();

        // Copy the original UI
        Image originalImage = GetComponentInChildren<Image>();
        Image image = dragImage.AddComponent<Image>();
        image.sprite = originalImage.sprite;
        image.rectTransform.sizeDelta = originalImage.rectTransform.sizeDelta;
        image.rectTransform.pivot = originalImage.rectTransform.pivot;
        image.transform.position = originalImage.transform.position;
        image.transform.localScale = originalImage.transform.localScale;

        // Create a Text for the drag image
        Text originalText = GetComponentInChildren<Text>();
        GameObject textObject = new GameObject("DragText");
        textObject.transform.SetParent(dragImage.transform, false);
        Text text = textObject.AddComponent<Text>();
        text.text = originalText.text;
        text.font = originalText.font;
        text.fontSize = originalText.fontSize;
        text.color = originalText.color;
        text.alignment = originalText.alignment;
        text.rectTransform.sizeDelta = originalText.rectTransform.sizeDelta;
        text.rectTransform.pivot = originalText.rectTransform.pivot;
        text.transform.position = originalText.transform.position;
        text.transform.localScale = originalText.transform.localScale;

        // Set the CanvasGroup for the drag image
        CanvasGroup dragCanvasGroup = dragImage.AddComponent<CanvasGroup>();
        dragCanvasGroup.alpha = 0.6f;
        dragCanvasGroup.blocksRaycasts = false;

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return; // 방장만 드래그 가능
        }

        if (dragImage != null)
        {
            dragImage.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return; // 방장만 드래그 가능
        }

        if (dragImage != null)
        {
            Destroy(dragImage);
        }

        canvasGroup.blocksRaycasts = true;

        if (eventData.pointerEnter != null)
        {
            TeamPanel teamPanel = eventData.pointerEnter.GetComponentInParent<TeamPanel>();
            if (teamPanel != null)
            {
                teamPanel.OnPlayerDropped(gameObject);
            }
        }
    }
}
