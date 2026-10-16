package com.iisigroup;

import java.net.URI;
import java.nio.charset.StandardCharsets;

import org.apache.http.client.config.RequestConfig;
import org.apache.http.client.methods.CloseableHttpResponse;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.HttpClients;
import org.apache.http.util.EntityUtils;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;

public class TGSApi {

	private static final String BASE_URL = "https://YOUR_TGS_API_HOST/tgs_api/";
	private static final String API_KEY = "YOUR_TGS_API_KEY";
	private static final String SCENE_LAYER_PATH = "3dms/I3S/building/i3s/SceneServer/layers/0";

	public static void main(String[] args) {
		try {
			if (BASE_URL.trim().isEmpty() || API_KEY.trim().isEmpty()
					|| "YOUR_TGS_API_KEY".equals(API_KEY)) {
				throw new IllegalArgumentException("Set BASE_URL and API_KEY before calling the API.");
			}

			URI url = URI.create(BASE_URL).resolve(SCENE_LAYER_PATH);
			System.out.println("Request URL: " + url);
			HttpGet request = new HttpGet(url);
			request.setHeader("tgs-api-key", API_KEY);

			RequestConfig config = RequestConfig.custom()
					.setConnectTimeout(30000)
					.setConnectionRequestTimeout(30000)
					.setSocketTimeout(30000)
					.build();

			try (CloseableHttpClient httpClient = HttpClients.custom()
					.setDefaultRequestConfig(config).build();
					CloseableHttpResponse response = httpClient.execute(request)) {
				System.out.println("HTTP " + response.getStatusLine().getStatusCode());
				String content = response.getEntity() == null ? ""
						: EntityUtils.toString(response.getEntity(), StandardCharsets.UTF_8);
				System.out.println("API response:");
				System.out.println(formatJson(content));
			}
		} catch (Exception error) {
			System.err.println("Request failed: " + error.getMessage());
			System.exit(1);
		}
	}

	private static String formatJson(String content) {
		try {
			ObjectMapper objectMapper = new ObjectMapper();
			JsonNode data = objectMapper.readTree(content);
			return data == null ? content : objectMapper.writerWithDefaultPrettyPrinter()
					.writeValueAsString(data);
		} catch (JsonProcessingException error) {
			return content;
		}
	}
}