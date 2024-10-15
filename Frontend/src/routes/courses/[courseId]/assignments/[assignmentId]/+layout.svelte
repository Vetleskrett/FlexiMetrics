<script lang="ts">
	import 'src/app.css';
	import { page } from '$app/stores';
	import type { Analyzer } from 'src/types';
	import MenuRow from 'src/components/MenuRow.svelte';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;

	const analyzers: Analyzer[] = [
		{
			id: '1',
			name: 'Git Analyzer'
		},
		{
			id: '2',
			name: 'Lighthouse Analyzer'
		},
		{
			id: '3',
			name: 'Code Analyzer'
		}
	];
</script>

<div class="fixed flex h-full w-60 flex-row flex-col items-start bg-white pt-12">
	<MenuRow href="/courses/{courseId}/assignments/{assignmentId}">
		<img
			width="24"
			height="24"
			src="https://img.icons8.com/fluency/480/edit-text-file.png"
			alt="knowledge-sharing"
		/>
		<h2>Overview</h2>
	</MenuRow>
	<MenuRow href="/courses/{courseId}/assignments/{assignmentId}/deliveries">
		<img
			width="24"
			height="24"
			src="https://img.icons8.com/fluency/480/metamorphose.png"
			alt="knowledge-sharing"
		/>
		<h2>Deliveries</h2>
	</MenuRow>

	<div class="mt-12 flex items-center gap-2 pl-6">
		<img
			width="24"
			height="24"
			src="https://img.icons8.com/fluency/480/artificial-intelligence--v2.png"
			alt="knowledge-sharing"
		/>
		<h2 class="font-bold">Analyzers</h2>
	</div>

	{#each analyzers as analyzer}
		<MenuRow href="/courses/{courseId}/assignments/{assignmentId}/analyzers/{analyzer.id}">
			<h2>{analyzer.name}</h2>
		</MenuRow>
	{/each}
</div>
<div class="pl-60">
	<slot />
</div>
