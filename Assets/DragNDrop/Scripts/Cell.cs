using UnityEngine;

namespace DragNDrop.Scripts
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        public RectTransform Rect => _rectTransform;
        public Piece CurrentPiece { get; private set; }

        public bool IsEmpty() => CurrentPiece == null;

        public void SetPiece(Piece piece)
        {
            CurrentPiece = piece;
            piece.UpdatePiece(this, transform, Vector2.zero, true);
        }

        public void Clear()
        {
            if (CurrentPiece != null)
                CurrentPiece.ResetCurrentCell();
            CurrentPiece = null;
        }
    }
}