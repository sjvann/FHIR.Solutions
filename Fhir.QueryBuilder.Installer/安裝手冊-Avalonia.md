# FHIR Query Builder Desktop（Avalonia）— 建置、發布與安裝手冊

本文件供 **交付／建置人員** 與 **終端使用者** 使用：前半為官方發布流程與指令，後半為客戶端安裝與設定說明。  
（對照：**網頁／本機瀏覽器版**見同目錄之 `安裝手冊.md`。）

---

## 第一部分：建置與發布（交付工程師）

### 環境需求

| 項目 | 說明 |
|------|------|
| 作業系統（建置機） | Windows x64 |
| .NET SDK | **10.0**（與方案 `net10.0` 一致） |
| Python | **3.x**（執行建置腳本；僅標準函式庫） |
| Inno Setup | **6.x**（需 `ISCC.exe`，用於編譯 `.exe` 安裝程式） |

Inno Setup 下載：<https://jrsoftware.org/isdl.php>

---

### 最終發布指令（Windows 離線安裝程式，建議）

於儲存庫根目錄或下列目錄開啟終端機，建置完成後安裝檔位於 **`artifacts\FHIRQueryBuilder-Avalonia-Setup-<版本>.exe`**。

```powershell
cd Fhir.QueryBuilder.Installer
py build_installer_avalonia.py
```

等同於：

```powershell
cd Fhir.QueryBuilder.Installer
python build_installer_avalonia.py
```

**不重遞增版號**（重建同一版時）：

```powershell
py build_installer_avalonia.py --no-bump
```

指定組態：

```powershell
py build_installer_avalonia.py --configuration Release
```

**產出說明**

| 產出 | 路徑 |
|------|------|
| 客戶安裝程式 | `artifacts\FHIRQueryBuilder-Avalonia-Setup-<版本>.exe`（版號見儲存庫根目錄 `Fhir.QueryBuilder.Version.props` 內 `FHIRQueryBuilderVersion`） |
| 未打包之安裝內容（除錯／手動打包） | `artifacts\FHIRQueryBuilderAvaloniaInstallerStaging\app\` |

若未安裝 Inno Setup，腳本仍會完成 **staging**（自包含發佈目錄），並提示以 Inno IDE 或 `ISCC.exe` 手動編譯 `FHIRQueryBuilder-Avalonia.iss`。

---

### 對照：僅使用 .NET CLI（進階）

不安裝 Python 時，可手動發佈後再於 `Fhir.QueryBuilder.Installer` 目錄執行 Inno Setup 編譯 `FHIRQueryBuilder-Avalonia.iss`：

```powershell
dotnet publish Fhir.QueryBuilder.App.Avalonia\Fhir.QueryBuilder.App.Avalonia.csproj -c Release -r win-x64 --self-contained true -p:PublishReadyToRun=true -o artifacts\FHIRQueryBuilderAvaloniaInstallerStaging\app
```

編譯安裝程式時請傳入版號（與 `Fhir.QueryBuilder.Version.props` 一致）：

```powershell
& "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe" /DMyAppVersion=1.0.0 FHIRQueryBuilder-Avalonia.iss
```

---

## 第二部分：終端使用者安裝與設定

### 系統需求

| 項目 | 說明 |
|------|------|
| 作業系統 | Windows **10／11**，**64 位元**（x64） |
| .NET Runtime | **不需另行安裝**（安裝套件已內含自包含執行時） |
| 網路 | 連線 FHIR 伺服器及 OAuth（SMART）時依貴單位環境而定 |
| 管理員權限 | **安裝程式預設需提升權限**（安裝至 `Program Files`） |

---

### 安裝步驟

1. 取得交付之 **`FHIRQueryBuilder-Avalonia-Setup-<版本>.exe`**。
2. 連按兩下執行；若出現使用者帳戶控制（UAC），請選擇「是」。
3. 依精靈選擇安裝目錄（預設：`C:\Program Files\FHIR Query Builder Desktop`）。
4. 可選：建立桌面捷徑。
5. 完成後可勾選「啟動程式」結束精靈。

安裝目錄內含 **`安裝手冊-Avalonia.md`**、**`使用手冊-Avalonia.md`** 與程式檔案。

---

### 啟動程式

自「開始」選單或桌面捷徑執行 **FHIR Query Builder Desktop**（主程式為 **`Fhir.QueryBuilder.App.Avalonia.exe`**）。本產品為 **原生桌面視窗**，無需瀏覽器或本機 HTTP 埠。

---

### 設定檔（選用）

安裝目錄下的 **`appsettings.json`** 可調整預設 FHIR 端點、逾時、快取與 SMART 相關預設值。詳見 **`使用手冊-Avalonia.md`**。修改後請重新啟動程式。

---

### 解除安裝

透過 **Windows 設定 → 應用程式 → 已安裝的應用程式**，選擇 **FHIR Query Builder Desktop** 並解除安裝；或使用「新增或移除程式」。

---

### 常見問題

**Q：與網頁版（Blazor）有何不同？**  
網頁版透過本機服務在瀏覽器操作；桌面版為獨立視窗，功能對應同一套 Query Builder 核心，適合不需瀏覽器或偏好桌面程式的環境。

**Q：是否須安裝 IIS 或另開埠？**  
**不需要。** 桌面版不架設網頁伺服器。

**Q：查詢資料是否經過網際網路？**  
請求會依您設定的 FHIR 伺服器 URL 送出（可能為公網或院內網路）。

---

*文件版本與安裝套件版本請以 `Fhir.QueryBuilder.Version.props` 內 `FHIRQueryBuilderVersion` 與安裝檔檔名為準。*
