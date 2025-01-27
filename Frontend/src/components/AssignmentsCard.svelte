<script lang="ts">
	import type { Assignment } from 'src/types.ts';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import Plus from 'lucide-svelte/icons/plus';
	import CustomButton from './CustomButton.svelte';
	import { Role } from 'src/types';

	export let userRole: Role;
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

		{#if userRole == Role.Teacher}
			<CustomButton href="{courseId}/assignments/new" color="green">
				<Plus />
				<p>New</p>
			</CustomButton>
		{/if}
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
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
						<p class="m-0 font-semibold text-gray-500">{assignment.dueDate.split('T')[0]}</p>
					</div>
				</a>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
