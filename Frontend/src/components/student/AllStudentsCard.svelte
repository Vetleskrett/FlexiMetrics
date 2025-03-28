<script lang="ts">
	import type { CourseStudent, Progress } from 'src/types/';
	import * as Card from '$lib/components/ui/card';
	import * as Table from '$lib/components/ui/table';
	import CustomButton from '../CustomButton.svelte';
	import Trash_2 from 'lucide-svelte/icons/trash-2';
	import { deleteStudentCourse } from 'src/api';
	import { CircleCheck, CircleX } from 'lucide-svelte';

	export let students: CourseStudent[], courseId: string;
	export let studentsProgress: Progress[];

	async function removeStudent(student: CourseStudent) {
		try {
			await deleteStudentCourse(courseId, student.id);
			deleteStudentFromList(student);
		} catch {
			console.error('Could not delete student');
		}
	}

	function deleteStudentFromList(student: CourseStudent) {
		const index = students.indexOf(student, 0);
		if (index > -1) {
			students.splice(index, 1);
			students = students;
		}
	}
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Content class="p-0">
		<Table.Root class="h-full">
			<Table.Header>
				<Table.Row>
					<Table.Head class="h-8 px-6 font-bold text-black">Name</Table.Head>
					<Table.Head class="h-8 px-6 font-bold text-black">Team</Table.Head>
					<Table.Head class="h-8 px-6 font-bold text-black">Progress</Table.Head>
					<Table.Head class="h-8 px-6 font-bold text-black"></Table.Head>
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each students as student}
					{@const studentProgress = studentsProgress.find((x) => x.id == student.id)}

					<Table.Row class="text-base ">
						<Table.Cell class="px-6">
							<p>{student.name}</p>
							<p class="text-xs text-gray-700">{student.email}</p>
						</Table.Cell>
						<Table.Cell class="px-6 text-center">
							<p>{student.teamNr || ''}</p>
						</Table.Cell>
						<Table.Cell class="px-6">
							<div class="flex flex-row items-center gap-4">
								{#if studentProgress}
									{#each studentProgress.assignmentsProgress as assignmentProgress}
										{#if assignmentProgress.isDelivered}
											<CircleCheck color="green" />
										{:else}
											<CircleX color="red" />
										{/if}
									{/each}
								{/if}
							</div>
						</Table.Cell>
						<Table.Cell>
							<CustomButton color="red" outline={true} on:click={() => removeStudent(student)}>
								<Trash_2 size="16" />
							</CustomButton>
						</Table.Cell>
					</Table.Row>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
