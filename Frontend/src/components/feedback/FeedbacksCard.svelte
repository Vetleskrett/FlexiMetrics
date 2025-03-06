<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Table from '$lib/components/ui/table';
	import type { Assignment, Student, Team, Feedback } from 'src/types/';
	import Check from 'lucide-svelte/icons/check';
	import X from 'lucide-svelte/icons/x';
	import Separator from 'src/lib/components/ui/separator/separator.svelte';

	export let assignment: Assignment;
	export let feedbacks: Feedback[];
	export let students: Student[];
	export let teams: Team[];

	teams.sort((a, b) => a.teamNr - b.teamNr);
	students.sort((a, b) => (a.name > b.name ? 1 : -1));

	const ids =
		assignment.collaborationType == 'Individual'
			? students.map((s) => s.id)
			: teams.map((t) => t.id);
</script>

<Card.Root class="w-[1080px] overflow-hidden p-0">
	<Card.Content class="p-0">
		<Table.Root class="h-full">
			<Table.Header>
				<Table.Row>
					{#if assignment.collaborationType == 'Individual'}
						<Table.Head class="h-8">
							<p class="font-bold text-black">Student</p>
						</Table.Head>
					{:else}
						<Table.Head class="flex h-8 items-center justify-center">
							<p class="font-bold text-black">Team</p>
						</Table.Head>
					{/if}
					<Table.Head class="h-8">
						<p class="font-bold text-black">Feedback</p>
					</Table.Head>

					{#if assignment.gradingType != 'NoGrading'}
						<Table.Head class="h-8">
							<p class="font-bold text-black">Grading</p>
						</Table.Head>
					{/if}
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each ids as id}
					{@const feedback = feedbacks.find((x) => x.studentId == id || x.teamId == id)}

					<a
						href={assignment.collaborationType == 'Individual'
							? `/teacher/courses/${assignment.courseId}/assignments/${assignment.id}/students/${id}`
							: `/teacher/courses/${assignment.courseId}/assignments/${assignment.id}/teams/${id}`}
						class="table-row hover:bg-blue-50"
					>
						{#if assignment.collaborationType == 'Individual'}
							<Table.Cell>
								<p>
									{students.find((x) => x.id == id)?.name}
								</p>
							</Table.Cell>
						{:else}
							<Table.Cell class="text-center">
								<p>{teams.find((x) => x.id == id)?.teamNr}</p>
							</Table.Cell>
						{/if}

						{#if feedback}
							<Table.Cell>
								<p>{feedback.comment}</p>
							</Table.Cell>

							{#if assignment.gradingType == 'ApprovalGrading'}
								<Table.Cell class="flex h-full items-center justify-center gap-1">
									{#if feedback.isApproved}
										<Check color="green" />
										<p>Approved</p>
									{:else}
										<X color="red" />
										<p>Disapproved</p>
									{/if}
								</Table.Cell>
							{:else if assignment.gradingType == 'LetterGrading'}
								<Table.Cell class="text-center text-lg">
									<p>{feedback.letterGrade}</p>
								</Table.Cell>
							{:else if assignment.gradingType == 'PointsGrading'}
								<Table.Cell class="whitespace-nowrap text-lg">
									<p>{feedback.points} / {assignment.maxPoints}</p>
								</Table.Cell>
							{/if}
						{:else}
							<Table.Cell>
								<p class="font-semibold text-red-500">No feedback given</p>
							</Table.Cell>
							{#if assignment.gradingType != 'NoGrading'}
								<Table.Cell></Table.Cell>
							{/if}
						{/if}
					</a>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
