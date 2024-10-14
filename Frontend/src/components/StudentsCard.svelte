<script lang="ts">
	import type { Student } from '../types.ts';
	import * as Card from '$lib/components/ui/card/index.js';

	export let students: Student[], courseId: string;

	const count = students.length;
</script>

<Card.Root class="w-full overflow-hidden p-5 hover:bg-blue-50">
	<a href="/courses/{courseId}/students">
		<Card.Header class="m-0 p-0">
			<div class="flex items-center">
				<img
					width="32"
					height="32"
					src="https://img.icons8.com/fluency/480/student-male.png"
					alt="knowledge-sharing"
				/>
				<Card.Title class="m-0 ml-2 text-2xl">Students</Card.Title>
			</div>
		</Card.Header>
		<Card.Content class="p-0 pb-3 pt-3">
			<h1 class="m-auto text-center text-5xl font-semibold">{count}</h1>
		</Card.Content>
	</a>
</Card.Root>
