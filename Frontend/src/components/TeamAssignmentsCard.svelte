<script lang="ts">
	import type { AssignmentTeam } from 'src/types.ts';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import X from 'lucide-svelte/icons/x';
	import Check from 'lucide-svelte/icons/check';

	export let assignmentsTeam: AssignmentTeam[];
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="48"
				height="48"
				src="https://img.icons8.com/fluency/480/edit-text-file.png"
				alt="assignment-icon"
			/>
			<Card.Title class="ml-4 text-3xl">Assignments</Card.Title>
		</div>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			<div class="flex items-center px-6 text-sm font-bold">
				<h1 class="w-2/3">Name</h1>
				<h1 class="w-2/3">Due</h1>
				<h1 class="w-1/4">Delivered</h1>
			</div>
			{#each assignmentsTeam as assignment}
				<Separator class="w-full" />
				<div class="flex items-center px-6 py-4">
					<h1 class="w-2/3">{assignment.name}</h1>
					<h1 class="w-2/3">{assignment.due}</h1>
					<div class="w-1/4">
						{#if assignment.completed}
							<X color="red" />
						{:else}
							<Check color="green" />
						{/if}
					</div>
				</div>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
