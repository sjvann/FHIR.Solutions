# FHIR Query Builder Desktop — 使用手冊

本手冊說明 **FHIR Query Builder Desktop**（Avalonia 桌面版）之主要畫面與操作流程。功能與 **Blazor 網頁版**共用同一套 `Fhir.QueryBuilder.Core` 邏輯，差異主要在呈現方式（桌面視窗而非瀏覽器）。

---

## 1. 畫面總覽

| 區域 | 說明 |
|------|------|
| 頂部 | **FHIR base URL**、**Connect**／**Cancel**、**Resource type**、**Bearer access token**（選填）。若伺服器支援 SMART，可展開 OAuth 相關欄位與操作。 |
| 查詢 URL 列 | **Full query URL**，可由下方條件組合後按 **Build composed URL**；**Execute query** 執行 HTTP GET；**Clear URL** 清空欄位。 |
| 左欄 | 分頁：**Search parameters**（Capability 參數列表、Typed builder）、**Advanced**、**Modifying results**。 |
| 中欄 | **Draft query lines**（草稿條件）、**_include**／**_revinclude**。 |
| 右欄 | 查詢結果 **JSON**（文字或樹狀檢視）、**Copy**／**Save…**／**Clear**／**Tree view** 切換。 |
| 狀態列／頁尾 | 狀態訊息、版權與聯絡資訊。 |

---

## 2. 連線與資源類型

1. 在 **FHIR base URL** 輸入伺服器基底位址（例如公開測試伺服器或院內 FHIR 閘道）。  
2. 按 **Connect** 載入該伺服器之 **CapabilityStatement** 與資源／搜尋參數。  
3. 在 **Resource type** 選擇或輸入要查詢的資源（例如 `Patient`）。連線成功後，左側會載入該資源支援的搜尋參數。

若連線逾時或憑證／網路錯誤，請檢查 URL、防火牆與伺服器是否允許您的用戶端。

---

## 3. 搜尋參數與 Typed builder

### Search parameters 分頁

- **Capability search params**：列出該資源於 Capability 中宣告的搜尋參數。點選一列可搭配右側 **Typed builder**（若該型別已實作）以表單方式組條件。  
- **Typed builder**：依參數型別（字串、token、日期等）顯示對應欄位；使用 **Add typed clause to draft** 將條件加入中欄 **Draft query lines**。  
- 不支援型別之參數可改在中欄草稿手動調整，或使用 **Advanced** 分頁。

### Advanced 分頁

- **Chain**／**Reverse chain**、**Composite**、**Filter** 等進階語法與操作方式與核心文件一致；完成後同樣會反映在草稿或查詢 URL。

### Modifying results 分頁

- **_sort**、**_count**、**_summary**、**_elements**、**_include**／**_revinclude** 等結果修飾相關選項（實際可用項依伺服器與 UI 綁定為準）。

---

## 4. Draft query lines 與 Include

- **Draft query lines**：列出即將組進查詢字串的條件；可刪除選取列或 **Clear all**。  
- **_include**／**_revinclude**：勾選伺服器支援之 include 路徑；可選「對列出路徑全部送出」類選項（語意等同於為每個路徑產生參數）。  
- 按 **Build composed URL** 將草稿與 include 組進 **Full query URL**；再按 **Execute query** 送出。

---

## 5. 查詢結果（JSON）

- 成功時於右欄顯示回應本文（JSON）。  
- **Copy**：複製到剪貼簿。  
- **Save…**：另存新檔。  
- **Clear**：清空結果區。  
- **Tree view**（或同等標籤）：在文字與樹狀結構間切換（若目前綁定支援）。

---

## 6. 驗證與 Token

- **Bearer access token**：若伺服器需要 **Authorization: Bearer**，請於執行查詢前填入（輸入框可能以遮罩顯示）。  
- **SMART on FHIR**：當連線能力／設定允許時，可使用登入、刷新權杖、JWT client credentials 等流程（細節依伺服器註冊與 `appsettings.json` 而定）。

請勿將含有長效權杖或私鑰的螢幕截圖外流。

---

## 7. 設定檔（appsettings.json）

安裝目錄（或可攜式發佈目錄）下的 **`appsettings.json`** 節點 **`Fhir.QueryBuilder`** 常用鍵值：

| 鍵值 | 說明 |
|------|------|
| `DefaultServerUrl` | 啟動時預設顯示的 FHIR 基底 URL。 |
| `RequestTimeoutSeconds` | HTTP 請求逾時（秒）。 |
| `EnableLogging` | 是否啟用記錄（細節依實作）。 |
| `EnableCaching` | 是否快取 Capability 等資料以減少重複請求。 |
| `Smart` | **ClientId**、**RedirectUri**、**DefaultScopes**、**OAuthLoopbackTimeoutSeconds**、**ConfidentialPrivateKeyPem** 等 SMART 預設值。 |

修改後請重新啟動程式。若檔案受安裝目錄權限保護，可能需以系統管理員身分編輯，或由貴單位 IT 部署自訂組態。

---

## 8. 疑難排解

| 現象 | 建議 |
|------|------|
| Connect 失敗 | 確認 URL、TLS、代理伺服器；伺服器是否封鎖來自用戶端 IP。 |
| Execute 後 401／403 | 檢查 Bearer 或 SMART 流程是否取得有效權杖。 |
| 查詢結果為空 | 確認資源類型、搜尋參數與伺服端資料；部分伺服器區分 R4／R5。 |
| 程式無法啟動 | 確認為 x64 Windows；必要時自「新增或移除程式」修復安裝或重新安裝。 |

---

## 9. 文件與版本

- 安裝與離線發佈流程：**`安裝手冊-Avalonia.md`**。  
- 版號與 **Blazor／桌面版**安裝套件共用 **`Fhir.QueryBuilder.Version.props`** 之 **FHIRQueryBuilderVersion**（除非交付另有說明）。

---

*若您使用的為瀏覽器＋本機服務包裝之版本，請改參考安裝目錄內之 **`安裝手冊.md`**（非本檔）。*
