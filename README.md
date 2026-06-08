# 🚢 Tutorial Unity: Submarine & Treasure
## Panduan Pemula — Core Loop Lengkap (Semua Script)

---

## ✅ Yang Dibutuhkan
- Unity **2022.3 LTS** (atau lebih baru)
- TextMeshPro (sudah built-in di Unity)
- **Tidak perlu asset berbayar** — pakai primitive shapes dulu

## 📁 Buat Folder Ini di Assets
```
Assets/
├── Scripts/
├── Prefabs/
├── Materials/
└── Scenes/
```

## 🗂️ Dua Scene yang Dibutuhkan
| Scene | Isi |
|---|---|
| **MainMenu** | Main Menu, About, Settings, Quit Confirm |
| **GameScene** | Gameplay, HUD, Game Over, High Score |

---

# ══════════════════════════════════════
# LANGKAH 0 — SETUP PROJECT AWAL
# ══════════════════════════════════════

1. Buka **Unity Hub** → **New Project** → pilih template **3D** → nama: `SubmarineTreasure` → **Create**
2. Pergi ke **Window → TextMeshPro → Import TMP Essential Resources** → klik **Import All**
3. Buat 4 folder di Assets: `Scripts`, `Prefabs`, `Materials`, `Scenes`
4. Buat 2 scene baru:
   - Klik kanan folder `Scenes` → **Create → Scene** → namai `MainMenu`
   - Ulangi → namai `GameScene`
5. Pergi ke **File → Build Settings**:
   - Drag `MainMenu` → posisi **index 0**
   - Drag `GameScene` → posisi **index 1**

---

# ══════════════════════════════════════
# BAGIAN 1 — SETUP GAMESCENE
# ══════════════════════════════════════

### Langkah 1.1 — Buka GameScene
Double-click scene `GameScene` untuk membukanya.

### Langkah 1.2 — Atur Main Camera
1. Klik **Main Camera** di Hierarchy
2. Set di Inspector:
   - **Position**: X=0, Y=4, Z=-6
   - **Rotation**: X=20, Y=0, Z=0

### Langkah 1.3 — Buat Lantai Laut
1. Klik kanan Hierarchy → **3D Object → Plane** → rename: `SeaFloor`
2. Set **Position**: X=0, Y=-1, Z=15
3. Set **Scale**: X=5, Y=1, Z=30
4. Buat material biru:
   - Klik kanan Assets/Materials → **Create → Material** → namai `SeaMat`
   - Ubah warna Albedo ke biru tua
   - Drag `SeaMat` ke `SeaFloor`

---

# ══════════════════════════════════════
# BAGIAN 2 — BUAT KAPAL SELAM & PREFABS
# ══════════════════════════════════════

### Langkah 2.1 — Buat Kapal Selam

1. Klik kanan Hierarchy → **3D Object → Capsule** → rename: `Submarine`
2. Set **Rotation**: X=90, Y=0, Z=0  *(supaya memanjang ke depan)*
3. Set **Scale**: X=0.8, Y=1.5, Z=0.8
4. Set **Position**: X=0, Y=0, Z=0
5. Klik **Add Component → Rigidbody**:
   - ✅ Centang **Is Kinematic**
   - Di **Constraints** → ✅ Freeze Rotation X, Y, Z
6. Klik **Add Component → Capsule Collider**:
   - ✅ Centang **Is Trigger**
   - Set **Direction**: Z-Axis | **Height**: 2 | **Radius**: 0.5
7. Buat material hijau-kuning untuk warna kapal selam, drag ke Submarine.

### Langkah 2.2 — Daftarkan Tag Baru

> **Tag** dipakai untuk deteksi tabrakan (collision)

1. Klik **Edit → Project Settings → Tags and Layers**
2. Di bagian **Tags**, klik **+** dan tambahkan: `Obstacle`
3. Klik **+** lagi dan tambahkan: `Treasure`

### Langkah 2.3 — Buat Prefab Obstacle

1. Klik kanan Hierarchy → **3D Object → Cube** → rename: `Obstacle`
2. Set **Scale**: X=1.5, Y=1.5, Z=1.5
3. Buat material merah → namai `ObstacleMat` → drag ke Obstacle
4. **Add Component → Box Collider** → ✅ **Is Trigger**
5. Di bagian **Tag** (atas Inspector) → pilih `Obstacle`
6. **Drag Obstacle dari Hierarchy ke folder Assets/Prefabs** *(ini membuat prefab)*
7. Setelah jadi prefab → **klik kanan di Hierarchy → Delete** *(hapus dari scene)*

### Langkah 2.4 — Buat Prefab Treasure

1. Klik kanan Hierarchy → **3D Object → Sphere** → rename: `Treasure`
2. Set **Scale**: X=0.7, Y=0.7, Z=0.7
3. Buat material kuning/gold → namai `TreasureMat` → drag ke Treasure
4. **Add Component → Sphere Collider** → ✅ **Is Trigger**
5. Set **Tag** = `Treasure`
6. **Drag ke Assets/Prefabs → hapus dari scene**

---

# ══════════════════════════════════════
# BAGIAN 3 — SEMUA SCRIPT (9 SCRIPT)
# ══════════════════════════════════════

> 💡 **Cara buat script**: Klik kanan di folder `Assets/Scripts`
> → **Create → C# Script** → beri nama sesuai judul di bawah

---

## 📄 Script 1: `GameManager.cs`

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

// Mengontrol keadaan game: mulai, game over, restart, ganti scene
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton, bisa diakses dari mana saja
    public bool isGameRunning = false;  // true = game sedang berjalan

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        isGameRunning = true;
        Time.timeScale = 1f;
    }

    // Dipanggil saat submarine menabrak obstacle
    public void GameOver()
    {
        isGameRunning = false;

        // Simpan skor terbaik ke device
        int currentScore = ScoreManager.Instance.score;
        int bestScore    = PlayerPrefs.GetInt("BestScore", 0);
        if (currentScore > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", currentScore);
            PlayerPrefs.Save();
        }

        UIManager.Instance.ShowGameOver();
    }

    public void RestartGame()   { SceneManager.LoadScene("GameScene"); }
    public void GoToMainMenu()  { SceneManager.LoadScene("MainMenu"); }
    public void GoToHighScore() { UIManager.Instance.ShowHighScore(); }
}
```

---

## 📄 Script 2: `ScoreManager.cs`

```csharp
using UnityEngine;

// Mengelola skor dan koin yang dikumpulkan
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score         = 0; // Total skor
    public int coinCollected = 0; // Total koin

    private float timer = 0f;     // Timer nambah skor otomatis

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!GameManager.Instance.isGameRunning) return;

        // Tambah +1 skor tiap 1 detik (skor jarak)
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            score += 1;
            timer  = 0f;
            UIManager.Instance.UpdateScoreUI(score);
        }
    }

    // Dipanggil saat mengambil treasure/koin
    public void CollectTreasure()
    {
        coinCollected++;
        score += 10; // Bonus skor per koin
        UIManager.Instance.UpdateScoreUI(score);
        UIManager.Instance.UpdateCoinUI(coinCollected);
    }
}
```

---

## 📄 Script 3: `SubmarineController.cs`

```csharp
using UnityEngine;

// Mengontrol gerakan kapal selam: ganti jalur kiri/kanan
public class SubmarineController : MonoBehaviour
{
    public float laneDistance = 3f;  // Jarak antar jalur (kiri-tengah-kanan)
    public float moveSpeed    = 15f; // Kecepatan pindah jalur

    private int     currentLane    = 1;     // 0=kiri, 1=tengah, 2=kanan
    private Vector3 targetPosition;         // Posisi tujuan smooth movement

    // Untuk deteksi swipe layar HP
    private Vector2 touchStart;
    private bool    isSwiping = false;

    void Start()
    {
        currentLane    = 1;
        targetPosition = transform.position;
    }

    void Update()
    {
        if (!GameManager.Instance.isGameRunning) return;

        // Gerak smooth menuju posisi target
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Input keyboard (untuk test di Unity Editor)
        if (Input.GetKeyDown(KeyCode.LeftArrow)  || Input.GetKeyDown(KeyCode.A)) MoveLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) MoveRight();

        // Input swipe (untuk HP/tablet)
        HandleSwipeInput();
    }

    void HandleSwipeInput()
    {
        if (Input.touchCount <= 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            touchStart = touch.position;
            isSwiping  = true;
        }

        if (touch.phase == TouchPhase.Ended && isSwiping)
        {
            Vector2 swipe = touch.position - touchStart;
            isSwiping = false;

            // Deteksi arah swipe — minimal 50 pixel agar tidak sensitif
            if (Mathf.Abs(swipe.x) > 50f)
            {
                if (swipe.x > 0) MoveRight();
                else             MoveLeft();
            }
        }
    }

    void MoveLeft()
    {
        if (currentLane > 0) { currentLane--; UpdateTarget(); }
    }

    void MoveRight()
    {
        if (currentLane < 2) { currentLane++; UpdateTarget(); }
    }

    void UpdateTarget()
    {
        // Jalur 0 = X:-3 | Jalur 1 = X:0 | Jalur 2 = X:+3
        float targetX  = (currentLane - 1) * laneDistance;
        targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
    }

    // Deteksi tabrakan dengan trigger collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();   // Kena obstacle → Game Over!
        }
        else if (other.CompareTag("Treasure"))
        {
            ScoreManager.Instance.CollectTreasure(); // Ambil koin
            Destroy(other.gameObject);
        }
    }
}
```

---

## 📄 Script 4: `MoveObject.cs`

```csharp
using UnityEngine;

// Menggerakkan obstacle & treasure menuju pemain
// ATTACH script ini ke prefab Obstacle DAN prefab Treasure
public class MoveObject : MonoBehaviour
{
    public float speed      = 10f;  // Kecepatan bergerak
    public float destroyAtZ = -10f; // Hapus object jika sudah melewati posisi Z ini

    void Update()
    {
        if (!GameManager.Instance.isGameRunning) return;

        // Gerak ke arah -Z (menuju kamera/pemain)
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        // Hapus dari memory jika sudah melewati pemain
        if (transform.position.z < destroyAtZ)
            Destroy(gameObject);
    }
}
```

---

## 📄 Script 5: `ObstacleSpawner.cs`

```csharp
using UnityEngine;

// Membuat obstacle secara otomatis di jalur random
public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;         // Drag prefab Obstacle ke sini di Inspector
    public float      spawnInterval = 2.5f;   // Jarak waktu spawn (detik)
    public float      spawnZ        = 25f;    // Posisi Z tempat obstacle muncul
    public float      obstacleSpeed = 10f;    // Kecepatan obstacle bergerak

    private float   timer = 0f;
    private float[] lanes = { -3f, 0f, 3f }; // Posisi X tiga jalur

    void Update()
    {
        if (!GameManager.Instance.isGameRunning) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnObstacle();
            timer = 0f;

            // Difficulty scaling: semakin lama semakin cepat dan sering
            spawnInterval = Mathf.Max(0.8f,  spawnInterval - 0.02f);
            obstacleSpeed = Mathf.Min(20f,   obstacleSpeed + 0.1f);
        }
    }

    void SpawnObstacle()
    {
        int lane     = Random.Range(0, 3); // Pilih jalur random
        Vector3 pos  = new Vector3(lanes[lane], 0f, spawnZ);

        GameObject obs  = Instantiate(obstaclePrefab, pos, Quaternion.identity);
        MoveObject mover = obs.GetComponent<MoveObject>();
        if (mover != null) mover.speed = obstacleSpeed;
    }
}
```

---

## 📄 Script 6: `TreasureSpawner.cs`

```csharp
using UnityEngine;

// Membuat treasure/koin secara otomatis
public class TreasureSpawner : MonoBehaviour
{
    public GameObject treasurePrefab;        // Drag prefab Treasure ke sini
    public float      spawnInterval = 1.5f;  // Jarak waktu spawn
    public float      spawnZ        = 25f;   // Posisi Z spawn
    public float      treasureSpeed = 10f;   // Kecepatan treasure

    private float   timer = 0f;
    private float[] lanes = { -3f, 0f, 3f };

    void Update()
    {
        if (!GameManager.Instance.isGameRunning) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnTreasure();
            timer = 0f;
        }
    }

    void SpawnTreasure()
    {
        int lane     = Random.Range(0, 3);
        Vector3 pos  = new Vector3(lanes[lane], 0f, spawnZ);

        GameObject tr  = Instantiate(treasurePrefab, pos, Quaternion.identity);
        MoveObject mv  = tr.GetComponent<MoveObject>();
        if (mv != null) mv.speed = treasureSpeed;
    }
}
```

---

## 📄 Script 7: `UIManager.cs`

```csharp
using UnityEngine;
using TMPro;

// Mengelola semua UI di GameScene
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("=== HUD (saat bermain) ===")]
    public TextMeshProUGUI scoreText;       // Tampilan skor
    public TextMeshProUGUI coinText;        // Tampilan koin

    [Header("=== Panel Game Over ===")]
    public GameObject      gameOverPanel;
    public TextMeshProUGUI goScoreText;     // Skor di Game Over
    public TextMeshProUGUI goCoinText;      // Koin di Game Over
    public TextMeshProUGUI goBestScoreText; // Best Score di Game Over

    [Header("=== Panel High Score ===")]
    public GameObject      highScorePanel;
    public TextMeshProUGUI hsText;          // Tampilan High Score

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        if (highScorePanel != null) highScorePanel.SetActive(false);
        UpdateScoreUI(0);
        UpdateCoinUI(0);
    }

    public void UpdateScoreUI(int score) { if (scoreText) scoreText.text = "Score: " + score; }
    public void UpdateCoinUI(int coin)   { if (coinText)  coinText.text  = "Coin: "  + coin;  }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        goScoreText.text     = "Score: "       + ScoreManager.Instance.score;
        goCoinText.text      = "Coin: "        + ScoreManager.Instance.coinCollected;
        goBestScoreText.text = "Best Score: "  + PlayerPrefs.GetInt("BestScore", 0);
    }

    public void ShowHighScore()
    {
        if (highScorePanel == null) return;
        gameOverPanel.SetActive(false);
        highScorePanel.SetActive(true);
        hsText.text = "High Score: " + PlayerPrefs.GetInt("BestScore", 0);
    }

    public void BackToGameOver()
    {
        if (highScorePanel != null) highScorePanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
}
```

---

## 📄 Script 8: `MainMenuManager.cs`

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

// Mengontrol semua panel di Main Menu
public class MainMenuManager : MonoBehaviour
{
    [Header("=== Panel-panel ===")]
    public GameObject mainMenuPanel;
    public GameObject aboutPanel;
    public GameObject settingsPanel;
    public GameObject quitConfirmPanel;

    void Start() { ShowMainMenu(); }

    // Tampilkan hanya panel utama, sembunyikan yang lain
    void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        aboutPanel.SetActive(false);
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
    }

    // === Tombol MAIN MENU ===
    public void OnPlayClicked()     { SceneManager.LoadScene("GameScene"); }

    public void OnAboutClicked()
    {
        mainMenuPanel.SetActive(false);
        aboutPanel.SetActive(true);
    }

    public void OnSettingsClicked()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnQuitClicked()
    {
        mainMenuPanel.SetActive(false);
        quitConfirmPanel.SetActive(true);
    }

    // === Tombol BACK ===
    public void OnAboutBackClicked()    { ShowMainMenu(); }
    public void OnSettingsBackClicked() { ShowMainMenu(); }

    // === Tombol QUIT CONFIRM ===
    public void OnQuitYesClicked()
    {
        Debug.Log("Keluar game..."); // Untuk test di Editor
        Application.Quit();
    }
    public void OnQuitNoClicked() { ShowMainMenu(); }
}
```

---

## 📄 Script 9: `SettingsManager.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;

// Mengontrol pengaturan audio (music, SFX, mute)
public class SettingsManager : MonoBehaviour
{
    public Slider musicSlider; // Drag slider musik ke sini
    public Slider sfxSlider;   // Drag slider SFX ke sini
    public Toggle muteToggle;  // Drag toggle mute ke sini

    void Start()
    {
        // Load pengaturan yang tersimpan
        if (musicSlider != null) musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (sfxSlider   != null) sfxSlider.value   = PlayerPrefs.GetFloat("SFXVolume",   1f);
        if (muteToggle  != null) muteToggle.isOn   = PlayerPrefs.GetInt("Muted", 0) == 1;
        ApplyVolume();
    }

    public void OnMusicChanged(float val) { PlayerPrefs.SetFloat("MusicVolume", val); ApplyVolume(); }
    public void OnSFXChanged(float val)   { PlayerPrefs.SetFloat("SFXVolume",   val); ApplyVolume(); }
    public void OnMuteChanged(bool muted) { PlayerPrefs.SetInt("Muted", muted ? 1 : 0); ApplyVolume(); }

    void ApplyVolume()
    {
        bool  muted   = PlayerPrefs.GetInt("Muted", 0) == 1;
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        AudioListener.volume = muted ? 0f : musicVol;
    }
}
```

---

# ══════════════════════════════════════
# BAGIAN 4 — SETUP UI DI GAMESCENE
# ══════════════════════════════════════

### Langkah 4.1 — Tambahkan MoveObject ke Prefabs

1. Di folder **Assets/Prefabs**, klik prefab `Obstacle`
2. **Add Component → MoveObject**
3. Klik prefab `Treasure` → **Add Component → MoveObject**

---

### Langkah 4.2 — Buat Semua GameObjects Manager

| Nama Object | Script yang Di-Attach |
|---|---|
| `GameManager` | `GameManager.cs` + `ScoreManager.cs` |
| `UIManager` | `UIManager.cs` |
| `Spawners` | `ObstacleSpawner.cs` + `TreasureSpawner.cs` |
| `Submarine` | `SubmarineController.cs` |

> Cara: Klik kanan Hierarchy → **Create Empty** → rename → drag script ke object

---

### Langkah 4.3 — Buat Canvas & Panel UI

1. Klik kanan Hierarchy → **UI → Canvas**
2. Di Canvas Component → **Render Mode**: Screen Space - Overlay
3. Klik kanan Canvas → **Create Empty** → namai `HUD`
4. Klik kanan Canvas → **Create Empty** → namai `GameOverPanel`
5. Klik kanan Canvas → **Create Empty** → namai `HighScorePanel`

---

### Langkah 4.4 — Isi HUD

Di dalam **HUD**:
- Klik kanan HUD → **UI → Text - TextMeshPro** → namai `ScoreText`
  - Posisi: Pojok kiri atas | Teks: `Score: 0` | Font size: 30
- Klik kanan HUD → **UI → Text - TextMeshPro** → namai `CoinText`
  - Posisi: Pojok kanan atas | Teks: `Coin: 0` | Font size: 30

---

### Langkah 4.5 — Isi GameOverPanel

Di dalam **GameOverPanel**, tambahkan Image background lalu:

| Object | Nama | Teks Default |
|---|---|---|
| TextMeshPro | `GoScoreText` | `Score: 0` |
| TextMeshPro | `GoCoinText` | `Coin Collected: 0` |
| TextMeshPro | `GoBestScoreText` | `Best Score: 0` |
| Button | `RetryButton` | `RETRY` |
| Button | `MenuButton` | `MENU` |
| Button | `HighScoreButton` | `HIGH SCORE` |

---

### Langkah 4.6 — Isi HighScorePanel

| Object | Nama | Teks Default |
|---|---|---|
| TextMeshPro | `HsText` | `High Score: 0` |
| Button | `BackButton` | `BACK` |

---

### Langkah 4.7 — Hubungkan Referensi UIManager

Klik object **UIManager** → di Inspector, drag:

- **Score Text** → drag `ScoreText` dari Hierarchy
- **Coin Text** → drag `CoinText`
- **Game Over Panel** → drag `GameOverPanel`
- **Go Score Text** → drag `GoScoreText`
- **Go Coin Text** → drag `GoCoinText`
- **Go Best Score Text** → drag `GoBestScoreText`
- **High Score Panel** → drag `HighScorePanel`
- **Hs Text** → drag `HsText`

---

### Langkah 4.8 — Hubungkan Referensi Spawners

Klik object **Spawners** → di Inspector:
- **Obstacle Prefab** → drag file `Obstacle` dari Assets/Prefabs
- **Treasure Prefab** → drag file `Treasure` dari Assets/Prefabs

---

### Langkah 4.9 — Hubungkan Tombol GameScene

| Tombol | On Click() → Object | Fungsi |
|---|---|---|
| `RetryButton` | `GameManager` | `RestartGame()` |
| `MenuButton` | `GameManager` | `GoToMainMenu()` |
| `HighScoreButton` | `GameManager` | `GoToHighScore()` |
| `BackButton` (HighScore) | `UIManager` | `BackToGameOver()` |

> Cara: Klik tombol → di Inspector, bagian **On Click()** → klik **+** → drag object → pilih fungsi dari dropdown

---

# ══════════════════════════════════════
# BAGIAN 5 — SETUP MAINMENU SCENE
# ══════════════════════════════════════

Double-click `MainMenu` untuk membuka scene ini.

### Langkah 5.1 — Buat Canvas & Empat Panel

1. **UI → Canvas**
2. Di dalam Canvas, buat 4 Empty Object:
   - `MainMenuPanel`
   - `AboutPanel`
   - `SettingsPanel`
   - `QuitConfirmPanel`

---

### Langkah 5.2 — Isi MainMenuPanel

| Object | Nama | Teks |
|---|---|---|
| TextMeshPro | `TitleText` | `SUBMARINE & TREASURE` |
| Button | `PlayButton` | `PLAY` |
| Button | `AboutButton` | `ABOUT / INFO` |
| Button | `SettingsButton` | `SETTINGS` |
| Button | `QuitButton` | `QUIT` |

---

### Langkah 5.3 — Isi AboutPanel

| Object | Nama | Isi |
|---|---|---|
| TextMeshPro | `AboutTitle` | `ABOUT / INFO` |
| TextMeshPro | `InfoText` | *(isi info game di bawah)* |
| Button | `BackButton` | `BACK` |

**Isi InfoText:**
```
GAME TITLE : SUBMARINE & TREASURES
GENRE : 3D ENDLESS RUNNER
TUJUAN : Kumpulkan harta karun dan raih skor tertinggi
KONTROL : Swipe Kiri / Swipe Kanan
DEVELOPER : Barcode Game Dev
VERSION : 1.0.0
```

---

### Langkah 5.4 — Isi SettingsPanel

| Object | Nama | Keterangan |
|---|---|---|
| TextMeshPro | `SettingsTitle` | `SETTINGS` |
| Slider | `MusicSlider` | Min:0, Max:1, Value:1 |
| Slider | `SFXSlider` | Min:0, Max:1, Value:1 |
| Toggle | `MuteToggle` | Default: Off |
| Button | `BackButton` | `BACK` |

---

### Langkah 5.5 — Isi QuitConfirmPanel

| Object | Nama | Teks |
|---|---|---|
| TextMeshPro | `ConfirmText` | `KONFIRMASI KELUAR GAME ?` |
| Button | `YesButton` | `YES` |
| Button | `NoButton` | `NO` |

---

### Langkah 5.6 — Buat Manager Object

1. Klik kanan Hierarchy → **Create Empty** → namai `MainMenuManager`
2. Drag `MainMenuManager.cs` ke object ini
3. Drag `SettingsManager.cs` ke object ini

---

### Langkah 5.7 — Hubungkan Referensi MainMenuManager

Klik **MainMenuManager** object → di Inspector:
- **Main Menu Panel** → drag `MainMenuPanel`
- **About Panel** → drag `AboutPanel`
- **Settings Panel** → drag `SettingsPanel`
- **Quit Confirm Panel** → drag `QuitConfirmPanel`

---

### Langkah 5.8 — Hubungkan Semua Tombol MainMenu

| Tombol | Event | Object | Fungsi |
|---|---|---|---|
| `PlayButton` | On Click | `MainMenuManager` | `OnPlayClicked()` |
| `AboutButton` | On Click | `MainMenuManager` | `OnAboutClicked()` |
| `SettingsButton` | On Click | `MainMenuManager` | `OnSettingsClicked()` |
| `QuitButton` | On Click | `MainMenuManager` | `OnQuitClicked()` |
| `AboutPanel BackButton` | On Click | `MainMenuManager` | `OnAboutBackClicked()` |
| `SettingsPanel BackButton` | On Click | `MainMenuManager` | `OnSettingsBackClicked()` |
| `MusicSlider` | On Value Changed | `SettingsManager` | `OnMusicChanged(float)` |
| `SFXSlider` | On Value Changed | `SettingsManager` | `OnSFXChanged(float)` |
| `MuteToggle` | On Value Changed | `SettingsManager` | `OnMuteChanged(bool)` |
| `YesButton` | On Click | `MainMenuManager` | `OnQuitYesClicked()` |
| `NoButton` | On Click | `MainMenuManager` | `OnQuitNoClicked()` |

---

# ══════════════════════════════════════
# BAGIAN 6 — STRUKTUR HIERARCHY FINAL
# ══════════════════════════════════════

## GameScene:
```
GameScene
├── Main Camera
├── Directional Light
├── SeaFloor
├── Submarine          ← SubmarineController.cs
├── GameManager        ← GameManager.cs + ScoreManager.cs
├── UIManager          ← UIManager.cs
├── Spawners           ← ObstacleSpawner.cs + TreasureSpawner.cs
└── Canvas
    ├── HUD
    │   ├── ScoreText
    │   └── CoinText
    ├── GameOverPanel
    │   ├── GoScoreText
    │   ├── GoCoinText
    │   ├── GoBestScoreText
    │   ├── RetryButton
    │   ├── MenuButton
    │   └── HighScoreButton
    └── HighScorePanel
        ├── HsText
        └── BackButton
```

## MainMenu:
```
MainMenu
├── Main Camera
├── Directional Light
├── MainMenuManager    ← MainMenuManager.cs + SettingsManager.cs
└── Canvas
    ├── MainMenuPanel
    │   ├── TitleText
    │   ├── PlayButton
    │   ├── AboutButton
    │   ├── SettingsButton
    │   └── QuitButton
    ├── AboutPanel
    │   ├── AboutTitle
    │   ├── InfoText
    │   └── BackButton
    ├── SettingsPanel
    │   ├── MusicSlider
    │   ├── SFXSlider
    │   ├── MuteToggle
    │   └── BackButton
    └── QuitConfirmPanel
        ├── ConfirmText
        ├── YesButton
        └── NoButton
```

---

# ══════════════════════════════════════
# BAGIAN 7 — TEST & BUILD
# ══════════════════════════════════════

## Checklist Test di Unity Editor

1. Buka scene `MainMenu` → klik **Play** (▶)
2. Cek semua fungsi:

| Yang Dites | Hasil yang Diharapkan |
|---|---|
| Klik PLAY | Masuk ke GameScene ✅ |
| Arrow key kiri/kanan | Submarine pindah jalur ✅ |
| Skor naik | +1 tiap detik ✅ |
| Ambil Treasure (sphere kuning) | Coin +1, Score +10 ✅ |
| Kena Obstacle (cube merah) | Game Over panel muncul ✅ |
| Klik RETRY | Game restart dari awal ✅ |
| Klik MENU | Kembali ke MainMenu ✅ |
| Klik HIGH SCORE | Panel High Score muncul ✅ |
| Klik ABOUT | Panel info muncul ✅ |
| Klik SETTINGS | Slider volume muncul ✅ |
| Klik QUIT → YES | Keluar (di editor: stop play) ✅ |

## Build ke Android

1. **File → Build Settings** → pilih **Android** → **Switch Platform**
2. **Player Settings** → isi Company Name & Product Name
3. Sambungkan HP via USB (aktifkan USB Debugging di HP)
4. Klik **Build and Run**

---

# ══════════════════════════════════════
# TROUBLESHOOTING
# ══════════════════════════════════════

| Error / Masalah | Penyebab | Solusi |
|---|---|---|
| `NullReferenceException` | Ada referensi yang belum di-drag | Cek semua slot di Inspector sudah diisi |
| Submarine tidak bergerak | `isGameRunning = false` | Cek `GameManager.StartGame()` dipanggil di `Start()` |
| Obstacle tidak muncul | Prefab belum di-assign | Drag prefab Obstacle ke Spawners di Inspector |
| Collision tidak terdeteksi | `Is Trigger` belum dicentang | Cek **Is Trigger** di Collider Obstacle dan Treasure |
| Tag tidak bekerja | Tag belum dibuat | Pastikan tag `Obstacle` dan `Treasure` ada di **Project Settings → Tags** |
| Scene tidak bisa load | Nama scene salah | Cek nama scene di Build Settings sama persis dengan string di `SceneManager.LoadScene()` |
| Error `TMP not found` | TMP belum di-import | **Window → TextMeshPro → Import TMP Essential Resources** |
| Tombol tidak merespons | Event System hilang | Pastikan ada **EventSystem** di Hierarchy (otomatis dibuat bersama Canvas) |

---

> ## ✅ Selamat! Core Loop Submarine & Treasure sudah jalan!
>
> Game flow sesuai diagram: **Main Menu → Play → Core Gameloop → Game Over → Retry / Menu / High Score** sudah lengkap.
>
> **Langkah selanjutnya yang bisa ditambahkan:**
> - 🎵 Background music dan sound effects
> - 💥 Efek partikel saat kena obstacle
> - 🌊 Scrolling background / ocean tiles
> - 📱 Optimasi mobile (resolusi, aspect ratio)
> - 🏆 Leaderboard online (Unity Gaming Services)
> - 🎨 Model 3D submarine yang lebih bagus
