<script lang="ts">
	import { Role, type Course } from 'src/types';
	import CoursesCard from 'src/components/CoursesCard.svelte';

	export let data: {
		courses: Course[];
	};
</script>

<CoursesCard userRole={Role.Student} courses={data.courses} />
