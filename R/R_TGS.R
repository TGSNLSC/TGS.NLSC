library(httr)
library(jsonlite)

tgs_api_options <- list(
  BaseUrl = "https://YOUR_TGS_API_HOST/tgs_api/",
  ApiKey = "YOUR_TGS_API_KEY"
)

scene_layer_path <- "3dms/I3S/building/i3s/SceneServer/layers/0"

format_json <- function(text) {
  tryCatch(
    jsonlite::prettify(text, indent = 2),
    error = function(error) text
  )
}

main <- function() {
  if (!nzchar(trimws(tgs_api_options$BaseUrl))
      || !nzchar(trimws(tgs_api_options$ApiKey))
      || tgs_api_options$ApiKey == "YOUR_TGS_API_KEY") {
    stop("Set BaseUrl and ApiKey before calling the API.", call. = FALSE)
  }

  url <- paste0(sub("/+$", "", tgs_api_options$BaseUrl), "/", scene_layer_path)
  cat("Request URL: ", url, "\n", sep = "")

  response <- GET(
    url,
    add_headers("tgs-api-key" = tgs_api_options$ApiKey),
    timeout(30)
  )

  cat("HTTP ", status_code(response), "\n", sep = "")
  cat("API response:\n")
  text <- content(response, as = "text", encoding = "UTF-8")
  cat(format_json(text), "\n", sep = "")
}

tryCatch(
  main(),
  error = function(error) {
    stop("Request failed: ", conditionMessage(error), call. = FALSE)
  }
)