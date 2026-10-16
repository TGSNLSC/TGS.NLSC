<?php

$tgsApiOptions = array(
	'BaseUrl' => 'https://YOUR_TGS_API_HOST/tgs_api/',
	'ApiKey' => 'YOUR_TGS_API_KEY'
);
$sceneLayerPath = '3dms/I3S/building/i3s/SceneServer/layers/0';
$curl = null;

header('Content-Type: text/plain; charset=utf-8');

try {
	if (trim($tgsApiOptions['BaseUrl']) === '' || trim($tgsApiOptions['ApiKey']) === ''
		|| $tgsApiOptions['ApiKey'] === 'YOUR_TGS_API_KEY') {
		throw new RuntimeException('Set BaseUrl and ApiKey before calling the API.');
	}

	$url = rtrim($tgsApiOptions['BaseUrl'], '/') . '/' . $sceneLayerPath;
	echo 'Request URL: ' . $url . PHP_EOL;

	$curl = curl_init($url);
	if ($curl === false) {
		throw new RuntimeException('Unable to initialize cURL.');
	}

	curl_setopt_array($curl, array(
		CURLOPT_HTTPGET => true,
		CURLOPT_HTTPHEADER => array('tgs-api-key: ' . $tgsApiOptions['ApiKey']),
		CURLOPT_RETURNTRANSFER => true,
		CURLOPT_ENCODING => '',
		CURLOPT_CONNECTTIMEOUT => 30,
		CURLOPT_TIMEOUT => 30,
		CURLOPT_SSL_VERIFYPEER => true,
		CURLOPT_SSL_VERIFYHOST => 2
	));

	$content = curl_exec($curl);
	if ($content === false) {
		throw new RuntimeException(curl_error($curl));
	}

	echo 'HTTP ' . curl_getinfo($curl, CURLINFO_HTTP_CODE) . PHP_EOL;
	echo 'API response:' . PHP_EOL;

	$data = json_decode($content);
	if (json_last_error() === JSON_ERROR_NONE) {
		$formatted = json_encode($data, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE);
		echo ($formatted !== false ? $formatted : $content) . PHP_EOL;
	} else {
		echo $content . PHP_EOL;
	}
} catch (Throwable $error) {
	http_response_code(500);
	echo 'Request failed: ' . $error->getMessage() . PHP_EOL;
} finally {
	if ($curl) {
		curl_close($curl);
	}
}
