using DG.Tweening;
using UnityEngine;

namespace DragNDrop.Scripts
{
    public class DragController : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GridManager _grid;

        private Piece _currentDraggable;
        private Cell _originCell;
        private bool _isDragging;
        private Vector2 _pointerScreen;

        private void Update()
        {
            _pointerScreen = GetPointerScreenPosition();

            if (!_isDragging)
            {
                if (PointerDown())
                {
                    TryStartDrag(_pointerScreen);
                }
            }
            else
            {
                FollowPointer(_pointerScreen);

                if (PointerUp())
                {
                    EndDrag(_pointerScreen);
                }
            }
        }

        private Vector2 GetPointerScreenPosition()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0) return Input.touches[0].position;
            else return Input.mousePosition;
#else
            return Input.mousePosition;
#endif
        }

        private bool PointerDown()
        {
#if UNITY_ANDROID || UNITY_IOS
            return Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began;
#else
            return Input.GetMouseButtonDown(0);
#endif
        }

        private bool PointerUp()
        {
#if UNITY_ANDROID || UNITY_IOS
            return Input.touchCount > 0 && (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled);
#else
            return Input.GetMouseButtonUp(0);
#endif
        }

        private void TryStartDrag(Vector2 screenPos)
        {
            var cell = _grid.GetCellUnderPointer(screenPos);
            if (cell != null && !cell.IsEmpty())
            {
                StartDragging(cell.CurrentPiece);
            }
        }

        private void StartDragging(Piece piece)
        {
            _currentDraggable = piece;
            _originCell = piece.CurrentCell;
            _isDragging = true;

            if (_originCell != null) _originCell.Clear();

            _currentDraggable.transform.SetParent(_grid.DragLayer, true);
            _currentDraggable.CanvasGroup.blocksRaycasts = false;

            _currentDraggable.transform.SetAsLastSibling();
            _currentDraggable.transform.DOKill();
            _currentDraggable.transform.DOScale(1.08f, 0.12f).SetEase(Ease.OutQuad);
        }

        private void FollowPointer(Vector2 screenPos)
        {
            if (_currentDraggable == null) return;

            var canvasRect = _canvas.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, _canvas.worldCamera,
                out var localPos);
            _currentDraggable.RectTransform.anchoredPosition = localPos;
        }

        private void EndDrag(Vector2 screenPos)
        {
            _isDragging = false;

            var targetCell = _grid.GetCellUnderPointer(screenPos);

            if (targetCell == null)
            {
                ReturnToOrigin();
                return;
            }

            if (targetCell.IsEmpty())
            {
                targetCell.SetPiece(_currentDraggable);
                _currentDraggable.transform.DOKill();
                _currentDraggable.transform.localScale = Vector3.one;
                _currentDraggable = null;
                _originCell = null;
                return;
            }

            var targetPiece = targetCell.CurrentPiece;
            if (targetPiece.PieceType == _currentDraggable.PieceType && targetPiece.Level == _currentDraggable.Level)
            {
                Merge(targetCell, targetPiece, _currentDraggable);
            }
            else
            {
                ReturnToOrigin();
            }
        }

        private void ReturnToOrigin()
        {
            if (_currentDraggable == null) return;

            _currentDraggable.transform.SetParent(_grid.DragLayer, true);

            var startPos = _currentDraggable.RectTransform.anchoredPosition;
            RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
            Vector2 originLocal;
            if (_originCell != null)
            {
                Vector2 screenPoint =
                    RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, _originCell.Rect.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, _canvas.worldCamera,
                    out originLocal);
            }
            else
            {
                originLocal = startPos;
            }

            _currentDraggable.RectTransform.DOKill();
            _currentDraggable.RectTransform
                .DOAnchorPos(originLocal, 0.18f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    if (_originCell != null)
                    {
                        _originCell.SetPiece(_currentDraggable);
                    }
                    else
                    {
                        var empties = _grid.AllCells.FindAll(c => c.IsEmpty());
                        if (empties.Count > 0) empties[0].SetPiece(_currentDraggable);
                        else Destroy(_currentDraggable.gameObject);
                    }

                    _currentDraggable.transform.DOKill();
                    _currentDraggable.transform.localScale = Vector3.one;

                    _currentDraggable = null;
                    _originCell = null;
                });
        }

        private void Merge(Cell targetCell, Piece targetPiece, Piece other)
        {
            targetPiece.transform.SetParent(_grid.DragLayer, true);
            other.transform.SetParent(_grid.DragLayer, true);

            targetPiece.transform.DOKill();
            other.gameObject.SetActive(false);

            Sequence seq = DOTween.Sequence();

            seq.Append(targetPiece.PlayMergeAnim(1.5f, 0.15f, 0.15f));
            seq.AppendCallback(() =>
            {
                Destroy(targetPiece.gameObject);
                Destroy(other.gameObject);
                targetCell.Clear();

                var newPiece = _grid.CreatePiece(other.PieceType, other.Level + 1);
                targetCell.SetPiece(newPiece);
            });

            seq.OnComplete(() =>
            {
                _currentDraggable = null;
                _originCell = null;
            });
        }
    }
}