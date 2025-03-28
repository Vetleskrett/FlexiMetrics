<script lang="ts">
	import type { AssignmentProgress, Assignment } from 'src/types/';
	import * as Card from '$lib/components/ui/card';
	import * as Table from '$lib/components/ui/table';
	import X from 'lucide-svelte/icons/x';
	import Check from 'lucide-svelte/icons/check';

	export let teamId: string;
	export let assignments: Assignment[];
	export let assignmentsProgress: AssignmentProgress[];
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
		<Table.Root>
			<Table.Header>
				<Table.Row>
					<Table.Head class="h-0 px-6 font-bold text-black">Name</Table.Head>
					<Table.Head class="h-0 px-6 font-bold text-black">Due</Table.Head>
					<Table.Head class="h-0 px-6 font-bold text-black">Delivered</Table.Head>
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each assignments as assignment}
					{@const assignmentProgress = assignmentsProgress.find((x) => x.id == assignment.id)}
					<a
						href="/teacher/courses/{assignment.courseId}/assignments/{assignment.id}/teams/{teamId}"
						class="table-row h-20 hover:bg-blue-50"
					>
						<Table.Cell class="px-6">
							{assignment.name}
						</Table.Cell>
						<Table.Cell class="px-6">
							{new Date(assignment?.dueDate).toLocaleDateString()}
						</Table.Cell>
						<Table.Cell class="px-6">
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
	</Card.Content>
</Card.Root>
