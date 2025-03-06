<script lang="ts">
	import { Role, type Course } from 'src/types/';
	import CoursesCard from 'src/components/course/CoursesCard.svelte';

	export let data: {
		courses: Course[];
	};
</script>

<CoursesCard userRole={Role.Teacher} courses={data.courses} />
