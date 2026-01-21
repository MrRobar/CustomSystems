using DG.Tweening;
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

        public Cell CurrentCell => _currentCell;
        public CanvasGroup CanvasGroup => _canvasGroup;
        public RectTransform RectTransform => _rectTransform;
        public PieceType PieceType => _pieceType;
        public int Level => _level;

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
            _canvasGroup.blocksRaycasts = blockRaycasts;
        }

        public void ResetCurrentCell()
        {
            _currentCell = null;
        }

        public Tween PlayMergeAnim(
            float peakScale = 1.6f,
            float upTime = 0.12f,
            float downTime = 0.1f)
        {
            transform.localScale = Vector3.one;

            return transform
                .DOScale(Vector3.one * peakScale, upTime)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    transform
                        .DOScale(Vector3.one, downTime)
                        .SetEase(Ease.OutBack);
                });
        }
    }
}