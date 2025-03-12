<script lang="ts">
	import 'src/app.css';
	import { page } from '$app/stores';
	import MenuRow from 'src/components/MenuRow.svelte';
	import type { Analyzer } from 'src/types/';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;
	$: currentPath = $page.url.pathname;

	export let data: {
		analyzers: Analyzer[];
	};
</script>

<div class="fixed flex h-full w-60 flex-row flex-col items-start bg-white pt-12">
	<MenuRow
		href="/teacher/courses/{courseId}/assignments/{assignmentId}"
		isActive={currentPath.endsWith(assignmentId)}
	>
		<img
			width="24"
			height="24"
			src="https://img.icons8.com/fluency/480/edit-text-file.png"
			alt="knowledge-sharing"
		/>
		<h2>Overview</h2>
	</MenuRow>

	<MenuRow
		href="/teacher/courses/{courseId}/assignments/{assignmentId}/deliveries"
		isActive={currentPath.endsWith('deliveries')}
	>
		<img
			width="24"
			height="24"
			src="https://img.icons8.com/fluency/480/pass.png"
			alt="knowledge-sharing"
		/>
		<h2>Deliveries</h2>
	</MenuRow>

	<MenuRow
		href="/teacher/courses/{courseId}/assignments/{assignmentId}/feedback"
		isActive={currentPath.endsWith('feedback')}
	>
		<img
			width="24"
			height="24"
			src="https://img.icons8.com/fluency/480/popular-topic--v1.png"
			alt="knowledge-sharing"
		/>
		<h2>Feedback</h2>
	</MenuRow>

	<div class="mt-12 flex items-center gap-2 px-4">
		<img
			width="24"
			height="24"
			src="https://img.icons8.com/fluency/480/artificial-intelligence--v2.png"
			alt="knowledge-sharing"
		/>
		<h2 class="font-bold">Analyzers</h2>
	</div>

	{#each data.analyzers as analyzer}
		<MenuRow
			href="/teacher/courses/{courseId}/assignments/{assignmentId}/analyzers/{analyzer.id}"
			isActive={currentPath.endsWith(analyzer.id)}
		>
			<h2 class="text-base">{analyzer.name}</h2>
		</MenuRow>
	{/each}
</div>
<div class="pl-60">
	<slot />
</div>
