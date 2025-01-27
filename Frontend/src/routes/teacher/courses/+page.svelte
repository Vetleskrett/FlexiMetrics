<script lang="ts">
	import { Role, type Course } from 'src/types';
	import { getCourses } from 'src/api';
	import { onMount } from 'svelte';
	import CoursesCard from 'src/components/CoursesCard.svelte';

	let courses: Course[] = [];

	onMount(async () => {
		try {
			courses = await getCourses();
		} catch (error) {
			console.error(error);
		}
	});
</script>

<CoursesCard userRole={Role.Teacher} {courses} />
