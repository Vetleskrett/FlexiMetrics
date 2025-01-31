<script lang="ts">
	import CreateOrEditCourse from 'src/components/CreateOrEditCourse.svelte';
	import { addTeacherToCourse, postCourse } from 'src/api';
	import { Semester } from 'src/types';
	import { goto } from '$app/navigation';
	import { teacherEmail } from 'src/store';

	async function addCourse(name: string, code: string, year: number, semester: Semester) {
		try {
			await postCourse({
				name: name,
				code: code,
				year: year,
				semester: semester == Semester.Spring ? 0 : 1
			}).then(async (result) => {
				var course = result.data;
				await addTeacherToCourse(course.id, { email: teacherEmail });
			});
			goto('/teacher/courses');
		} catch (exception) {
			console.error('Something Went Wrong!');
		}
	}
</script>

<CreateOrEditCourse submitFunction={addCourse} redirect={'/teacher/courses'} />
