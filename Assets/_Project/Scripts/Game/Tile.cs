using System;

namespace Yunus.Match3
{

/// <summary>
/// Oyun tahtasındaki tek bir tile'ın veri modelini temsil eder.
/// 
/// NE İŞE YARAR:
/// Tile'ın pozisyonu, tipi gibi VERİLERİ tutar. Görsel veya input işlemi YAPMAZ!
/// Bu sınıf Pure C# class'tır, Unity'ye bağımlı değildir.
/// 
/// NEDEN MONOBEHAVIOUR DEĞİL:
/// 1. Test edilebilir → Unity olmadan birim test yazabilirim
/// 2. Performanslı → GameObject oluşturma maliyeti yok
/// 3. Temiz → Sorumluluk ayrımı (Veri ≠ Görsel)
/// 4. Sektör standardı → Büyük stüdyolarda kullanılan yöntem
/// 
/// UNITY'DEKİ KARŞILIĞI:
/// Normalde "Tile : MonoBehaviour" yapardık ve her tile bir GameObject olurdu.
/// Ama bu yöntemde:
/// - Tile: Sadece data (bu sınıf)
/// - TileView: GameObject + Görsel (ayrı bir MonoBehaviour)
/// 
/// ÖRNEK KULLANIM:
/// var tile = new Tile(5, 3, TileType.Red);
/// if (tile.IsNeighbor(otherTile)) { ... }
/// 
/// SEKTÖR BİLGİSİ:
/// Bu yaklaşıma "Data-Oriented" veya "Entity-Component ayrımı" denir.
/// Candy Crush, Homescapes gibi büyük oyunlar bu yapıyı kullanır.
/// </summary>
public class Tile
{
    #region Properties (Özellikler)

    /// <summary>
    /// Tile'ın X (sütun) pozisyonu.
    /// Grid'de soldan sağa 0'dan başlar.
    /// 
    /// Örnek: 8x8 grid'de X değeri 0-7 arasındadır.
    /// </summary>
    public int X { get; private set; }

    /// <summary>
    /// Tile'ın Y (satır) pozisyonu.
    /// Grid'de alttan yukarı 0'dan başlar.
    /// 
    /// Örnek: 8x8 grid'de Y değeri 0-7 arasındadır.
    /// </summary>
    public int Y { get; private set; }

    /// <summary>
    /// Tile'ın tipi (rengi).
    /// Match kontrolünde bu değer kullanılır.
    /// 
    /// Örnek: TileType.Red, TileType.Blue vb.
    /// </summary>
    public TileType Type { get; private set; }

    /// <summary>
    /// Bu tile eşleştirilebilir mi?
    /// 
    /// KULLANIM ALANLARI:
    /// - Normal tile: true
    /// - Bomba patlarken: false (geçici olarak)
    /// - Boş hücre: false
    /// - Engel tile: false
    /// </summary>
    public bool IsMatchable { get; set; } = true;

    /// <summary>
    /// Bu tile hareket edebilir mi?
    /// 
    /// KULLANIM ALANLARI:
    /// - Normal tile: true
    /// - Taş gibi sabit tile: false
    /// - Animasyon oynarken: false (geçici)
    /// </summary>
    public bool IsMovable { get; set; } = true;

    #endregion

    #region Constructor (Yapıcı Metod)

    /// <summary>
    /// Yeni bir Tile oluşturur.
    /// 
    /// PARAMETRE AÇIKLAMALARI:
    /// x: Grid'deki X pozisyonu (sütun)
    /// y: Grid'deki Y pozisyonu (satır)
    /// type: Tile'ın tipi (rengi)
    /// 
    /// KULLANIM:
    /// var redTile = new Tile(0, 0, TileType.Red);
    /// var blueTile = new Tile(1, 0, TileType.Blue);
    /// 
    /// NEDEN CONSTRUCTOR:
    /// Tile oluşturulurken pozisyon ve tip ZORUNLU olmalı.
    /// Bu sayede "yarım yamalak" tile oluşturulması engellenir.
    /// </summary>
    public Tile(int x, int y, TileType type)
    {
        X = x;
        Y = y;
        Type = type;
    }

    #endregion

    #region Public Methods (Dışarıdan Kullanılabilir Metodlar)

    /// <summary>
    /// Tile'ın pozisyonunu günceller.
    /// 
    /// NE ZAMAN KULLANILIR:
    /// - Tile swap (takas) işleminde
    /// - Gravity (yerçekimi) uygulanırken
    /// - Board shuffle (karıştırma) işleminde
    /// 
    /// NEDEN AYRI BİR METOD:
    /// Property'ler private set olduğu için dışarıdan değiştirilemez.
    /// Bu metod kontrollü bir şekilde pozisyon değişimine izin verir.
    /// 
    /// ÖRNEK:
    /// tile.SetPosition(3, 5); // Tile'ı (3,5) pozisyonuna taşı
    /// </summary>
    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Tile'ın tipini değiştirir.
    /// 
    /// NE ZAMAN KULLANILIR:
    /// - Power-up oluştururken (normal tile → bomba)
    /// - Board shuffle'da
    /// - Özel level mekaniklerinde
    /// 
    /// ÖRNEK:
    /// tile.SetType(TileType.Bomb); // Normal tile'ı bombaya çevir
    /// </summary>
    public void SetType(TileType type)
    {
        Type = type;
    }

    /// <summary>
    /// Bu tile başka bir tile'ın komşusu mu kontrol eder.
    /// 
    /// KOMŞULUK KURALI:
    /// İki tile komşudur ancak ve ancak:
    /// - Aynı satırda ve 1 sütun farkı varsa (yatay komşu)
    /// - Aynı sütunda ve 1 satır farkı varsa (dikey komşu)
    /// - Çapraz komşular GEÇERLİ DEĞİL (Match-3 kuralı)
    /// 
    /// KULLANIM ALANLARI:
    /// - Swap validation (takas geçerli mi?)
    /// - Match detection (eşleşme kontrolü)
    /// - Pathfinding (özel seviye mekanikleri)
    /// 
    /// ÖRNEKLER:
    /// Tile A (2,3) ve Tile B (3,3) → true (yatay komşu)
    /// Tile A (2,3) ve Tile B (2,4) → true (dikey komşu)
    /// Tile A (2,3) ve Tile B (3,4) → false (çapraz)
    /// Tile A (2,3) ve Tile B (4,3) → false (2 tile uzakta)
    /// </summary>
    public bool IsNeighbor(Tile other)
    {
        // Null kontrolü (savunma programlama)
        if (other == null)
            return false;

        // X farkını hesapla (mutlak değer)
        int deltaX = Math.Abs(X - other.X);
        
        // Y farkını hesapla (mutlak değer)
        int deltaY = Math.Abs(Y - other.Y);

        // Komşuluk kontrolü:
        // (deltaX == 1 && deltaY == 0) → Yatay komşu (sağ veya sol)
        // (deltaX == 0 && deltaY == 1) → Dikey komşu (üst veya alt)
        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    /// <summary>
    /// Bu tile başka bir tile ile eşleşebilir mi kontrol eder.
    /// 
    /// EŞLEŞME KURALLARI:
    /// 1. Her iki tile da matchable olmalı
    /// 2. Aynı tipte olmalılar
    /// 
    /// ÖZEL DURUMLAR:
    /// - Wildcard tile'lar her şeyle eşleşebilir (ileride eklenecek)
    /// - Bomba tile'lar eşleşmez (Type = Bomb)
    /// - Engel tile'lar eşleşmez (IsMatchable = false)
    /// 
    /// KULLANIM:
    /// if (tile1.CanMatchWith(tile2))
    /// {
    ///     // Bu iki tile eşleşebilir!
    /// }
    /// 
    /// NEDEN AYRI BİR METOD:
    /// Sadece "Type == Type" kontrolünden daha fazlası var.
    /// İleride wildcard, özel tile gibi mekanikler eklenebilir.
    /// </summary>
    public bool CanMatchWith(Tile other)
    {
        // Null kontrolü
        if (other == null)
            return false;

        // Her iki tile da eşleştirilebilir olmalı
        if (!IsMatchable || !other.IsMatchable)
            return false;

        // Aynı tipte olmalılar
        return Type == other.Type;
    }

    /// <summary>
    /// İki tile arasındaki Manhattan mesafesini hesaplar.
    /// 
    /// MANHATTAN MESAFESİ NEDİR:
    /// Grid üzerinde bir noktadan diğerine gitmek için
    /// gereken minimum adım sayısıdır (çapraz gidiş yok).
    /// 
    /// FORMÜL:
    /// Distance = |X1 - X2| + |Y1 - Y2|
    /// 
    /// ÖRNEKLER:
    /// A(0,0) → B(3,0) = 3 (3 sağa)
    /// A(0,0) → B(0,4) = 4 (4 yukarı)
    /// A(0,0) → B(3,4) = 7 (3 sağa + 4 yukarı)
    /// 
    /// KULLANIM ALANLARI:
    /// - Animasyon süresi hesaplama (uzak tile'lar daha uzun sürede hareket eder)
    /// - Pathfinding algoritmaları
    /// - Özel level mekanikleri (menzil kontrolü)
    /// </summary>
    public int GetManhattanDistance(Tile other)
    {
        if (other == null)
            return int.MaxValue; // Çok uzak kabul et

        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }

    #endregion

    #region Override Methods (C# Object Metodları)

    /// <summary>
    /// Tile'ı string formatında gösterir (debug için).
    /// 
    /// KULLANIM:
    /// Debug.Log(tile.ToString());
    /// // Output: "Tile(2,3) - Red"
    /// 
    /// NE ZAMAN KULLANILIR:
    /// - Console'da tile bilgisi gösterirken
    /// - Log mesajlarında
    /// - Debug amaçlı
    /// </summary>
    public override string ToString()
    {
        return $"Tile({X},{Y}) - {Type}";
    }

    /// <summary>
    /// İki tile'ın eşit olup olmadığını kontrol eder.
    /// 
    /// EŞİTLİK KURALI:
    /// İki tile eşittir ancak ve ancak:
    /// - Aynı pozisyondalarsa (X ve Y aynı)
    /// 
    /// NOT: Tip kontrolü YAPILMAZ!
    /// Çünkü aynı pozisyonda iki farklı tile olamaz.
    /// 
    /// KULLANIM:
    /// if (tile1.Equals(tile2)) { ... }
    /// if (tile1 == tile2) { ... } // Bu da çalışır
    /// 
    /// NEDEN ÖNEMLİ:
    /// List, Dictionary gibi koleksiyonlarda tile aramak için gerekli.
    /// </summary>
    public override bool Equals(object obj)
    {
        // Tip kontrolü
        if (obj is Tile other)
        {
            // Aynı pozisyondalarsa eşitler
            return X == other.X && Y == other.Y;
        }

        return false;
    }

    /// <summary>
    /// Tile için hash code üretir.
    /// 
    /// NE İŞE YARAR:
    /// Dictionary ve HashSet gibi koleksiyonlarda kullanılır.
    /// Hızlı arama için gereklidir.
    /// 
    /// KURAL:
    /// Eğer Equals() override ediliyorsa, GetHashCode() da edilmelidir!
    /// 
    /// FORMÜL:
    /// Position'a göre unique bir sayı üretir.
    /// </summary>
    public override int GetHashCode()
    {
        // X ve Y'yi birleştirerek unique hash oluştur
        return (X * 397) ^ Y; // 397 prime sayı (collision azaltmak için)
    }

    #endregion
}

/// <summary>
/// Tile tiplerini tanımlar.
/// 
/// ENUM NEDİR:
/// Sabit değerleri isimlendirmek için kullanılır.
/// "Magic number" kullanmaktan daha güvenli ve okunabilirdir.
/// 
/// KÖTÜ ÖRNEK:
/// if (tile.type == 0) // 0 ne demek?
/// 
/// İYİ ÖRNEK:
/// if (tile.Type == TileType.Red) // Açık ve anlaşılır!
/// 
/// GELECEK EKLEMELERİ:
/// - Wildcard (her tile ile eşleşir)
/// - Bomb (patlar, etrafındakileri yok eder)
/// - Rocket (satır/sütun temizler)
/// - ColorBomb (tüm aynı renkleri temizler)
/// </summary>
public enum TileType
{
    /// <summary>Kırmızı tile</summary>
    Red,
    
    /// <summary>Mavi tile</summary>
    Blue,
    
    /// <summary>Yeşil tile</summary>
    Green,
    
    /// <summary>Sarı tile</summary>
    Yellow,
    
    /// <summary>Mor tile</summary>
    Purple,
    
    /// <summary>Turuncu tile (opsiyonel, 6 renk varsa)</summary>
    Orange,
    
    // İleride eklenecek özel tipler:
    // Bomb,        // 4-match ile oluşur
    // Rocket,      // L/T-match ile oluşur
    // ColorBomb,   // 5-match ile oluşur
}

/*
 * ===== KULLANIM KILAVUZU =====
 * 
 * --- YENİ TİLE OLUŞTURMA ---
 * 
 * var tile = new Tile(5, 3, TileType.Red);
 * 
 * --- POZİSYON DEĞİŞTİRME ---
 * 
 * tile.SetPosition(6, 3); // Tile'ı sağa kaydır
 * 
 * --- KOMŞULUK KONTROLÜ ---
 * 
 * if (tile1.IsNeighbor(tile2))
 * {
 *     // İki tile yan yana, swap yapılabilir
 * }
 * 
 * --- EŞLEŞME KONTROLÜ ---
 * 
 * if (tile1.CanMatchWith(tile2))
 * {
 *     // İki tile eşleşebilir (aynı tip ve matchable)
 * }
 * 
 * --- MESAFE HESAPLAMA ---
 * 
 * int distance = tile1.GetManhattanDistance(tile2);
 * float animDuration = distance * 0.1f; // Uzak tile'lar daha uzun sürede hareket eder
 * 
 * ===== SEKTÖR BİLGİSİ =====
 * 
 * Bu yapı "Data-Oriented Design" yaklaşımının bir örneğidir:
 * - Veri (Tile) ve görsel (TileView) ayrı
 * - Test edilebilir (Pure C#)
 * - Performanslı (MonoBehaviour overhead yok)
 * - Ölçeklenebilir (binlerce tile yaratılabilir)
 * 
 * Büyük oyun stüdyolarında (King, Supercell, Playrix) kullanılan standarttır.
 */

} // namespace Yunus.Match3

