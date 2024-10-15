<script lang="ts">
	import type { Analyzer } from 'src/types.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import Plus from 'lucide-svelte/icons/plus';

	export let analyzers: Analyzer[], assignmentId: string, courseId: string;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="48"
				height="48"
				src="https://img.icons8.com/fluency/480/artificial-intelligence--v2.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="ml-4 text-3xl">Analyzers</Card.Title>
		</div>

		<Button
			href="/courses/{courseId}/assignments/{assignmentId}/analyzers/new"
			class="flex h-9 flex-row justify-between gap-2 bg-button-green pl-2 pr-3 hover:bg-button-green-hover"
		>
			<Plus />
			<p>New</p>
		</Button>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			{#each analyzers as analyzer}
				<Separator class="w-full" />
				<a
					href="/courses/{courseId}/assignments/{assignmentId}/analyzers/{analyzer.id}"
					class="h-18 flex w-full items-center justify-between p-6 hover:bg-blue-50"
				>
					<h1 class="text-xl">{analyzer.name}</h1>
				</a>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
