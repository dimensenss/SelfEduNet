let pollingInterval = null;
let prevContent = '';
let ctxTaskId = null;
const maxRetries = 30;

function startPollingContext(stepId, taskId, button) {
	if (pollingInterval !== null) clearInterval(pollingInterval);

	let retryCount = 0;
	

	pollingInterval = setInterval(function () {
		if (retryCount >= maxRetries) {
			clearInterval(pollingInterval);
			pollingInterval = null;
			hideLoading(button); // <--- вернуть кнопку
			$.notify({ message: "Контекст не вдалося отримати після 10 спроб." }, { type: 'warning' });
			return;
		}

		$.ajax({
			url: '/Teach/Step/GetContent',
			type: 'GET',
			data: { id: stepId, taskId: taskId, keyType: 0 },
			success: function (response) {
				if (response.isSuccess) {
					if (response.content !== prevContent) {
						document.getElementById("step-context").value = response.content;
						prevContent = response.content;
					}

					if (response.isEnd) {
						clearInterval(pollingInterval);
						pollingInterval = null;
						hideLoading(button); // <--- вернуть кнопку
					}
				}
			},
			error: function () {
				retryCount++;
			}
		});
	}, 3000);
}
function startPollingResume(stepId, taskId, button) {
    if (pollingInterval !== null) clearInterval(pollingInterval);
	let retryCount = 0;

	pollingInterval = setInterval(function () {
		if (retryCount >= maxRetries) {
			clearInterval(pollingInterval);
			pollingInterval = null;
			hideLoading(button); // <--- вернуть кнопку
			$.notify({ message: "Резюме не вдалося отримати. Спробуйте ще раз." }, { type: 'warning' });
			return;
		}
        $.ajax({
            url: '/Teach/Step/GetContent',
            type: 'GET',
            data: { id: stepId, taskId: taskId, keyType: 1 },
            success: function (response) {
                if (response.isSuccess) {
                    // Если новый текст
                    if (response.content !== prevContent) {
                        resumeEditor.setData(response.content);
                        prevContent = response.content;
                    }
                
                    if (response.isEnd) {
                        clearInterval(pollingInterval);
						pollingInterval = null;
						hideLoading(button);
                    }
                }
            },
            error: function (xhr) {
				retryCount++;
            }
        });
    }, 3000);
}
function showLoading(button) {
	// Сохраняем исходный HTML кнопки в data-атрибут (если еще не сохранено)
	if (!button.data('original-content')) {
		button.data('original-content', button.html());
	}
	button.prop("disabled", true);
	button.html(`
        <span class="spinner-border spinner-border-sm" aria-hidden="true"></span>
        <span class="visually-hidden" role="status">Завантаження...</span>
    ` + '<span class="btn-text ms-2">' + button.data('original-content') + '</span>');
}

function hideLoading(button) {
	button.prop("disabled", false);
	// Восстанавливаем исходный HTML из data-атрибута
	const original = button.data('original-content');
	button.html(original);
}