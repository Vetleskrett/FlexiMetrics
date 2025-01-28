<script lang="ts">
	import { Role, type Course } from 'src/types';
	import { getCoursesByStudent } from 'src/api';
	import { onMount } from 'svelte';
	import CoursesCard from 'src/components/CoursesCard.svelte';
	import { studentId } from 'src/store';

	let courses: Course[] = [];

	onMount(async () => {
		try {
			courses = await getCoursesByStudent(studentId);
		} catch (error) {
			console.error(error);
		}
	});
</script>

<CoursesCard userRole={Role.Student} {courses} />
