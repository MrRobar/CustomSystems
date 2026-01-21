using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DragNDrop.Scripts
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Piece _piecePrefab;
        [SerializeField] private RectTransform _dragLayer;
        [SerializeField] private List<Cell> _allCells;

        private readonly PieceType[] _spawnTypes = { PieceType.Red, PieceType.Blue, PieceType.Green };
        public RectTransform DragLayer => _dragLayer;
        public List<Cell> AllCells => _allCells;

        private void Awake()
        {
            if (_canvas == null)
                _canvas = FindObjectOfType<Canvas>();
        }

        public void SpawnRandomPiece()
        {
            var empties = _allCells.Where(c => c.IsEmpty()).ToArray();
            if (empties.Length == 0)
            {
                Debug.Log("No free cells...");
                return;
            }

            var cell = empties[Random.Range(0, empties.Length)];
            var type = _spawnTypes[Random.Range(0, _spawnTypes.Length)];
            var piece = CreatePiece(type, 1);
            cell.SetPiece(piece);
        }

        public Piece CreatePiece(PieceType type, int lvl)
        {
            var go = Instantiate(_piecePrefab.gameObject, _canvas.transform, false);
            var piece = go.GetComponent<Piece>();
            piece.Init(type, lvl);
            return piece;
        }

        public Cell GetCellUnderPointer(Vector2 screenPoint)
        {
            foreach (var cell in _allCells)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(cell.Rect, screenPoint, _canvas.worldCamera))
                    return cell;
            }

            return null;
        }
    }
}