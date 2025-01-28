<script lang="ts">
	import { Role, type Course } from 'src/types';
	import { getCoursesByTeacher } from 'src/api';
	import { onMount } from 'svelte';
	import CoursesCard from 'src/components/CoursesCard.svelte';
	import { teacherId } from 'src/store';

	let courses: Course[] = [];

	onMount(async () => {
		try {
			courses = await getCoursesByTeacher(teacherId);
		} catch (error) {
			console.error(error);
		}
	});
</script>

<CoursesCard userRole={Role.Teacher} {courses} />
