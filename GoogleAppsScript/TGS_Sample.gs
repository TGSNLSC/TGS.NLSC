const tgsApiOptions = {
  BaseUrl: "https://YOUR_TGS_API_HOST/tgs_api/",
  ApiKey: "YOUR_TGS_API_KEY"
};

const sceneLayerPath = "3dms/I3S/building/i3s/SceneServer/layers/0";

function doGet(e) {
  try {
    if (!tgsApiOptions.BaseUrl.trim() || !tgsApiOptions.ApiKey.trim()
        || tgsApiOptions.BaseUrl.includes("YOUR_TGS_API_HOST")
        || tgsApiOptions.ApiKey === "YOUR_TGS_API_KEY") {
      throw new Error("Set BaseUrl and ApiKey in TGS_Sample.gs before calling the API.");
    }

    if (!/^https:\/\//i.test(tgsApiOptions.BaseUrl)) {
      throw new Error("BaseUrl must be an HTTPS URL reachable from Google Apps Script.");
    }

    const url = tgsApiOptions.BaseUrl.replace(/\/+$/, "") + "/" + sceneLayerPath;
    const response = UrlFetchApp.fetch(url, {
      method: "get",
      headers: { "tgs-api-key": tgsApiOptions.ApiKey },
      muteHttpExceptions: true
    });

    console.log("HTTP " + response.getResponseCode());
    let content = response.getContentText();
    let mimeType = ContentService.MimeType.JSON;

    try {
      content = JSON.stringify(JSON.parse(content), null, 2);
    } catch {
      mimeType = ContentService.MimeType.TEXT;
    }

    return ContentService.createTextOutput(content).setMimeType(mimeType);
  } catch (error) {
    return ContentService.createTextOutput(JSON.stringify({ error: error.message }, null, 2))
      .setMimeType(ContentService.MimeType.JSON);
  }
}