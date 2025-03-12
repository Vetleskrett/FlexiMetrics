<script lang="ts">
	import type { Assignment } from 'src/types/';
	import * as Card from '$lib/components/ui/card';
	import { Separator } from '$lib/components/ui/separator';
	import Plus from 'lucide-svelte/icons/plus';
	import CustomButton from '../CustomButton.svelte';

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

		<CustomButton href="{courseId}/assignments/new" color="green">
			<Plus />
			<p>New</p>
		</CustomButton>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			{#if assignments.length > 0}
				{#each assignments as assignment}
					<Separator class="w-full" />
					<a
						href="{courseId}/assignments/{assignment.id}"
						class="flex h-20 w-full items-center justify-between px-6 hover:bg-blue-50"
					>
						<h1 class="text-xl">{assignment.name}</h1>

						<div>
							{#if !assignment.published}
								<p class="ml-4 font-semibold text-red-500">DRAFT</p>
							{/if}
							<p class="m-0 font-semibold text-gray-500">
								{new Date(assignment?.dueDate).toLocaleDateString()}
							</p>
						</div>
					</a>
				{/each}
			{:else}
				<p class="mx-auto py-4">No assignments found</p>
			{/if}
		</div>
	</Card.Content>
</Card.Root>
