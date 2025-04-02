<script lang="ts">
	import type { CourseStudent, SlimProgress } from 'src/types/';
	import * as Card from '$lib/components/ui/card';
	import * as Table from '$lib/components/ui/table';
	import { CircleCheck, CircleX } from 'lucide-svelte';

	export let students: CourseStudent[], courseId: string;
	export let studentsProgress: SlimProgress[];
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Content class="p-0">
		<Table.Root class="h-full">
			<Table.Header>
				<Table.Row>
					<Table.Head class="h-8 px-6 font-bold text-black">Name</Table.Head>
					<Table.Head class="h-8 px-6 font-bold text-black">Team</Table.Head>
					<Table.Head class="h-8 px-6 font-bold text-black">Assignments Delivered</Table.Head>
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each students as student}
					{@const studentProgress = studentsProgress.find((x) => x.id == student.id)}

					<a
						href="/teacher/courses/{courseId}/students/{student.id}"
						class="table-row hover:bg-blue-50"
					>
						<Table.Cell class="px-6">
							<p>{student.name}</p>
							<p class="text-xs text-gray-700">{student.email}</p>
						</Table.Cell>
						<Table.Cell class="px-6">
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
					</a>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
