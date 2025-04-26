let pollingInterval = null;
let prevContent = '';
let ctxTaskId = null; // taskId будет возвращен сервером

function startPollingContext(stepId, taskId) {
    if (pollingInterval !== null) clearInterval(pollingInterval);
    prevContent = "";

    pollingInterval = setInterval(function () {
        $.ajax({
            url: '/Teach/Step/GetContent',
            type: 'GET',
            data: { id: stepId, taskId: taskId, keyType: 0 },
            success: function (response) {
                if (response.isSuccess) {
                    // Если новый текст
                    if (response.content !== prevContent) {
                        document.getElementById("step-context").value = response.content;
                        prevContent = response.content;
                    }
                    // Если транскрипция завершилась — останавливаем polling
                    if (response.isEnd) {
                        clearInterval(pollingInterval);
                        pollingInterval = null;
                    }
                }
            },
            error: function (xhr) {
                // тут можно что-то показать или просто проигнорировать
            }
        });
    }, 3000);
}

function startPollingResume(stepId, taskId) {
    if (pollingInterval !== null) clearInterval(pollingInterval);
    prevContent = "";

    pollingInterval = setInterval(function () {
        $.ajax({
            url: '/Teach/Step/GetContent',
            type: 'GET',
            data: { id: stepId, taskId: taskId, keyType: 1 },
            success: function (response) {
                if (response.isSuccess) {
                    // Если новый текст
                    if (response.content !== prevContent) {
                        document.getElementById("step-resume").value = response.content;
                        prevContent = response.content;
                    }
                    // Если транскрипция завершилась — останавливаем polling
                    if (response.isEnd) {
                        clearInterval(pollingInterval);
                        pollingInterval = null;
                    }
                }
            },
            error: function (xhr) {
                // тут можно что-то показать или просто проигнорировать
            }
        });
    }, 3000);
}

