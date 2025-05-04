function showLoading(button) {
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
	const original = button.data('original-content');
	button.html(original);
}