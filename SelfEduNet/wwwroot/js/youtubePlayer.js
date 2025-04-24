function getYouTubeVideoId(url) {
    var pattern = /(?:v=|\/)([0-9A-Za-z_-]{11}).*/;
    var match = url.match(pattern);
    return match ? match[1] : null;
}

// Функция для вставки плеера YouTube
function updateYouTubePlayer(videoUrl) {
    var videoId = getYouTubeVideoId(videoUrl);
    if (videoId) {
        var embedUrl = "https://www.youtube.com/embed/" + videoId;
        var iframeHtml = '<iframe id="youtubePlayer" width="100%" height="315" src="' + embedUrl +
            '" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>';
        $('#youtubePlayerBox').html(iframeHtml);  // Вставляем плеер
    } else {
        $('#youtubePlayerBox').html('<span class="upload-video-placeholder d-block">🎥</span>'); // Если невалидная ссылка
    }
}
