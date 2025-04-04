<script lang="ts">
	import type { AssignmentProgress } from 'src/types/';
	import * as Card from '$lib/components/ui/card';
	import * as Table from '$lib/components/ui/table';
	import X from 'lucide-svelte/icons/x';
	import Check from 'lucide-svelte/icons/check';
	import { CircleAlert } from 'lucide-svelte';

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
					<Table.Head class="h-0 px-6 font-bold text-black">Grading</Table.Head>
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each assignmentsProgress as assignmentProgress}
					<a
						href={assignmentProgress.studentId
							? `/teacher/courses/${assignmentProgress.assignment.courseId}/assignments/${assignmentProgress.assignment.id}/students/${assignmentProgress.studentId}`
							: assignmentProgress.teamId
								? `/teacher/courses/${assignmentProgress.assignment.courseId}/assignments/${assignmentProgress.assignment.id}/teams/${assignmentProgress.teamId}`
								: 'javascript:void(0);'}
						class="table-row h-20 hover:bg-blue-50"
					>
						<Table.Cell class="px-6">
							{assignmentProgress.assignment.name}
						</Table.Cell>
						<Table.Cell class="px-6">
							{new Date(assignmentProgress.assignment?.dueDate).toLocaleDateString()}
						</Table.Cell>
						{#if assignmentProgress.assignment.collaborationType == 'Teams' && !assignmentProgress.teamId}
							<Table.Cell class="px-6">
								<div class="align-center flex gap-1 font-semibold text-red-500">
									<CircleAlert size="20" />
									<p>Missing team</p>
								</div>
							</Table.Cell>
							<Table.Cell></Table.Cell>
						{:else}
							<Table.Cell class="px-6">
								{#if assignmentProgress.isDelivered}
									<Check color="green" />
								{:else}
									<X color="red" />
								{/if}
							</Table.Cell>
							<Table.Cell class="px-6">
								{#if assignmentProgress.assignment.gradingType == 'NoGrading'}
									<p>N/A</p>
								{:else if !assignmentProgress.feedback}
									<p class="font-semibold text-red-500">No feedback</p>
								{:else if assignmentProgress.assignment.gradingType == 'ApprovalGrading'}
									<div class="flex h-full items-center justify-start gap-1">
										{#if assignmentProgress.feedback.isApproved}
											<Check color="green" />
											<p>Approved</p>
										{:else}
											<X color="red" />
											<p>Disapproved</p>
										{/if}
									</div>
								{:else if assignmentProgress.assignment.gradingType == 'LetterGrading'}
									<div class="text-center text-base">
										<p>{assignmentProgress.feedback.letterGrade}</p>
									</div>
								{:else if assignmentProgress.assignment.gradingType == 'PointsGrading'}
									<div class="whitespace-nowrap text-base">
										<p>
											{assignmentProgress.feedback.points} / {assignmentProgress.assignment
												.maxPoints}
										</p>
									</div>
								{/if}
							</Table.Cell>
						{/if}
					</a>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
