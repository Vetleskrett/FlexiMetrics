<script lang="ts">
	import { page } from '$app/stores';
	import CreateOrEditCourse from 'src/components/CreateOrEditCourse.svelte';
	import { editCourse } from 'src/api';
	import { Semester, type Course } from 'src/types';
	import { goto } from '$app/navigation';

	const courseId = $page.params.courseId;

	export let data: {
		course: Course;
	};

	async function editCoursePage(name: string, code: string, year: number, semester: Semester) {
		try {
			await editCourse(courseId, {
				name: name,
				code: code,
				year: year,
				semester: semester == Semester.Spring ? 0 : 1
			});
			goto(`/teacher/courses/${courseId}`);
		} catch (exception) {
			console.error('Something Went Wrong!');
		}
	}
</script>

<CreateOrEditCourse
	courseName={data.course.name}
	courseCode={data.course.code}
	year={data.course.year}
	semester={data.course.semester}
	edit={true}
	submitFunction={editCoursePage}
	redirect={`/teacher/courses/${courseId}`}
/>
