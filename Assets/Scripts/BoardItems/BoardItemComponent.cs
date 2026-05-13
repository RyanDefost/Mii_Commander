using UnityEngine;

public class BoardItemComponent : MonoBehaviour
{
    [SerializeField, HideInInspector]
    protected BoardItem boardItem;

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;
        this.boardItem = GetComponent<BoardItem>();
    }

    public virtual void ConnectToBoardItem() { }
}