<script lang="ts">
	import type { Assignment, AssignmentProgress } from 'src/types/';
	import * as Card from '$lib/components/ui/card';
	import * as Table from '$lib/components/ui/table';
	import Check from 'lucide-svelte/icons/check';
	import X from 'lucide-svelte/icons/x';

	export let assignments: Assignment[];
	export let assignmentsProgress: AssignmentProgress[];
	export let courseId: string;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-2 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="48"
				height="48"
				src="https://img.icons8.com/fluency/480/edit-text-file.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="ml-4 text-3xl">Assignments</Card.Title>
		</div>
	</Card.Header>
	<Card.Content class="flex p-0">
		{#if assignments.length > 0}
			<Table.Root class="h-full">
				<Table.Header>
					<Table.Row>
						<Table.Head class="h-8 px-6 font-bold text-black">Name</Table.Head>
						<Table.Head class="h-8 px-6 font-bold text-black">Due</Table.Head>
						<Table.Head class="h-8 px-6 font-bold text-black">Delivered</Table.Head>
					</Table.Row>
				</Table.Header>
				<Table.Body>
					{#each assignments as assignment}
						{@const assignmentProgress = assignmentsProgress.find((x) => x.id == assignment.id)}
						<a
							href="/student/courses/{courseId}/assignments/{assignment.id}"
							class="table-row h-20 hover:bg-blue-50"
						>
							<Table.Cell class="px-6">
								<h1 class="text-xl">{assignment.name}</h1>
							</Table.Cell>
							<Table.Cell class="w-32 px-6">
								<p>{new Date(assignment?.dueDate).toLocaleDateString()}</p>
							</Table.Cell>
							<Table.Cell class="flex h-full items-center justify-center px-6">
								{#if assignmentProgress && assignmentProgress.isDelivered}
									<Check color="green" />
								{:else}
									<X color="red" />
								{/if}
							</Table.Cell>
						</a>
					{/each}
				</Table.Body>
			</Table.Root>
		{:else}
			<p class="mx-auto py-4">No assignments found</p>
		{/if}
	</Card.Content>
</Card.Root>
