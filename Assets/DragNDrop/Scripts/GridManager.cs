using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DragNDrop.Scripts
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Piece _piecePrefab;
        [SerializeField] private List<Cell> _allCells;

        private readonly PieceType[] _spawnTypes = { PieceType.Red, PieceType.Blue, PieceType.Green };

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

        private Piece CreatePiece(PieceType type, int lvl)
        {
            var go = Instantiate(_piecePrefab.gameObject, _canvas.transform, false);
            var piece = go.GetComponent<Piece>();
            piece.Init(type, lvl);
            return piece;
        }
    }
}