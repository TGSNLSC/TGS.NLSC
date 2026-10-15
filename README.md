# TGS API 介接範例程式碼說明

本專案提供多種程式語言的 TGS API 介接範例，供開發者參考。範例使用 API Key 呼叫 I3S 建物圖層服務，取得圖層資訊並顯示回應內容。

## API 認證與呼叫方式

本範例直接在 HTTP 請求標頭帶入 `tgs-api-key`，不需要先取得 Access Token，也不使用 Client Id、Client Secret 或 Bearer Token。

請先向 TGS 服務管理者取得可連線的 API 基底網址及有效金鑰。

| 設定 | 說明 |
| --- | --- |
| `BaseUrl` | API 基底網址，例如 `https://YOUR_TGS_API_HOST/tgs_api/`；結尾須保留 `/`，避免相對路徑取代最後一段網址。 |
| `ApiKey` | 服務管理者提供的 API Key，透過 `tgs-api-key` 請求標頭傳送。 |
| API 相對路徑 | 本範例使用 `3dms/I3S/building/i3s/SceneServer/layers/0`，用來指定要取得的服務資源。 |

範例使用 HTTP GET，將 `BaseUrl` 與相對路徑組合為完整網址：

```text
https://xxx.xxx.xxx.xxx/tgsApi/3dms/I3S/building/i3s/SceneServer/layers/0
```

上述網址是示意值，請以服務管理者提供的實際網址為準。若使用內網 IP，執行環境必須能連到該網路；雲端服務無法直接存取本機內網位址。

`SceneServer/layers/0` 表示取得場景服務中編號為 `0` 的圖層資訊。這個端點回傳圖層描述，不會直接下載所有建物模型或顯示 3D 畫面。

## 介接流程

### 1. 設定 API 網址與金鑰

在使用的範例設定檔或程式設定區填入基底網址與金鑰。以下僅示意設定值，實際設定格式與讀取方式依範例而定：

```json
{
     "BaseUrl": "https://YOUR_TGS_API_HOST/tgs_api/",
     "ApiKey": "YOUR_TGS_API_KEY"
}
```

`YOUR_TGS_API_KEY` 是占位值，使用前必須替換成有效金鑰。

### 2. 呼叫 API

以 HTTP GET 呼叫完整網址，並在請求標頭帶入 API Key：

```http
tgs-api-key: YOUR_TGS_API_KEY
```

以下為 Windows PowerShell 的 curl 呼叫範例：

```powershell
curl.exe --request GET `
     --url "https://YOUR_TGS_API_HOST/tgs_api/3dms/I3S/building/i3s/SceneServer/layers/0" `
     --header "tgs-api-key: YOUR_TGS_API_KEY" `
     --compressed
```

`--compressed` 讓 curl 協商並自動解壓壓縮回應。使用其他 HTTP 用戶端時，請確認其支援自動解壓；瀏覽器會自行處理壓縮，不需手動設定 `Accept-Encoding`。

可依執行環境設定合理的逾時，例如 30 秒。連線逾時、讀取逾時與整體請求逾時的意義不同，實際支援方式依 HTTP 工具而定。

### 3. 處理回應內容

成功時可收到 HTTP 200 與圖層 JSON。以下僅列出部分欄位，實際名稱及內容依服務資料而定：

```json
{
     "id": 0,
     "name": "台北市",
     "layerType": "3DObject"
}
```

請先確認 HTTP 狀態碼，再處理回應內容。JSON 可縮排顯示；非 JSON 則保留原文，不要強制解析。

HTTP 4xx 或 5xx 的回應可能是 JSON 或純文字／HTML，應保留狀態碼與錯誤內容以利排查；逾時、網路與憑證錯誤則可能在收到 HTTP 回應前發生。

## HTTPS 與金鑰安全

- API 伺服器憑證須有效，且網址必須符合憑證中的主機名稱或 IP。憑證鏈也必須受執行環境信任。
- 不同執行環境的憑證信任來源可能不同。瀏覽器成功不代表其他 HTTP 用戶端也已信任同一張憑證。
- 使用內部 CA 或受公司防毒／代理軟體的 HTTPS 過濾影響時，請由管理者提供正確 CA，依執行環境設定信任，不要以停用 TLS 驗證解決問題。
- 不要將真實 API Key 提交到 GitHub，也不要在日誌中輸出請求金鑰。瀏覽器中的 API Key 可被使用者查看，不適合用來保護機密金鑰；正式服務可改由受控後端代為呼叫。
- 瀏覽器直接呼叫 API 時，服務端須允許對應來源的 CORS 請求、預檢請求與 `tgs-api-key` 標頭。
- 若不希望影響電腦全域環境，請使用專案隔離或可攜式工具，並將套件與下載快取也限定在隔離目錄。

## 呼叫限制與問題排查

本範例未實作重試、快取或呼叫頻率控制。TGS 的可用圖層、金鑰權限與呼叫限制請依服務管理者提供的規範，不適用其他平台的 Token 或訂閱方案規則。

| 現象 | 建議檢查 |
| --- | --- |
| 設定缺少或為空 | 確認對應設定檔或程式設定區已填入必要值，且金鑰不再是占位值。 |
| HTTP 401／403 | 確認 `tgs-api-key` 有效，並向管理者確認金鑰的存取權限。 |
| HTTP 404 | 確認 `BaseUrl` 結尾的 `/`、相對路徑及圖層編號。 |
| 逾時或無法連線 | 確認內網／VPN、服務狀態、防火牆與執行環境的網路可達性。 |
| HTTPS 驗證失敗 | 檢查憑證有效期、主機名稱、完整憑證鏈及該語言的 CA 信任設定。 |
| 瀏覽器顯示 CORS 錯誤 | 確認伺服器允許頁面來源，以及 GET 請求、預檢請求與 `tgs-api-key` 標頭。 |
