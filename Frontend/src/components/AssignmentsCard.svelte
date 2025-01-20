<script lang="ts">
	import type { Assignment } from 'src/types.ts';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import Plus from 'lucide-svelte/icons/plus';
	import CustomButton from './CustomButton.svelte';

	export let assignments: Assignment[];
	export let courseId: string;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="48"
				height="48"
				src="https://img.icons8.com/fluency/480/edit-text-file.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="ml-4 text-3xl">Assignments</Card.Title>
		</div>

		<CustomButton href="/courses/{courseId}/assignments/new" color="green">
			<Plus />
			<p>New</p>
		</CustomButton>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			{#each assignments as assignment}
				<Separator class="w-full" />
				<a
					href="/courses/{courseId}/assignments/{assignment.id}"
					class="h-18 flex w-full items-center justify-between p-6 hover:bg-blue-50"
				>
					<div class="flex items-center">
						<h1 class="text-xl">{assignment.name}</h1>
						{#if !assignment.published}
							<p class="ml-4 font-semibold text-gray-500">DRAFT</p>
						{/if}
					</div>

					<p class="font-semibold text-gray-500">{assignment.dueDate.split("T")[0]}</p> <!--Find a better solution than this has format 00-00-00T23.55-->
				</a>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
