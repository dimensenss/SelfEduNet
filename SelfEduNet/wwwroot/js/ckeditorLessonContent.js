
$(document).ready(function () {
	const {
		ClassicEditor,
		Autoformat,
		AutoImage,
		Autosave,
		BlockQuote,
		Bold,
		Code,
		Essentials,
		FontBackgroundColor,
		FontColor,
		FontFamily,
		FontSize,
		FullPage,
		GeneralHtmlSupport,
		Heading,
		Highlight,
		HtmlComment,
		HtmlEmbed,
		ImageBlock,
		ImageCaption,
		ImageInline,
		ImageInsert,
		ImageInsertViaUrl,
		ImageResize,
		ImageStyle,
		ImageTextAlternative,
		ImageToolbar,
		ImageUpload,
		Indent,
		IndentBlock,
		Italic,
		Link,
		LinkImage,
		List,
		ListProperties,
		MediaEmbed,
		Paragraph,
		PasteFromOffice,
		SimpleUploadAdapter,
		SourceEditing,
		Table,
		TableCaption,
		TableCellProperties,
		TableColumnResize,
		TableProperties,
		TableToolbar,
		TextTransformation,
		TodoList,
		Underline,
		WordCount
	} = window.CKEDITOR;

	const LICENSE_KEY =
		'eyJhbGciOiJFUzI1NiJ9.eyJleHAiOjE3Njk0NzE5OTksImp0aSI6ImZmODc4MjQ2LTNmZjktNDJiOS05OTkwLWEzZjNiZTNkMWZjZCIsImxpY2Vuc2VkSG9zdHMiOlsiMTI3LjAuMC4xIiwibG9jYWxob3N0IiwiMTkyLjE2OC4qLioiLCIxMC4qLiouKiIsIjE3Mi4qLiouKiIsIioudGVzdCIsIioubG9jYWxob3N0IiwiKi5sb2NhbCJdLCJ1c2FnZUVuZHBvaW50IjoiaHR0cHM6Ly9wcm94eS1ldmVudC5ja2VkaXRvci5jb20iLCJkaXN0cmlidXRpb25DaGFubmVsIjpbImNsb3VkIiwiZHJ1cGFsIl0sImxpY2Vuc2VUeXBlIjoiZGV2ZWxvcG1lbnQiLCJmZWF0dXJlcyI6WyJEUlVQIl0sInZjIjoiOWJmZWM3NzQifQ.OJoOoCj0ZE71rathues6fOhdJheLYjnBEXkwMXNrLrDvyzE4kDzPLanQ4aqTd4Szu5z416aIzxklOIKJmHcGOg';

	const editorConfig = {
		toolbar: {
			items: [
				'sourceEditing',
				'|',
				'heading',
				'|',
				'fontSize',
				'fontFamily',
				'fontColor',
				'fontBackgroundColor',
				'|',
				'bold',
				'italic',
				'underline',
				'code',
				'|',
				'link',
				'insertImage',
				'mediaEmbed',
				'insertTable',
				'highlight',
				'blockQuote',
				'htmlEmbed',
				'|',
				'bulletedList',
				'numberedList',
				'todoList',
				'outdent',
				'indent'
			],
			shouldNotGroupWhenFull: false
		},
		plugins: [
			Autoformat,
			AutoImage,
			Autosave,
			BlockQuote,
			Bold,
			Code,
			Essentials,
			FontBackgroundColor,
			FontColor,
			FontFamily,
			FontSize,
			FullPage,
			GeneralHtmlSupport,
			Heading,
			Highlight,
			HtmlComment,
			HtmlEmbed,
			ImageBlock,
			ImageCaption,
			ImageInline,
			ImageInsert,
			ImageInsertViaUrl,
			ImageResize,
			ImageStyle,
			ImageTextAlternative,
			ImageToolbar,
			ImageUpload,
			Indent,
			IndentBlock,
			Italic,
			Link,
			LinkImage,
			List,
			ListProperties,
			MediaEmbed,
			Paragraph,
			PasteFromOffice,
			SimpleUploadAdapter,
			SourceEditing,
			Table,
			TableCaption,
			TableCellProperties,
			TableColumnResize,
			TableProperties,
			TableToolbar,
			TextTransformation,
			TodoList,
			Underline,
			WordCount
		],
		fontFamily: {
			supportAllValues: true
		},
		fontSize: {
			options: [10, 12, 14, 'default', 18, 20, 22],
			supportAllValues: true
		},
		heading: {
			options: [
				{
					model: 'paragraph',
					title: 'Paragraph',
					class: 'ck-heading_paragraph'
				},
				{
					model: 'heading1',
					view: 'h1',
					title: 'Heading 1',
					class: 'ck-heading_heading1'
				},
				{
					model: 'heading2',
					view: 'h2',
					title: 'Heading 2',
					class: 'ck-heading_heading2'
				},
				{
					model: 'heading3',
					view: 'h3',
					title: 'Heading 3',
					class: 'ck-heading_heading3'
				},
				{
					model: 'heading4',
					view: 'h4',
					title: 'Heading 4',
					class: 'ck-heading_heading4'
				},
				{
					model: 'heading5',
					view: 'h5',
					title: 'Heading 5',
					class: 'ck-heading_heading5'
				},
				{
					model: 'heading6',
					view: 'h6',
					title: 'Heading 6',
					class: 'ck-heading_heading6'
				}
			]
		},
		htmlSupport: {
			allow: [
				{
					name: /^.*$/,
					styles: true,
					attributes: true,
					classes: true
				}
			]
		},
		image: {
			toolbar: [
				'toggleImageCaption',
				'imageTextAlternative',
				'|',
				'imageStyle:inline',
				'imageStyle:wrapText',
				'imageStyle:breakText',
				'|',
				'resizeImage'
			]
		},
		initialData: $('#editor').data('initial'),
	licenseKey: LICENSE_KEY,
		link: {
			addTargetToExternalLinks: true,
			defaultProtocol: 'https://',
			decorators: {
				toggleDownloadable: {
					mode: 'manual',
					label: 'Downloadable',
					attributes: {
						download: 'file'
					}
				}
			}
		},
		list: {
			properties: {
				styles: true,
				startIndex: true,
				reversed: true
			}
		},
		placeholder: 'Type or paste your content here!',
		table: {
			contentToolbar: ['tableColumn', 'tableRow', 'mergeTableCells', 'tableProperties', 'tableCellProperties']
		},
		autosave: {
			waitingTime: 3000,
			save(editor) {
				return saveData(editor.getData());
			}
		},
		simpleUpload: {
			uploadUrl: '/Teach/Step/UploadImage',
			withCredentials: true,  // Если требуется аутентификация
			headers: {
				'X-CSRF-TOKEN': 'your-csrf-token',
				'Authorization': 'Bearer your-token'
			}
		}
	};

	ClassicEditor.create(document.querySelector('#editor'), editorConfig).then(editor => {
		const wordCount = editor.plugins.get('WordCount');
		document.querySelector('#editor-word-count').appendChild(wordCount.wordCountContainer);

		return editor;
	});
	function saveData(content) {
		let stepId = $('#editor').data('step-id');
		return $.ajax({
			url: `/Teach/Step/UpdateStepContent/${stepId}`,
			type: 'POST',
			data: JSON.stringify(content),
			contentType: 'application/json',
			success: function () {
				let time = new Date().toLocaleTimeString();
				$('#saveStatus').text(`Збережено: ${time}`).css("color", "green");
			},
			error: function (xhr, status, error) {
				console.error("Ошибка автосохранения:", error);
				$('#saveStatus').text("Помилка збереження").css("color", "red");
			}
		});
	}
});
