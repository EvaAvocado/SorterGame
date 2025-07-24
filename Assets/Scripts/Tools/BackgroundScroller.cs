using UnityEngine;

namespace Tools
{
    /// <summary>
    /// Управляет бесконечной прокруткой фона
    /// </summary>
    public class BackgroundScroller : MonoBehaviour
    {
        [SerializeField] private float _scrollSpeed = 1f;
        [SerializeField] private Vector2 _movementDirection = new Vector2(-1f, -1f);
        [SerializeField] private SpriteRenderer _spriteTemplate;
        [SerializeField] private Camera _mainCamera;

        private Vector2 _tileSize;
        private int _tilesX, _tilesY;
        private GameObject[,] _tiles;
        private Vector2 _scrollOffset = Vector2.zero;

        private void Start()
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;

            _tileSize = _spriteTemplate.bounds.size;

            if (_mainCamera != null)
            {
                float screenHeight = 2f * _mainCamera.orthographicSize;
                float screenWidth = screenHeight * _mainCamera.aspect;

                _tilesX = Mathf.CeilToInt(screenWidth / _tileSize.x) + 2;
                _tilesY = Mathf.CeilToInt(screenHeight / _tileSize.y) + 2;
            }

            _tiles = new GameObject[_tilesX, _tilesY];

            // Создаем тайлы и размещаем в сетке
            for (int x = 0; x < _tilesX; x++)
            {
                for (int y = 0; y < _tilesY; y++)
                {
                    GameObject tile = new GameObject($"Tile_{x}_{y}");
                    tile.transform.parent = transform;

                    SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                    sr.sprite = _spriteTemplate.sprite;
                    sr.color = _spriteTemplate.color;
                    sr.sortingLayerID = _spriteTemplate.sortingLayerID;
                    sr.sortingOrder = _spriteTemplate.sortingOrder;

                    tile.transform.localPosition = new Vector3(
                        x * _tileSize.x,
                        y * _tileSize.y,
                        0
                    );

                    _tiles[x, y] = tile;
                }
            }
        }

        private void Update()
        {
            Vector2 direction = _movementDirection.normalized;
            _scrollOffset += direction * (_scrollSpeed * Time.deltaTime);

            for (int x = 0; x < _tilesX; x++)
            {
                for (int y = 0; y < _tilesY; y++)
                {
                    Vector2 basePos = new Vector2(x * _tileSize.x, y * _tileSize.y);
                    Vector2 offsetPos = basePos + _scrollOffset;

                    // Зацикливание по X
                    if (offsetPos.x < -_tileSize.x)
                        offsetPos.x += _tileSize.x * _tilesX;
                    else if (offsetPos.x > _tileSize.x * (_tilesX - 1))
                        offsetPos.x -= _tileSize.x * _tilesX;

                    // Зацикливание по Y
                    if (offsetPos.y < -_tileSize.y)
                        offsetPos.y += _tileSize.y * _tilesY;
                    else if (offsetPos.y > _tileSize.y * (_tilesY - 1))
                        offsetPos.y -= _tileSize.y * _tilesY;

                    _tiles[x, y].transform.localPosition = offsetPos;
                }
            }
        }
    }
}