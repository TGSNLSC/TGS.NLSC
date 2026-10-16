const tgsApiOptions = {
    BaseUrl: "https://YOUR_TGS_API_HOST/tgs_api/",
    ApiKey: "YOUR_TGS_API_KEY"
};

const sceneLayerPath = "3dms/I3S/building/i3s/SceneServer/layers/0";

document.addEventListener("DOMContentLoaded", GetApiResponse);

async function GetApiResponse() {
    const requestUrl = document.getElementById("requesturl");
    const status = document.getElementById("httpstatus");
    const responseContent = document.getElementById("apiresponse");
    const controller = new AbortController();
    let timeoutId;

    requestUrl.textContent = "";
    status.textContent = "Loading...";
    responseContent.textContent = "";

    try {
        if (!tgsApiOptions.BaseUrl.trim() || !tgsApiOptions.ApiKey.trim()
            || tgsApiOptions.ApiKey === "YOUR_TGS_API_KEY") {
            throw new Error("Set BaseUrl and ApiKey in Sample.js before calling the API.");
        }

        const url = new URL(sceneLayerPath, tgsApiOptions.BaseUrl);
        requestUrl.textContent = url.href;
        timeoutId = setTimeout(() => controller.abort(), 30000);

        const response = await fetch(url, {
            method: "GET",
            headers: { "tgs-api-key": tgsApiOptions.ApiKey },
            signal: controller.signal
        });

        status.textContent = `HTTP ${response.status}`;
        const content = await response.text();
        responseContent.textContent = FormatJson(content);
    } catch (error) {
        status.textContent = "Request failed";
        responseContent.textContent = controller.signal.aborted
            ? "The request timed out after 30 seconds."
            : error instanceof TypeError
                ? "Request failed. Check the URL, network, HTTPS certificate and server CORS settings."
                : error.message;
    } finally {
        clearTimeout(timeoutId);
    }
}

function FormatJson(content) {
    try {
        return JSON.stringify(JSON.parse(content), null, 2);
    } catch {
        return content;
    }
}
