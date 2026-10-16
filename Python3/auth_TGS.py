import json
import sys
from urllib.parse import urljoin

import requests


tgs_api_options = {
    "BaseUrl": "https://YOUR_TGS_API_HOST/tgs_api/",
    "ApiKey": "YOUR_TGS_API_KEY",
}

SCENE_LAYER_PATH = "3dms/I3S/building/i3s/SceneServer/layers/0"


def format_json(content):
    try:
        return json.dumps(json.loads(content), ensure_ascii=False, indent=2)
    except ValueError:
        return content


def main():
    try:
        if (
            not tgs_api_options["BaseUrl"].strip()
            or not tgs_api_options["ApiKey"].strip()
            or tgs_api_options["ApiKey"] == "YOUR_TGS_API_KEY"
        ):
            raise ValueError("Set BaseUrl and ApiKey before calling the API.")

        url = urljoin(tgs_api_options["BaseUrl"], SCENE_LAYER_PATH)
        print(f"Request URL: {url}")

        with requests.get(
            url,
            headers={"tgs-api-key": tgs_api_options["ApiKey"]},
            timeout=30,
        ) as response:
            print(f"HTTP {response.status_code}")
            print("API response:")
            print(format_json(response.text))
        return 0
    except requests.Timeout:
        print("Request timed out.", file=sys.stderr)
        return 1
    except (requests.RequestException, ValueError) as error:
        print(f"Request failed: {error}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())