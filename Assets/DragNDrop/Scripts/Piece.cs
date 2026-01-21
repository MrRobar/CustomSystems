using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DragNDrop.Scripts
{
    public class Piece : MonoBehaviour
    {
        [SerializeField] private PieceType _pieceType;
        [SerializeField] private int _level = 1;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        private Cell _currentCell;

        public void Init(PieceType t, int lvl)
        {
            _pieceType = t;
            _level = lvl;
            _image.color = GetColor(_pieceType);
            _levelText.text = _level.ToString();
        }

        private Color GetColor(PieceType type)
        {
            return type switch
            {
                PieceType.Blue => Color.blue,
                PieceType.Green => Color.green,
                _ => Color.red,
            };
        }

        public void UpdatePiece(Cell cell, Transform parent, Vector2 position, bool blockRaycasts)
        {
            _currentCell = cell;
            transform.SetParent(parent, false);
            _rectTransform.anchoredPosition = position;
            _canvasGroup.blocksRaycasts = blockRaycasts; // Test
        }

        public void ResetCurrentCell()
        {
            _currentCell = null;
        }
    }
}