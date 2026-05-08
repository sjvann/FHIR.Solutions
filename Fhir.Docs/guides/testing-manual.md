# 資源產生測試手冊

## 測試架構（雙層）

| 層級 | 專案 | 職責 |
|------|------|------|
| 第一層 | **Fhir.Resource.Tests.Common** | JSON 回合輔助（`FhirJsonRoundTrip`）、線上格式編碼介面（JSON 實作／XML stub）、Fixture 讀檔等 |
| 第二層 | **各 `{OutputProjectName}.Tests`**（產生） | 每一資源一組薄測試：最小 JSON smoke、`FhirJsonRoundTrip` 反序列化／序列化 |

產生器會為每個已產出之資源類型建立 `*SerializationTests.cs`，預設包含 **一個** `Deserialize_minimal_json_roundtrip` 事實測試。

## Visual Studio 測試總管（Test Explorer）

產生之 **`Fhir.Resources.R5.Tests`** 若未加入方案，VS **不會**載入該專案，測試總管也無法探索其中的 xUnit 測試。

本方案已在 **`Fhir.Solution.slnx`** 納入：

- `Fhir.ResourceCreator/generated/Fhir.Resources.R5/Fhir.Resources.R5.csproj`
- `Fhir.ResourceCreator/generated/Fhir.Resources.R5/Fhir.Resources.R5.Tests/Fhir.Resources.R5.Tests.csproj`

請以 **`Fhir.Solution.slnx`** 開啟方案後：

1. 若尚未產生程式碼，請先於 `Fhir.ResourceCreator` 執行 `dotnet run`，使上述路徑存在且可建置。
2. **建置方案**（Ctrl+Shift+B）或至少建置測試專案。
3. 開啟 **測試總管**，必要時按 **重新整理**；應可見 `Fhir.Resources.R5.Tests` 內各項測試。

若儲存庫clone後尚未執行產生器，方案可能出現 **找不到專案檔** 或載入失敗；先生成 `generated/` 目錄即可。

## 本機執行測試

於產生之測試專案目錄：

```powershell
cd path\to\FHIR.Solutions\Fhir.ResourceCreator\generated\Fhir.Resources.R5\Fhir.Resources.R5.Tests
dotnet test -c Release
```

若僅建置測試專案而不執行：

```powershell
dotnet build -c Release
```

## 與方案內其他測試的關係

- **Fhir.TypeFramework.Tests**：驗證基底型別與 `FhirJsonSerializer` 等；與產生之資源組件測試互補，但 **不** 取代各資源 smoke。
- **產生之 Tests**：依賴 **資源組件** + **Tests.Common** + **TypeFramework**（依發射之 `.csproj` 而定）。

## Smoke 測試在做什麼（解析／產出 JSON）

每一個 `*SerializationTests` 預設包含 **`Deserialize_minimal_json_roundtrip`**：

1. **輸入**：組出最小 FHIR JSON 字串（含 `resourceType` 與 `id`）。
2. **解析**：`FhirJsonRoundTrip.Deserialize<T>`，經 `FhirJsonSerializer` 轉成強型別 POCO。
3. **產出**：`FhirJsonRoundTrip.Serialize` 再寫回 JSON 字串。
4. **斷言**：反序列化結果不為 null，且輸出字串仍含 `resourceType`。

測試內容透過 **`ITestOutputHelper.WriteLine`** 寫出 **Input JSON** 與 **Output JSON**。在 Visual Studio **測試總管** 執行單一測試後，選取該測試並開啟**輸出**／標準輸出視窗即可檢視；命令列可使用 `dotnet test --logger "console;verbosity=detailed"`（依環境與 xUnit 版本略有差異）。

## 測試涵義與限制

- **Smoke 測試**：以最小合法 JSON（含 `resourceType` 與 `id`）做 round-trip，確認 **型別可反序列化** 且序列化後仍帶有 `resourceType`；**不**保證與官方範例或 Profile 的欄位完整度一致。
- **非驗證器**：不取代 FHIR 官方 Validator；不保證與所有 Profile 或業務規則一致。
- **XML**：第一版僅 JSON；XML 編解碼預留於 Tests.Common，完整實作在 TypeFramework 演進後再接。

## 擴充測試（建議）

若需加深覆蓋：

1. 在 **Tests.Common** 使用 `FhirTestFixture.ReadUtf8File`／內嵌資源載入 **真實範例 JSON**。
2. 在對應資源之 `*SerializationTests.cs` 新增 **Fact**／**Theory**，針對該資源断言關鍵欄位或集合長度。
3. 大規模 CI：對 `generated` 下每個測試專案執行 `dotnet test`（可撰寫批次指令或 pipeline 矩陣）。

## CI 建議流程（概念）

1. 還原與建置 **Fhir.TypeFramework**、**Fhir.Resource.Tests.Common**。
2. 執行 **Fhir.ResourceCreator**（或使用已提交之 `generated` 產物之一致性 commit 策略）。
3. `dotnet build`／`dotnet test` 產生之 `*.Tests`。
4. （選用）`dotnet pack` 資源組件並上傳工件。

## 疑難排解

| 現象 | 可能原因 |
|------|----------|
| 測試通過但執行期失敗 | Smoke 僅覆蓋最小 JSON；實際 payload 觸及未完整對應之元素或型別 |
| 大量測試失敗 | TypeFramework 或產生器與該版 FHIR 套件不一致；檢查組件與 TF 版本 |
| 找不到 Tests.Common | 確認發射之測試 `.csproj` 中 **ProjectReference** 路徑相對於 repo 根目錄仍有效 |
