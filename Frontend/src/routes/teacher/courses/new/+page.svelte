<script lang="ts">
	import CreateOrEditCourse from 'src/components/CreateOrEditCourse.svelte';
	import { postCourse } from 'src/api';
	import { Semester } from 'src/types';
	import { goto } from '$app/navigation';
	import { teacherId } from 'src/store';

	async function addCourse(name: string, code: string, year: number, semester: Semester) {
		try {
			await postCourse({
				name: name,
				code: code,
				year: year,
				semester: semester == Semester.Spring ? 'Spring' : 'Autumn',
				teacherId: teacherId
			});
			goto('/teacher/courses');
		} catch (exception) {
			console.error('Something Went Wrong!');
		}
	}
</script>

<CreateOrEditCourse submitFunction={addCourse} redirect={'/teacher/courses'} />
