function renderCoursesList(forTeacher = false, data) {
	if (forTeacher === true) {
		var url = "/Course/RenderTeacherCourses/";
	} else {
		var url = "/Course/RenderCoursesList/";
	}

	$.ajax({
		url: url,
		type: 'GET',
		data: data,
		success: function (response) {
			$('#courses-container').html(response);
		},
		error: function (xhr, status, error) {
			console.error("Error loading courses list: ", status, error);
		}
	});
}
$(document).ready(function () {
	$(document).on('click', 'a[data-delete-course-id]', function () {
		var courseId = $(this).data('delete-course-id');
		showConfirmationDialog(
			'Ви впевнені?',
			'Ця дія видаляє курс. Ви не зможете відновити його!',
			'Так, видалити!',
			'Відмінити',
			() => {
				$.ajax({
					url: "/Course/DeleteCourse/",
					type: 'POST',
					data: {
						courseId: courseId,
					},
					success: function (response) {
						$.notify({
							message: response.message
						}, {
							type: 'success'
						});
						//TODO check for teacher
						renderCoursesList(true);
					},
					error: function (xhr) {
						const response = xhr.responseJSON;
						const errorMessage = response && response.message ? response.message : "An unexpected error occurred.";
						$.notify({
							message: response.message
						}, {
							type: 'danger'
						});
					}
				});
			}
		);
	});
});