using UnityEngine;
using DG.Tweening;

namespace Yunus.Match3
{
    /// <summary>
    /// Tile görsel temsili (Single Responsibility - SADECE görsel!)
    /// Input logic MouseInputHandler'da
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class TileView : MonoBehaviour
    {
        private Tile tile;
        private SpriteRenderer spriteRenderer;
        private bool isSelected = false;
        
        public Tile Tile => tile;
        public int X => tile?.X ?? -1;
        public int Y => tile?.Y ?? -1;
        public bool IsSelected => isSelected;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        public void Initialize(Tile tileData, Sprite sprite)
        {
            this.tile = tileData;
            
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            spriteRenderer.sprite = sprite;
            UpdatePosition();
        }
        
        public void UpdatePosition()
        {
            if (tile == null) return;
            // TEK KAYNAK: Tile.X, Tile.Y
            // DİREK KULLAN! Y=0 (alt), Y=7 (üst)
            Vector3 localPos = new Vector3(tile.X, tile.Y, 0);
            transform.localPosition = localPos;
        }
        
        public void Select()
        {
            isSelected = true;
            transform.DOScale(Vector3.one * 1.1f, 0.15f).SetEase(Ease.OutBack);
        }
        
        public void Deselect()
        {
            isSelected = false;
            transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.InBack);
        }
        
        /// <summary>
        /// Tile'ı hedef pozisyona hareket ettir (DOTween - LOCAL!)
        /// </summary>
        public void MoveTo(Vector3 targetPosition, float duration = 0.3f)
        {
            transform.DOLocalMove(targetPosition, duration).SetEase(Ease.OutCubic);
        }
        
        /// <summary>
        /// Shake animasyon (fail feedback)
        /// </summary>
        public void PlayShake()
        {
            transform.DOShakePosition(0.4f, 0.12f, 10, 90, false, true);
        }
        
        /// <summary>
        /// Tile'ı yok et (DOTween scale animasyonu ile)
        /// Match sonrası çağrılır
        /// </summary>
        public void DestroyWithAnimation(float duration = 0.25f)
        {
            // Scale to 0 animation
            transform.DOScale(Vector3.zero, duration)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(gameObject));
        }
        
        /// <summary>
        /// Tile'ı yeni pozisyona düşür (gravity animation)
        /// </summary>
        /// <param name="newY">Yeni Y pozisyonu</param>
        /// <param name="duration">Animation süresi</param>
        public void AnimateFall(int newY, float duration = 0.3f)
        {
            // DİREK KULLAN! Y=0 (alt), Y=7 (üst)
            Vector3 targetPos = new Vector3(transform.localPosition.x, newY, 0);
            transform.DOLocalMove(targetPos, duration).SetEase(Ease.OutBounce);
        }
    }
}
