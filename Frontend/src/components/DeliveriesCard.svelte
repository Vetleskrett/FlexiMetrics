<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Table from '$lib/components/ui/table';
	import ArrowDownToline from 'lucide-svelte/icons/arrow-down-to-line';
	import type { Delivery, AssignmentField, Assignment, Student, Team } from 'src/types';
	import Check from 'lucide-svelte/icons/check';
	import X from 'lucide-svelte/icons/x';
	import Separator from 'src/lib/components/ui/separator/separator.svelte';
	import { getDeliveryFieldFile } from 'src/api';

	export let assignment: Assignment;
	export let assignmentFields: AssignmentField[];
	export let deliveries: Delivery[];
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

					<Table.Head class="h-8 w-2 p-0">
						<Separator orientation="vertical"></Separator>
					</Table.Head>

					{#each assignmentFields as field}
						<Table.Head class="h-8">
							{#if field.type == 'Integer'}
								<p class="text-right font-bold text-black">{field.name}</p>
							{:else if field.type == 'Boolean'}
								<p class="text-center font-bold text-black">{field.name}</p>
							{:else}
								<p class="font-bold text-black">{field.name}</p>
							{/if}
						</Table.Head>
					{/each}
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each ids as id}
					{@const delivery = deliveries.find((x) => x.studentId == id || x.teamId == id)}

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
							<Table.Cell class="flex h-full items-center justify-center">
								<p>{teams.find((x) => x.id == id)?.teamNr}</p>
							</Table.Cell>
						{/if}

						<Table.Cell class="w-2 p-0">
							<Separator orientation="vertical"></Separator>
						</Table.Cell>

						{#if delivery}
							{#each assignmentFields as assignmentField}
								{@const deliveryField = delivery.fields.find(
									(x) => x.assignmentFieldId == assignmentField.id
								)}

								<Table.Cell>
									{#if assignmentField.type == 'File'}
										<a class="flex items-center text-blue-500" download href={getDeliveryFieldFile(deliveryField?.id || '')}>
											<ArrowDownToline size="20" />
											{deliveryField?.value?.FileName}
										</a>
									{:else if assignmentField.type == 'Integer'}
										<p class="text-right">{deliveryField?.value}</p>
									{:else if assignmentField.type == 'Boolean'}
										{#if deliveryField?.value}
											<Check color="green" class="mx-auto" />
										{:else}
											<X color="red" class="mx-auto" />
										{/if}
									{:else if assignmentField.type == 'URL'}
										<a class="text-blue-500" target="_blank" href={deliveryField?.value}>{deliveryField?.value}</a>
									{:else}
										<p>{deliveryField?.value}</p>
									{/if}
								</Table.Cell>
							{/each}
						{:else}
							{#each assignmentFields as assignmentField}
								<Table.Cell></Table.Cell>
							{/each}
						{/if}
					</a>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
